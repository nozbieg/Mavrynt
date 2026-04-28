using Mavrynt.Api.DependencyInjection;
using Mavrynt.Api.Endpoints;
using Mavrynt.Modules.Users.Application.DependencyInjection;
using Mavrynt.Modules.Users.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ── Aspire service defaults (OTel, health checks, service discovery) ─────────
builder.AddServiceDefaults();

// ── Users module ──────────────────────────────────────────────────────────────
builder.Services.AddUsersApplication();
builder.Services.AddUsersInfrastructure(builder.Configuration);

// ── Authentication + authorisation ───────────────────────────────────────────
builder.Services.AddApiAuthentication(builder.Configuration);

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Aspire health / alive endpoints (development only — see ServiceDefaults).
app.MapDefaultEndpoints();

// Module endpoints
app.MapAuthEndpoints();

app.Run();