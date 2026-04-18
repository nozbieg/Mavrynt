var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Mavrynt.AdminApp");
app.MapGet("/api/ping", () => Results.Ok(new { message = "pong from admin api" }));
app.Run();