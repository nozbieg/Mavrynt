using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Audit.Application.DependencyInjection;

public static class AuditApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddAuditApplication(this IServiceCollection services)
    {
        return services;
    }
}
