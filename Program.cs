using System;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/api/{date}", (string date) =>
    {
        DateTime myDateTime = new DateTime();
        if (DateTime.TryParse(date, out myDateTime))
        {
            return Results.Ok(new DateTimeResponse(
                myDateTime.ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'"),
                DateTimeToUnix(myDateTime)));
        }


        long unixTimestamp = 0;
        if (long.TryParse(date, out unixTimestamp))
        {
            return Results.Ok(new DateTimeResponse(
                UnixTimeToDateTime(unixTimestamp).ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'"),
                unixTimestamp));
        }

        return Results.BadRequest(new DateTimeResponseError("Invalid Date"));
    })
    .WithName("GetDate")
    .WithOpenApi();

app.Run();


long DateTimeToUnix(DateTime MyDateTime)
{
    app.Logger.LogInformation($"myDateTime {MyDateTime}");
    TimeSpan timeSpan = MyDateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    app.Logger.LogInformation($"timeSpan {timeSpan}");

    return (long)timeSpan.TotalSeconds;
}

DateTime UnixTimeToDateTime(long unixtime)
{
    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
    return dtDateTime;
}

record DateTimeResponseError(string error)
{
}

record DateTimeResponse(string utc, long unix)
{
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}