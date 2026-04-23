const string apiName = "api";
const string adminApiName = "adminApi";
const string webAppName = "web";
const string adminWebAppName = "adminWeb";
const string landingAppName = "landing";



var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration["DcpPublisher:RandomizePorts"] = "false";

var api = builder.AddProject<Projects.Mavrynt_Api>(apiName)
    .WithExternalHttpEndpoints();

var adminApi = builder.AddProject<Projects.Mavrynt_AdminApp>(adminApiName)
    .WithExternalHttpEndpoints();

_ = builder.AddProject<Projects.Mavrynt_Web_App>(webAppName)
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

_ = builder.AddProject<Projects.Mavrynt_Web_Admin>(adminWebAppName)
    .WithReference(adminApi)
    .WaitFor(adminApi)
    .WithExternalHttpEndpoints();

// The marketing landing SPA is intentionally NOT wired to `api` via
// `.WithReference(api)` / `.WaitFor(api)` — per ADR-010 and ADR-015 the
// landing must stay deployable and runnable independently of the
// backend. Any future integration (e.g. contact-form submission) goes
// through the in-app LeadService port + HTTP adapter, not a runtime
// AppHost reference.
_ = builder.AddProject<Projects.Mavrynt_Web_Landing>(landingAppName)
    .WithExternalHttpEndpoints();

builder.Build().Run();
