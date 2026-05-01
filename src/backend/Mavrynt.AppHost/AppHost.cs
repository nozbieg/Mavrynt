const string apiName = "api";
const string adminApiName = "adminApi";
const string webAppName = "web";
const string adminWebAppName = "adminWeb";
const string landingAppName = "landing";

var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration["DcpPublisher:RandomizePorts"] = "false";

// ── PostgreSQL ────────────────────────────────────────────────────────────────
//
// The database resource name "MavryntDb" matches the ConnectionStrings key used
// in both Mavrynt.Api and Mavrynt.AdminApp appsettings files. Aspire injects
// the resolved connection string as ConnectionStrings__MavryntDb at runtime.
//
// Migrations are applied automatically on startup by DatabaseMigrationService
// (registered in AddUsersInfrastructure). MigrateAsync holds a PostgreSQL
// advisory lock, so running both Api and AdminApp simultaneously is safe.

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("mavrynt-postgres-data")
    .AddDatabase("MavryntDb");

// ── Backend services ──────────────────────────────────────────────────────────

var api = builder.AddProject<Projects.Mavrynt_Api>(apiName)
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithExternalHttpEndpoints();

var adminApi = builder.AddProject<Projects.Mavrynt_AdminApp>(adminApiName)
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithExternalHttpEndpoints();

// ── Frontend SPAs — all three start in parallel ───────────────────────────────
//
// No cross-frontend WaitFor: each SPA is independently startable per
// ADR-010 / ADR-015. The VITE_APP_URL_* injections below are UI-level
// URL hints only — they tell each Vite bundle where sibling SPAs live in
// the browser. They do NOT create runtime service dependencies.
//
// Because RandomizePorts = false the endpoint values are resolved
// statically from each project's launchSettings.json, so the injection
// is free of startup sequencing concerns.

// Landing: no backend reference (ADR-010 / ADR-015).
var landing = builder.AddProject<Projects.Mavrynt_Web_Landing>(landingAppName)
    .WithExternalHttpEndpoints();

// Web app: waits for the API to be healthy before serving.
var webApp = builder.AddProject<Projects.Mavrynt_Web_App>(webAppName)
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

// Admin app: waits for the admin API.
var adminWebApp = builder.AddProject<Projects.Mavrynt_Web_Admin>(adminWebAppName)
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithExternalHttpEndpoints();

// ── Cross-SPA URL injection ───────────────────────────────────────────────────
//
// Injected into each .NET SpaProxy host process; the SpaProxy-launched
// Vite dev server inherits them as a child process (env var inheritance),
// making them available via import.meta.env.VITE_APP_URL_* at runtime.
//
// The base path for each SPA is appended to the endpoint origin so the
// full browser-navigable URL is correct:
//   landing  → /          (base "/")   → origin only
//   web      → /app       (base "/app/")
//   admin    → /admin     (base "/admin/")
//
// Without the base path suffix, cross-SPA links would hit the SpaProxy
// host at a path Vite does not serve (e.g. /login instead of /app/login),
// resulting in a 404 from the Vite dev server.

// Landing needs the web app's full base URL for Login / Register buttons.
landing
    .WithEnvironment("VITE_APP_URL_WEB",
        ReferenceExpression.Create($"{webApp.GetEndpoint("http")}/app"));

// Web app needs the landing URL for the public-site home/back link.
// Landing base is "/", so no path suffix — the origin alone is the entry point.
webApp
    .WithEnvironment("VITE_APP_URL_LANDING",
        landing.GetEndpoint("http"));

builder.Build().Run();
