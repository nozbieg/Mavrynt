using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Mavrynt.Modules.Notifications.Infrastructure.DependencyInjection;
using Mavrynt.Modules.Notifications.Infrastructure.Persistence;
using Mavrynt.Modules.Notifications.Infrastructure.Seeding;
using Mavrynt.Modules.Notifications.Infrastructure.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Mavrynt.Modules.Notifications.Infrastructure.Tests;

[Collection(PostgreSqlCollection.Name)]
public sealed class NotificationsInfrastructureTests(PostgreSqlContainerFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // ── DbContext ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task DbContext_Should_Connect_And_Create_Schema()
    {
        await using var context = fixture.CreateDbContext();
        Assert.True(await context.Database.CanConnectAsync());
        Assert.NotNull(context.EmailTemplates);
        Assert.NotNull(context.SmtpSettings);
    }

    // ── EmailTemplate Repository ───────────────────────────────────────────────

    [Fact]
    public async Task EmailTemplate_Should_Persist_And_Retrieve_By_Key()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var template = CreateTemplate(EmailTemplateKey.LoginConfirmation);
        await repository.AddAsync(template);
        await context.SaveChangesAsync();

        var found = await repository.GetByKeyAsync(
            EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value);

        Assert.NotNull(found);
        Assert.Equal(EmailTemplateKey.LoginConfirmation, found!.Key.Value);
        Assert.Equal("Test Template", found.DisplayName);
    }

    [Fact]
    public async Task EmailTemplate_Unique_Key_Should_Be_Enforced()
    {
        await using var context = fixture.CreateDbContext();
        var key = EmailTemplateKey.Create(EmailTemplateKey.PasswordReset).Value;
        context.EmailTemplates.Add(CreateTemplate(EmailTemplateKey.PasswordReset));
        context.EmailTemplates.Add(CreateTemplate(EmailTemplateKey.PasswordReset));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task EmailTemplate_List_Should_Return_All()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        await repository.AddAsync(CreateTemplate(EmailTemplateKey.LoginConfirmation));
        await repository.AddAsync(CreateTemplate(EmailTemplateKey.PasswordReset));
        await context.SaveChangesAsync();

        var all = await repository.ListAsync();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task EmailTemplate_Update_Should_Be_Persisted()
    {
        await using var scope1 = CreateScope();
        var repo1 = scope1.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var ctx1 = scope1.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        await repo1.AddAsync(CreateTemplate(EmailTemplateKey.TwoFactorCode));
        await ctx1.SaveChangesAsync();

        await using var scope2 = CreateScope();
        var repo2 = scope2.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var ctx2 = scope2.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var loaded = await repo2.GetByKeyAsync(EmailTemplateKey.Create(EmailTemplateKey.TwoFactorCode).Value);
        loaded!.UpdateContent("Updated Name", null, null, null, null, false, DateTimeOffset.UtcNow);
        await ctx2.SaveChangesAsync();

        await using var scope3 = CreateScope();
        var repo3 = scope3.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var verified = await repo3.GetByKeyAsync(EmailTemplateKey.Create(EmailTemplateKey.TwoFactorCode).Value);
        Assert.Equal("Updated Name", verified!.DisplayName);
        Assert.False(verified.IsEnabled);
    }

    // ── SmtpSettings Repository ────────────────────────────────────────────────

    [Fact]
    public async Task SmtpSettings_Should_Persist_And_Retrieve()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var settings = CreateSmtp(isEnabled: true);
        await repository.AddAsync(settings);
        await context.SaveChangesAsync();

        var found = await repository.GetByIdAsync(settings.Id);
        Assert.NotNull(found);
        Assert.Equal("Test Provider", found!.ProviderName);
        Assert.True(found.IsEnabled);
    }

    [Fact]
    public async Task SmtpSettings_GetActive_Should_Return_Enabled()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        await repository.AddAsync(CreateSmtp(isEnabled: false));
        var active = CreateSmtp(isEnabled: true);
        await repository.AddAsync(active);
        await context.SaveChangesAsync();

        var found = await repository.GetActiveAsync();
        Assert.NotNull(found);
        Assert.Equal(active.Id.Value, found!.Id.Value);
    }

    [Fact]
    public async Task SmtpSettings_GetActive_Should_Return_Null_When_None_Enabled()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        await repository.AddAsync(CreateSmtp(isEnabled: false));
        await context.SaveChangesAsync();

        var found = await repository.GetActiveAsync();
        Assert.Null(found);
    }

    [Fact]
    public void SmtpSettingsDto_Should_Not_Have_Password_Property()
    {
        var passwordProps = typeof(Mavrynt.Modules.Notifications.Application.DTOs.SmtpSettingsDto)
            .GetProperties()
            .Where(p => p.Name.Contains("Password", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Empty(passwordProps);
    }

    // ── Seeding ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Seeder_Should_Create_All_Default_Templates()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var seeder = new DefaultEmailTemplateSeeder(
            repository, context,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<DefaultEmailTemplateSeeder>.Instance);

        await seeder.SeedAsync();

        var all = await repository.ListAsync();
        Assert.Equal(3, all.Count);
        Assert.Contains(all, t => t.Key.Value == EmailTemplateKey.LoginConfirmation);
        Assert.Contains(all, t => t.Key.Value == EmailTemplateKey.PasswordReset);
        Assert.Contains(all, t => t.Key.Value == EmailTemplateKey.TwoFactorCode);
    }

    [Fact]
    public async Task Seeder_Should_Be_Idempotent()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var seeder = new DefaultEmailTemplateSeeder(
            repository, context,
            NullLogger<DefaultEmailTemplateSeeder>.Instance);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var all = await repository.ListAsync();
        Assert.Equal(3, all.Count);
    }

    [Fact]
    public async Task Seeder_Should_Not_Overwrite_Administrator_Modified_Template()
    {
        await using var scope1 = CreateScope();
        var repo1 = scope1.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var ctx1 = scope1.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var seeder1 = new DefaultEmailTemplateSeeder(repo1, ctx1, NullLogger<DefaultEmailTemplateSeeder>.Instance);
        await seeder1.SeedAsync();

        var loaded = await repo1.GetByKeyAsync(EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value);
        loaded!.UpdateContent("Admin Modified Name", null, null, null, null, null, DateTimeOffset.UtcNow);
        await ctx1.SaveChangesAsync();

        await using var scope2 = CreateScope();
        var repo2 = scope2.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var ctx2 = scope2.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var seeder2 = new DefaultEmailTemplateSeeder(repo2, ctx2, NullLogger<DefaultEmailTemplateSeeder>.Instance);
        await seeder2.SeedAsync();

        await using var scope3 = CreateScope();
        var repo3 = scope3.ServiceProvider.GetRequiredService<IEmailTemplateRepository>();
        var verified = await repo3.GetByKeyAsync(EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value);
        Assert.Equal("Admin Modified Name", verified!.DisplayName);
    }

    // ── Default SMTP Seeder ────────────────────────────────────────────────────

    [Fact]
    public async Task DefaultSmtpSeeder_Should_Create_Default_Configuration_When_None_Exist()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var protector = scope.ServiceProvider.GetRequiredService<ISecretProtector>();
        var seeder = new DefaultSmtpSettingsSeeder(
            repository, context, protector,
            NullLogger<DefaultSmtpSettingsSeeder>.Instance);

        await seeder.SeedAsync();

        var all = await repository.ListAsync();
        Assert.Single(all);
        var seeded = all[0];
        Assert.Equal("Local Dev SMTP", seeded.ProviderName);
        Assert.Equal("localhost", seeded.Host);
        Assert.Equal(1025, seeded.Port);
        Assert.Equal("dev", seeded.Username);
        Assert.Equal("noreply@mavrynt.local", seeded.SenderEmail);
        Assert.Equal("Mavrynt", seeded.SenderName);
        Assert.False(seeded.UseSsl);
        Assert.True(seeded.IsEnabled);
    }

    [Fact]
    public async Task DefaultSmtpSeeder_Should_Be_Idempotent()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var protector = scope.ServiceProvider.GetRequiredService<ISecretProtector>();
        var seeder = new DefaultSmtpSettingsSeeder(
            repository, context, protector,
            NullLogger<DefaultSmtpSettingsSeeder>.Instance);

        await seeder.SeedAsync();
        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var all = await repository.ListAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task DefaultSmtpSeeder_Should_Skip_When_Configuration_Already_Exists()
    {
        await using var scope1 = CreateScope();
        var repository1 = scope1.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context1 = scope1.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var existing = SmtpSettings.Create(
            SmtpSettingsId.New().Value,
            "Existing Provider",
            "smtp.existing.com",
            587,
            "existing-user",
            "existing-protected-password",
            "existing@example.com",
            "Existing",
            useSsl: true,
            isEnabled: false,
            createdAt: DateTimeOffset.UtcNow).Value;
        await repository1.AddAsync(existing);
        await context1.SaveChangesAsync();

        await using var scope2 = CreateScope();
        var repository2 = scope2.ServiceProvider.GetRequiredService<ISmtpSettingsRepository>();
        var context2 = scope2.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var protector2 = scope2.ServiceProvider.GetRequiredService<ISecretProtector>();
        var seeder = new DefaultSmtpSettingsSeeder(
            repository2, context2, protector2,
            NullLogger<DefaultSmtpSettingsSeeder>.Instance);

        await seeder.SeedAsync();

        var all = await repository2.ListAsync();
        Assert.Single(all);
        Assert.Equal("Existing Provider", all[0].ProviderName);
    }

    private static EmailTemplate CreateTemplate(string keyStr) =>
        EmailTemplate.Create(
            EmailTemplateId.New().Value,
            EmailTemplateKey.Create(keyStr).Value,
            "Test Template",
            null,
            "Subject",
            "<p>HTML</p>",
            null, true, DateTimeOffset.UtcNow).Value;

    private static SmtpSettings CreateSmtp(bool isEnabled = false) =>
        SmtpSettings.Create(
            SmtpSettingsId.New().Value,
            "Test Provider",
            "smtp.test.com",
            587,
            "user@test.com",
            "secret",
            "sender@test.com",
            "Test Sender",
            false,
            isEnabled,
            DateTimeOffset.UtcNow).Value;

    private AsyncServiceScope CreateScope()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MavryntDb"] = fixture.ConnectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddNotificationsInfrastructure(config);
        return services.BuildServiceProvider().CreateAsyncScope();
    }
}
