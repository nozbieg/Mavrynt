var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Mavrynt.AdminApp");
app.MapGet("/api/ping", () => Results.Ok(new { message = "pong from admin api" }));

app.Run();