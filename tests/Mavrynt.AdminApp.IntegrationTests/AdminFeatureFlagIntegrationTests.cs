using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Mavrynt.AdminApp.IntegrationTests.Helpers;
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

public sealed class AdminFeatureFlagIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("mavrynt_ff_tests")
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

    // ── Authorization checks ───────────────────────────────────────────────────

    [Fact]
    public async Task List_Without_Token_Returns_401()
    {
        var response = await _anonClient.GetAsync("/api/admin/feature-flags");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task List_With_User_Token_Returns_403()
    {
        var response = await _userClient.GetAsync("/api/admin/feature-flags");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_Without_Token_Returns_401()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "test.flag", name = "Test", isEnabled = true });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_With_User_Token_Returns_403()
    {
        var response = await _userClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "test.flag", name = "Test", isEnabled = true });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ── Feature flag CRUD ──────────────────────────────────────────────────────

    [Fact]
    public async Task Admin_Can_Create_Feature_Flag()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/admin/feature-flags", new
        {
            key = "create.test-flag",
            name = "Create Test Flag",
            description = "Test description",
            isEnabled = true
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("create.test-flag", body.GetProperty("key").GetString());
        Assert.Equal("Create Test Flag", body.GetProperty("name").GetString());
        Assert.True(body.GetProperty("isEnabled").GetBoolean());
    }

    [Fact]
    public async Task Admin_Can_List_Feature_Flags()
    {
        await _adminClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "list.flag-a", name = "Flag A", isEnabled = true });
        await _adminClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "list.flag-b", name = "Flag B", isEnabled = false });

        var response = await _adminClient.GetAsync("/api/admin/feature-flags");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetArrayLength() >= 2);
    }

    [Fact]
    public async Task Admin_Can_Get_Feature_Flag_By_Key()
    {
        await _adminClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "get.by-key-flag", name = "Get By Key", isEnabled = true });

        var response = await _adminClient.GetAsync("/api/admin/feature-flags/get.by-key-flag");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("get.by-key-flag", body.GetProperty("key").GetString());
    }

    [Fact]
    public async Task Get_By_Key_Returns_404_When_Not_Found()
    {
        var response = await _adminClient.GetAsync("/api/admin/feature-flags/not.existing-flag");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Can_Update_Feature_Flag()
    {
        await _adminClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "update.test-flag", name = "Original Name", isEnabled = true });

        var response = await _adminClient.PatchAsJsonAsync(
            "/api/admin/feature-flags/update.test-flag",
            new { name = "Updated Name", description = "Updated desc" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Updated Name", body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Admin_Can_Toggle_Feature_Flag()
    {
        await _adminClient.PostAsJsonAsync("/api/admin/feature-flags",
            new { key = "toggle.test-flag", name = "Toggle Flag", isEnabled = true });

        var response = await _adminClient.PatchAsJsonAsync(
            "/api/admin/feature-flags/toggle.test-flag/toggle",
            new { });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(body.GetProperty("isEnabled").GetBoolean());
    }

    [Fact]
    public async Task Create_Returns_409_For_Duplicate_Key()
    {
        var payload = new { key = "dup.flag", name = "Dup Flag", isEnabled = true };
        await _adminClient.PostAsJsonAsync("/api/admin/feature-flags", payload);

        var response = await _adminClient.PostAsJsonAsync("/api/admin/feature-flags", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Update_Returns_404_For_Missing_Flag()
    {
        var response = await _adminClient.PatchAsJsonAsync(
            "/api/admin/feature-flags/not.there",
            new { name = "X", description = (string?)null });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
