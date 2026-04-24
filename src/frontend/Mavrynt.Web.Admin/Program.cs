var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Same deep-link forwarding as Mavrynt.Web.App — SpaProxy only handles "/".
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
