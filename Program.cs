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

app.MapGet("/api/{date}", (string date) =>
    {
        if (DateTime.TryParse(date, out var myDateTime))
        {
            return Results.Ok(
                new DateTimeResponse(
                    myDateTime.ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'"),
                    DateTimeToUnix(myDateTime))
            );
        }

        if (long.TryParse(date, out var unixTimestamp))
        {
            return Results.Ok(
                new DateTimeResponse(
                    UnixTimeToDateTime(unixTimestamp).ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'"),
                    unixTimestamp)
            );
        }

        return Results.BadRequest(new DateTimeResponseError("Invalid Date"));
    })
    .WithName("GetDate")
    .WithOpenApi();

app.Run();

long DateTimeToUnix(DateTime dt)
{
    TimeSpan timeSpan = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    return (long)timeSpan.TotalMilliseconds;
}

DateTime UnixTimeToDateTime(long unixTime)
{
    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    dtDateTime = dtDateTime.AddMilliseconds(unixTime).ToLocalTime();
    return dtDateTime;
}

record struct DateTimeResponseError(string error);

record struct DateTimeResponse(string utc, long unix);