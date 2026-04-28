using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Persistence;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Abstractions;

namespace Mavrynt.Modules.Users.Application.Tests.Fakes;

internal sealed class FixedDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => $"hashed::{password}";

    public bool VerifyPassword(string password, string passwordHash)
        => HashPassword(password) == passwordHash;
}

internal sealed class FakeAuditService : IAuditService
{
    public List<AuditEntry> Entries { get; } = [];

    public Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}

internal sealed class FakeJwtTokenService : IJwtTokenService
{
    public AccessTokenResult GenerateToken(Guid userId, string email, string? displayName, string role)
        => new("placeholder-token", DateTimeOffset.UtcNow.AddMinutes(30));
}

internal sealed class FakeCurrentUserContext(Guid? userId = null, string? email = null) : ICurrentUserContext
{
    public Guid? UserId { get; } = userId;
    public string? Email { get; } = email;
    public bool IsAuthenticated => UserId.HasValue;
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;
        return Task.FromResult(1);
    }
}

internal sealed class TestValidator<TRequest>(Func<TRequest, Result> validation) : Mavrynt.BuildingBlocks.Application.Validation.IValidator<TRequest>
{
    public Task<Result> ValidateAsync(TRequest request, CancellationToken ct = default)
        => Task.FromResult(validation(request));
}
