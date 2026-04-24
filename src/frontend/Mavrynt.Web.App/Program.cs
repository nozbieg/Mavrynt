var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Microsoft.AspNetCore.SpaProxy (activated via ASPNETCORE_HOSTINGSTARTUPASSEMBLIES)
    // only intercepts the root path "/" for its redirect to the Vite dev server.
    // Deep-link navigations from sibling SPAs (e.g., landing → /app/register) hit
    // the .NET host directly and bypass SpaProxy's middleware, so we forward them
    // here. Health endpoints are excluded so Aspire probes are unaffected.
    var viteDevUrl = builder.Configuration["SpaProxyServerUrl"]
        ?? throw new InvalidOperationException(
            "SpaProxyServerUrl must be set in appsettings.Development.json");

    app.Use(async (ctx, next) =>
    {
        if (ctx.Request.Method == HttpMethods.Get
            && !ctx.Request.Path.StartsWithSegments("/health")
            && !ctx.Request.Path.StartsWithSegments("/alive"))
        {
            ctx.Response.Redirect(viteDevUrl + ctx.Request.Path + ctx.Request.QueryString);
            return;
        }
        await next(ctx);
    });
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("/index.html");
}

app.Run();
