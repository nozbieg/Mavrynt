namespace Mavrynt.Modules.Users.Application.Queries;

public static class UsersCacheKeys
{
    public static string UserById(Guid id) => $"users:user:id:{id:N}";
    public static string UserByEmail(string email) => $"users:user:email:{email.Trim().ToLowerInvariant()}";
    public const string UsersList = "users:list:all";
}
