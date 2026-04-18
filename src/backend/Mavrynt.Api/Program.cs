var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Mavrynt.Api is running!");
app.MapGet("/api/ping", () => Results.Ok(new { message = "pong from api" }));

app.Run();