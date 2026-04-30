using Mavrynt.AdminApp.DependencyInjection;
using Mavrynt.AdminApp.Endpoints;
using Mavrynt.Modules.Audit.Application.DependencyInjection;
using Mavrynt.Modules.Audit.Infrastructure.DependencyInjection;
using Mavrynt.Modules.FeatureManagement.Application.DependencyInjection;
using Mavrynt.Modules.FeatureManagement.Infrastructure.DependencyInjection;
using Mavrynt.Modules.Notifications.Application.DependencyInjection;
using Mavrynt.Modules.Notifications.Infrastructure.DependencyInjection;
using Mavrynt.Modules.Users.Application.DependencyInjection;
using Mavrynt.Modules.Users.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ── Aspire service defaults (OTel, health checks, service discovery) ─────────
builder.AddServiceDefaults();

// ── Users module ──────────────────────────────────────────────────────────────
builder.Services.AddUsersApplication();
builder.Services.AddUsersInfrastructure(builder.Configuration);

// ── Audit module ──────────────────────────────────────────────────────────────
builder.Services.AddAuditApplication();
builder.Services.AddAuditInfrastructure(builder.Configuration);

// ── FeatureManagement module ──────────────────────────────────────────────────
builder.Services.AddFeatureManagementApplication();
builder.Services.AddFeatureManagementInfrastructure(builder.Configuration);

// ── Notifications module ──────────────────────────────────────────────────────
builder.Services.AddNotificationsApplication();
builder.Services.AddNotificationsInfrastructure(builder.Configuration);

// ── Authentication + authorisation ───────────────────────────────────────────
builder.Services.AddApiAuthentication(builder.Configuration);

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Aspire health / alive endpoints (development only — see ServiceDefaults).
app.MapDefaultEndpoints();

// Module endpoints
app.MapAdminEndpoints();

app.Run();

public partial class Program;
