using System.Reflection;
using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Mediator;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.DependencyInjection;

public static class MediatorServiceCollectionExtensions
{
    private static readonly Type CommandHandlerOpenType = typeof(ICommandHandler<>);
    private static readonly Type CommandHandlerWithResponseOpenType = typeof(ICommandHandler<,>);
    private static readonly Type QueryHandlerOpenType = typeof(IQueryHandler<,>);
    private static readonly Type ValidatorOpenType = typeof(IValidator<>);

    /// <summary>
    /// Registers the Mavrynt internal mediator and scans the provided assemblies for:
    ///   - <see cref="ICommandHandler{TCommand}"/> implementations
    ///   - <see cref="ICommandHandler{TCommand,TResponse}"/> implementations
    ///   - <see cref="IQueryHandler{TQuery,TResponse}"/> implementations
    ///   - <see cref="IValidator{TRequest}"/> implementations
    ///
    /// Pipeline behaviors are registered in execution order:
    ///   Logging → Validation → Resilience → Audit → Transaction
    ///
    /// Do not add MediatR. This is Mavrynt's own mediator (see ADR-020).
    /// </summary>
    public static IServiceCollection AddMavryntMediator(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        // ── Mediator ─────────────────────────────────────────────────────────
        services.AddScoped<IMediator, MavryntMediator>();

        // ── Pipeline behaviors (order = registration order) ───────────────────
        // 1. Logging — outermost: measures total elapsed time including all inner behaviors
        services.AddTransient(typeof(IMavryntBehavior<,>), typeof(LoggingBehavior<,>));

        // 2. Validation — stops pipeline early on invalid input
        services.AddTransient(typeof(IMavryntBehavior<,>), typeof(ValidationBehavior<,>));

        // 3. Resilience — retry/timeout hook (no-op until Polly is introduced)
        services.AddTransient(typeof(IMavryntBehavior<,>), typeof(ResilienceBehavior<,>));

        // 4. Audit — records after handler succeeds or fails
        services.AddTransient(typeof(IMavryntBehavior<,>), typeof(AuditBehavior<,>));

        // 5. Transaction — commits unit-of-work on success (innermost, closest to handler)
        services.AddTransient(typeof(IMavryntBehavior<,>), typeof(TransactionBehavior<,>));

        // ── Handlers and validators from provided assemblies ──────────────────
        foreach (var assembly in assemblies)
            RegisterFromAssembly(services, assembly);

        return services;
    }

    private static void RegisterFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var concreteTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false });

        foreach (var type in concreteTypes)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType)
                    continue;

                var definition = iface.GetGenericTypeDefinition();

                if (definition == CommandHandlerOpenType ||
                    definition == CommandHandlerWithResponseOpenType ||
                    definition == QueryHandlerOpenType ||
                    definition == ValidatorOpenType)
                {
                    services.AddTransient(iface, type);
                }
            }
        }
    }
}
