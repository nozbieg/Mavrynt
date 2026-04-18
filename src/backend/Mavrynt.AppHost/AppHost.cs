const string apiName = "api";
const string adminApiName = "admin-api";
const string webAppName = "web";
const string adminWebAppName = "admin-web";
const string landingAppName = "landing";

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

var landing = builder.AddViteApp(landingAppName, "../../frontend/mavrynt-landing")
    .WithNpmPackageInstallation()
    .WithExternalHttpEndpoints();

builder.Build().Run();