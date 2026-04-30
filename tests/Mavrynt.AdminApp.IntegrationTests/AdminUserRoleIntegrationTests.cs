using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Mavrynt.AdminApp.IntegrationTests.Helpers;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Mavrynt.Modules.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Testcontainers.PostgreSql;
using Xunit;

namespace Mavrynt.AdminApp.IntegrationTests;

public sealed class AdminUserRoleIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("mavrynt_role_tests")
        .WithUsername("mavrynt")
        .WithPassword("mavrynt")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _adminClient = null!;
    private HttpClient _userClient = null!;
    private HttpClient _anonClient = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MavryntDb"] = _postgres.GetConnectionString(),
                    ["Jwt:Issuer"] = "mavrynt-tests",
                    ["Jwt:Audience"] = "mavrynt-tests",
                    ["Jwt:Secret"] = JwtTestTokenHelper.TestSecret,
                    ["Jwt:ExpirationMinutes"] = "60"
                });
            });
            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "mavrynt-tests",
                        ValidateAudience = true,
                        ValidAudience = "mavrynt-tests",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(JwtTestTokenHelper.TestSecret)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = "sub",
                        RoleClaimType = "role",
                    };
                });
            });
        });

        _adminClient = _factory.CreateClient();
        _adminClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestTokenHelper.GenerateAdminToken());

        _userClient = _factory.CreateClient();
        _userClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestTokenHelper.GenerateUserToken());

        _anonClient = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _adminClient.Dispose();
        _userClient.Dispose();
        _anonClient.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    // ── Authorization ──────────────────────────────────────────────────────────

    [Fact]
    public async Task AssignRole_Without_Token_Returns_401()
    {
        var response = await _anonClient.PatchAsJsonAsync(
            $"/api/admin/users/{Guid.NewGuid()}/role",
            new { role = "Admin" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AssignRole_With_User_Token_Returns_403()
    {
        var response = await _userClient.PatchAsJsonAsync(
            $"/api/admin/users/{Guid.NewGuid()}/role",
            new { role = "Admin" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ── Role assignment ────────────────────────────────────────────────────────

    [Fact]
    public async Task AssignRole_Returns_404_For_Missing_User()
    {
        var response = await _adminClient.PatchAsJsonAsync(
            $"/api/admin/users/{Guid.NewGuid()}/role",
            new { role = "Admin" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AssignRole_Returns_400_For_Invalid_Role()
    {
        var userId = await SeedUserAsync("roletest@example.com");

        var response = await _adminClient.PatchAsJsonAsync(
            $"/api/admin/users/{userId}/role",
            new { role = "SuperAdmin" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Can_Assign_Role_To_User()
    {
        var userId = await SeedUserAsync("assignrole@example.com");

        var response = await _adminClient.PatchAsJsonAsync(
            $"/api/admin/users/{userId}/role",
            new { role = "Admin" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Admin", body.GetProperty("role").GetString());
    }

    private async Task<Guid> SeedUserAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        var id = UserId.From(Guid.NewGuid()).Value;
        var user = User.Register(
            id,
            Email.Create(email).Value,
            PasswordHash.Create("$2a$12$placeholder.hash.for.tests.only").Value,
            null,
            DateTimeOffset.UtcNow);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return id.Value;
    }
}
