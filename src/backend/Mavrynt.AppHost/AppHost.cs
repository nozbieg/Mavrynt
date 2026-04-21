const string apiName = "api";
const string adminApiName = "adminApi";
const string webAppName = "web";
const string adminWebAppName = "adminWeb";
const string landingAppName = "landing";
const string yarpProxyName = "yarp-proxy";

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Mavrynt_Api>(apiName)
    .WithExternalHttpEndpoints();
var adminApi = builder.AddProject<Projects.Mavrynt_AdminApp>(adminApiName)
    .WithExternalHttpEndpoints();

var web = builder.AddViteApp(webAppName, "../../frontend/mavrynt-web")
    .WithNpmPackageInstallation()
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

var adminWeb = builder.AddViteApp(adminWebAppName, "../../frontend/mavrynt-admin")
    .WithNpmPackageInstallation()
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithExternalHttpEndpoints();

// The marketing landing SPA is intentionally NOT wired to `api` via
// `.WithReference(api)` / `.WaitFor(api)` — per ADR-010 and ADR-015 the
// landing must stay deployable and runnable independently of the
// backend. Any future integration (e.g. contact-form submission) goes
// through the in-app LeadService port + HTTP adapter, not a runtime
// AppHost reference.
var landing = builder.AddViteApp(landingAppName, "../../frontend/mavrynt-landing")
    .WithNpmPackageInstallation()
    .WithExternalHttpEndpoints();

// Add YARP Reverse Proxy to route all app URLs through a unified entry point
var yarpProxy = builder.AddProject<Projects.Mavrynt_YarpProxy>(yarpProxyName)
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WithReference(adminApi)
    .WithReference(web)
    .WithReference(adminWeb)
    .WithReference(landing)
    .WaitFor(api)
    .WaitFor(adminApi)
    .WaitFor(web)
    .WaitFor(adminWeb)
    .WaitFor(landing);

builder.Build().Run();
