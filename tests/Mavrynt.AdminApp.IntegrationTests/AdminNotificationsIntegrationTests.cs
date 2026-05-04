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

public sealed class AdminNotificationsIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("mavrynt_notifications_tests")
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

    // ── Authorization – SMTP settings ─────────────────────────────────────────

    [Fact]
    public async Task SmtpSettings_List_Without_Token_Returns_401()
    {
        var response = await _anonClient.GetAsync("/api/admin/notifications/smtp-settings/");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SmtpSettings_List_With_User_Token_Returns_403()
    {
        var response = await _userClient.GetAsync("/api/admin/notifications/smtp-settings/");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SmtpSettings_Create_Without_Token_Returns_401()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/admin/notifications/smtp-settings/",
            new { providerName = "X", host = "smtp.x.com", port = 587, username = "u", password = "p",
                  senderEmail = "no-reply@x.com", senderName = "X", useSsl = true, isEnabled = false });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── SMTP settings CRUD ─────────────────────────────────────────────────────

    [Fact]
    public async Task Admin_Can_Create_And_List_SmtpSettings()
    {
        var createResponse = await _adminClient.PostAsJsonAsync("/api/admin/notifications/smtp-settings/", new
        {
            providerName = "SendGrid",
            host = "smtp.sendgrid.net",
            port = 587,
            username = "apikey",
            password = "SG.secret",
            senderEmail = "no-reply@example.com",
            senderName = "Example App",
            useSsl = true,
            isEnabled = false
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("SendGrid", created.GetProperty("providerName").GetString());
        Assert.False(created.TryGetProperty("password", out _), "Response must not include password");

        var listResponse = await _adminClient.GetAsync("/api/admin/notifications/smtp-settings/");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(list.GetArrayLength() >= 1);
    }

    [Fact]
    public async Task Admin_Can_Get_SmtpSettings_By_Id()
    {
        var createResponse = await _adminClient.PostAsJsonAsync("/api/admin/notifications/smtp-settings/", new
        {
            providerName = "Mailgun",
            host = "smtp.mailgun.org",
            port = 465,
            username = "postmaster@mg.example.com",
            password = "secret",
            senderEmail = "noreply@mg.example.com",
            senderName = "Mailgun App",
            useSsl = true,
            isEnabled = false
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var getResponse = await _adminClient.GetAsync($"/api/admin/notifications/smtp-settings/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var body = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Mailgun", body.GetProperty("providerName").GetString());
    }

    [Fact]
    public async Task GetById_Returns_404_For_Unknown_Id()
    {
        var response = await _adminClient.GetAsync($"/api/admin/notifications/smtp-settings/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Can_Update_SmtpSettings()
    {
        var createResponse = await _adminClient.PostAsJsonAsync("/api/admin/notifications/smtp-settings/", new
        {
            providerName = "OldProvider",
            host = "smtp.old.com",
            port = 25,
            username = "user",
            password = "pass",
            senderEmail = "old@old.com",
            senderName = "Old Sender",
            useSsl = false,
            isEnabled = false
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var updateResponse = await _adminClient.PatchAsJsonAsync(
            $"/api/admin/notifications/smtp-settings/{id}", new
            {
                providerName = "NewProvider",
                host = "smtp.new.com",
                port = 587,
                username = "newuser",
                password = (string?)null,
                senderEmail = "new@new.com",
                senderName = "New Sender",
                useSsl = true
            });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("NewProvider", updated.GetProperty("providerName").GetString());

        // Persistence regression: re-fetch and confirm the update was committed to the database.
        var refetchResponse = await _adminClient.GetAsync($"/api/admin/notifications/smtp-settings/{id}");
        var refetched = await refetchResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("NewProvider", refetched.GetProperty("providerName").GetString());
        Assert.Equal("smtp.new.com", refetched.GetProperty("host").GetString());
        Assert.Equal(587, refetched.GetProperty("port").GetInt32());
        Assert.Equal("newuser", refetched.GetProperty("username").GetString());
        Assert.True(refetched.GetProperty("useSsl").GetBoolean());
    }

    [Fact]
    public async Task SendTest_Without_Token_Returns_401()
    {
        var response = await _anonClient.PostAsJsonAsync(
            $"/api/admin/notifications/smtp-settings/{Guid.NewGuid()}/test",
            new { recipientEmail = "admin@example.com" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SendTest_With_User_Token_Returns_403()
    {
        var response = await _userClient.PostAsJsonAsync(
            $"/api/admin/notifications/smtp-settings/{Guid.NewGuid()}/test",
            new { recipientEmail = "admin@example.com" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SendTest_Returns_404_For_Unknown_Id()
    {
        var response = await _adminClient.PostAsJsonAsync(
            $"/api/admin/notifications/smtp-settings/{Guid.NewGuid()}/test",
            new { recipientEmail = "admin@example.com" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SendTest_Returns_400_For_Invalid_Recipient()
    {
        var createResponse = await _adminClient.PostAsJsonAsync("/api/admin/notifications/smtp-settings/", new
        {
            providerName = "TestRecipientProvider",
            host = "smtp.test.com",
            port = 587,
            username = "user",
            password = "pass",
            senderEmail = "test@test.com",
            senderName = "Test",
            useSsl = true,
            isEnabled = false
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var response = await _adminClient.PostAsJsonAsync(
            $"/api/admin/notifications/smtp-settings/{id}/test",
            new { recipientEmail = "not-an-email" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Can_Enable_SmtpSettings()
    {
        var createResponse = await _adminClient.PostAsJsonAsync("/api/admin/notifications/smtp-settings/", new
        {
            providerName = "EnableTest",
            host = "smtp.test.com",
            port = 587,
            username = "user",
            password = "pass",
            senderEmail = "test@test.com",
            senderName = "Test",
            useSsl = true,
            isEnabled = false
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var enableResponse = await _adminClient.PatchAsync(
            $"/api/admin/notifications/smtp-settings/{id}/enable",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, enableResponse.StatusCode);
        var enabled = await enableResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(enabled.GetProperty("isEnabled").GetBoolean());

        // Persistence regression: re-fetch and confirm the enable persisted.
        var refetchResponse = await _adminClient.GetAsync($"/api/admin/notifications/smtp-settings/{id}");
        var refetched = await refetchResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(refetched.GetProperty("isEnabled").GetBoolean());
    }

    // ── Authorization – email templates ───────────────────────────────────────

    [Fact]
    public async Task EmailTemplates_List_Without_Token_Returns_401()
    {
        var response = await _anonClient.GetAsync("/api/admin/notifications/email/templates");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task EmailTemplates_List_With_User_Token_Returns_403()
    {
        var response = await _userClient.GetAsync("/api/admin/notifications/email/templates");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ── Email templates ────────────────────────────────────────────────────────

    [Fact]
    public async Task Admin_Can_List_Email_Templates_Seeded_On_Startup()
    {
        var response = await _adminClient.GetAsync("/api/admin/notifications/email/templates");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, list.GetArrayLength());
    }

    [Fact]
    public async Task Admin_Can_Get_Template_By_Key()
    {
        var response = await _adminClient.GetAsync(
            "/api/admin/notifications/email/templates/auth.login_confirmation");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("auth.login_confirmation", body.GetProperty("templateKey").GetString());
    }

    [Fact]
    public async Task GetTemplate_Returns_400_For_Unknown_Key()
    {
        var response = await _adminClient.GetAsync(
            "/api/admin/notifications/email/templates/unknown.key");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_Can_Update_Template_Content()
    {
        var updateResponse = await _adminClient.PatchAsJsonAsync(
            "/api/admin/notifications/email/templates/auth.password_reset", new
            {
                subjectTemplate = "Reset your password",
                htmlBodyTemplate = "<p>Hi {{DisplayName}}, click <a href='{{ResetLink}}'>here</a>. Expires: {{ExpiresAt}}</p>",
                textBodyTemplate = "Hi {{DisplayName}}, reset link: {{ResetLink}}. Expires: {{ExpiresAt}}"
            });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var body = await updateResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Reset your password", body.GetProperty("subjectTemplate").GetString());

        // Persistence regression: re-fetch and confirm the update was committed.
        var refetchResponse = await _adminClient.GetAsync(
            "/api/admin/notifications/email/templates/auth.password_reset");
        var refetched = await refetchResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Reset your password", refetched.GetProperty("subjectTemplate").GetString());
    }

    [Fact]
    public async Task Admin_Can_List_Template_Definitions()
    {
        var response = await _adminClient.GetAsync("/api/admin/notifications/email/template-definitions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, list.GetArrayLength());
    }
}
