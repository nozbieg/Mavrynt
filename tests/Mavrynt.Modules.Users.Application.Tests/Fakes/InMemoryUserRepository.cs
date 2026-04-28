using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Tests.Fakes;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id.Value, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var user = _users.Values.FirstOrDefault(x => x.Email == email);
        return Task.FromResult(user);
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.Values.Any(x => x.Email == email));

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id.Value] = user;
        return Task.CompletedTask;
    }

    public void Seed(User user) => _users[user.Id.Value] = user;
    public IReadOnlyCollection<User> Users => _users.Values.ToArray();
}
