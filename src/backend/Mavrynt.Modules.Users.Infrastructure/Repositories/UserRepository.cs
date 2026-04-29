using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Mavrynt.Modules.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.Users.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        // TODO: migrate to explicit IUnitOfWork.SaveChangesAsync when multi-aggregate
        //       transactions are required. For now each add is its own transaction.
        await _context.SaveChangesAsync(cancellationToken);
    }
}
