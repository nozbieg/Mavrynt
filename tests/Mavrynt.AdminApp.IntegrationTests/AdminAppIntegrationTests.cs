using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Xunit;

namespace Mavrynt.AdminApp.IntegrationTests;

public sealed class AdminAppIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().WithDatabase("mavrynt_admin_tests").WithUsername("mavrynt").WithPassword("mavrynt").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

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
                    ["Jwt:SigningKey"] = "0123456789abcdef0123456789abcdef",
                    ["Jwt:AccessTokenLifetimeMinutes"] = "60"
                });
            });
        });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task App_Should_Start_Successfully()
    {
        var response = await _client.GetAsync("/");
        Assert.NotEqual(System.Net.HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task Health_Endpoint_Should_Return_Success()
    {
        var response = await _client.GetAsync("/health");
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Admin_Me_Endpoint_Should_Return_Unauthorized_Without_Token()
    {
        var response = await _client.GetAsync("/api/admin/me");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
