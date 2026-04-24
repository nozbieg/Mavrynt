var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Mavrynt.Api is running!");
app.MapGet("/api/ping", () => Results.Ok(new { message = "pong from api" }));

app.Run();