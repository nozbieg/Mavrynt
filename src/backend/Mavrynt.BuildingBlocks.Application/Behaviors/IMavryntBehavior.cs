namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Core pipeline behavior interface. All cross-cutting concerns
/// (logging, validation, audit, resilience, transaction) implement this contract.
///
/// Behaviors are resolved via DI as open generics and executed in registration order.
/// The mediator builds the pipeline: Logging → Validation → Resilience → Audit → Transaction → Handler.
///
/// To create a custom behavior:
///   1. Implement <see cref="IMavryntBehavior{TRequest,TResponse}"/>.
///   2. Register it in DI via <c>services.AddTransient(typeof(IMavryntBehavior&lt;,&gt;), typeof(YourBehavior&lt;,&gt;))</c>.
///   3. Control order by registration position in the DI container.
/// </summary>
public interface IMavryntBehavior<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken);
}
