using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace Mavrynt.Api.IntegrationTests;

public sealed class ApiIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().WithDatabase("mavrynt_api_tests").WithUsername("mavrynt").WithPassword("mavrynt").Build();

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
    public async Task Register_Endpoint_Should_Return_Created_For_Valid_Request()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new { Email = $"{Guid.NewGuid():N}@example.com", Password = "Secret1!", DisplayName = "John" });
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_Endpoint_Should_Return_Unauthorized_For_Invalid_Credentials()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { Email = "missing@example.com", Password = "Secret1!" });
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
