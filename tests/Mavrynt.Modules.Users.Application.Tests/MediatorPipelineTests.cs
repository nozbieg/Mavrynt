using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.DependencyInjection;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Application.Persistence;
using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mavrynt.Modules.Users.Application.Tests;

public sealed class MediatorPipelineTests
{
    [Fact]
    public async Task Mediator_Should_Dispatch_Command_Without_Response()
    {
        await using var provider = BuildServices().BuildServiceProvider();
        var result = await provider.GetRequiredService<IMediator>().SendAsync(new VoidCommand());
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Mediator_Should_Dispatch_Command_With_Response_And_Query()
    {
        await using var provider = BuildServices().BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var commandResult = await mediator.SendAsync(new ResponseCommand("ok"));
        var queryResult = await mediator.SendAsync(new ResponseQuery("ok"));

        Assert.Equal("ok", commandResult.Value);
        Assert.Equal("ok", queryResult.Value);
    }

    [Fact]
    public async Task Mediator_Should_Return_Failure_For_Missing_Handler()
    {
        var services = new ServiceCollection();
        services.AddMavryntMediator(typeof(MediatorPipelineTests).Assembly);

        await using var provider = services.BuildServiceProvider();
        var result = await provider.GetRequiredService<IMediator>().SendAsync(new MissingHandlerCommand());

        Assert.True(result.IsFailure);
        Assert.Equal("Mediator.HandlerNotFound", result.Error.Code);
    }

    [Fact]
    public async Task Mediator_Should_Convert_Unhandled_Exception_To_Failure()
    {
        await using var provider = BuildServices().BuildServiceProvider();
        var result = await provider.GetRequiredService<IMediator>().SendAsync(new ThrowingCommand());

        Assert.True(result.IsFailure);
        Assert.Equal("Mediator.UnhandledException", result.Error.Code);
    }

    [Fact]
    public async Task Pipeline_Order_Should_Be_Deterministic()
    {
        OrderTracker.Reset();
        await using var provider = BuildServices().BuildServiceProvider();

        await provider.GetRequiredService<IMediator>().SendAsync(new OrderedCommand());

        Assert.Equal(new[] { "Validation", "Handler" }, OrderTracker.Steps.Take(2).ToArray());
    }

    [Fact]
    public async Task Validation_Should_Stop_Pipeline_And_Transaction_Commit()
    {
        await using var provider = BuildServices(validationFailure: true).BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var unitOfWork = provider.GetRequiredService<FakeUnitOfWork>();

        var result = await mediator.SendAsync(new OrderedCommand());

        Assert.True(result.IsFailure);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Audit_Should_Run_Only_For_Auditable_Requests()
    {
        await using var provider = BuildServices().BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var audit = provider.GetRequiredService<FakeAuditService>();

        await mediator.SendAsync(new OrderedCommand());
        await mediator.SendAsync(new VoidCommand());

        Assert.Single(audit.Entries, x => x.EventType == "ordered_command");
    }

    [Fact]
    public async Task Transaction_Should_Commit_Only_For_Transactional_Success()
    {
        await using var provider = BuildServices().BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var unitOfWork = provider.GetRequiredService<FakeUnitOfWork>();

        await mediator.SendAsync(new OrderedCommand());
        await mediator.SendAsync(new FailedTransactionalCommand());

        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    private static ServiceCollection BuildServices(bool validationFailure = false)
    {
        var services = new ServiceCollection();
        services.AddSingleton<FakeAuditService>();
        services.AddSingleton<IAuditService>(sp => sp.GetRequiredService<FakeAuditService>());
        services.AddSingleton<ICurrentUserContext>(new FakeCurrentUserContext(Guid.NewGuid(), "test@example.com"));
        services.AddSingleton<FakeUnitOfWork>();
        services.AddSingleton<IUnitOfWork>(sp => sp.GetRequiredService<FakeUnitOfWork>());
        services.AddMavryntMediator(typeof(MediatorPipelineTests).Assembly);

        if (validationFailure)
        {
            services.AddTransient<IValidator<OrderedCommand>>(_ => new TestValidator<OrderedCommand>(_ => Result.Failure(new Error("Validation.Failed", "Invalid"))));
        }
        else
        {
            services.AddTransient<IValidator<OrderedCommand>>(_ => new TrackingValidator());
        }

        return services;
    }

    private sealed class TrackingValidator : IValidator<OrderedCommand>
    {
        public Task<Result> ValidateAsync(OrderedCommand request, CancellationToken ct = default)
        {
            OrderTracker.Steps.Add("Validation");
            return Task.FromResult(Result.Success());
        }
    }

    private static class OrderTracker
    {
        public static List<string> Steps { get; } = [];
        public static void Reset() => Steps.Clear();
    }

    private sealed record VoidCommand : ICommand;
    private sealed class VoidCommandHandler : ICommandHandler<VoidCommand>
    {
        public Task<Result> HandleAsync(VoidCommand command, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success());
    }

    private sealed record ResponseCommand(string Value) : ICommand<string>;
    private sealed class ResponseCommandHandler : ICommandHandler<ResponseCommand, string>
    {
        public Task<Result<string>> HandleAsync(ResponseCommand command, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(command.Value));
    }

    private sealed record ResponseQuery(string Value) : IQuery<string>;
    private sealed class ResponseQueryHandler : IQueryHandler<ResponseQuery, string>
    {
        public Task<Result<string>> HandleAsync(ResponseQuery query, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(query.Value));
    }

    private sealed record MissingHandlerCommand : ICommand;

    private sealed record ThrowingCommand : ICommand;
    private sealed class ThrowingCommandHandler : ICommandHandler<ThrowingCommand>
    {
        public Task<Result> HandleAsync(ThrowingCommand command, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("boom");
    }

    private sealed record OrderedCommand : ICommand, IAuditableRequest, ITransactionalRequest
    {
        public string AuditEventType => "ordered_command";
    }

    private sealed class OrderedCommandHandler : ICommandHandler<OrderedCommand>
    {
        public Task<Result> HandleAsync(OrderedCommand command, CancellationToken cancellationToken = default)
        {
            OrderTracker.Steps.Add("Handler");
            return Task.FromResult(Result.Success());
        }
    }

    private sealed record FailedTransactionalCommand : ICommand, ITransactionalRequest;
    private sealed class FailedTransactionalCommandHandler : ICommandHandler<FailedTransactionalCommand>
    {
        public Task<Result> HandleAsync(FailedTransactionalCommand command, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Failure(new Error("Failure", "f")));
    }
}
