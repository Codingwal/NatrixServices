using NatrixServices.Shared.Core;

namespace NatrixServices.Shared.Application;

public interface ICommandDispatcher
{
    public Task<Result> ExecuteCommandAsync<TCommand>(TCommand command)
        where TCommand : ICommand;

    public Task<Result<TResult>> ExecuteCommandAsync<TCommand, TResult>(TCommand command)
        where TCommand : ICommand<TResult>;
}

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task<Result> ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        if (command == null)
            throw new ArgumentException("Argument is null", nameof(command));

        var handler = serviceProvider.GetService<ICommandHandler<TCommand>>()
            ?? throw new NotImplementedException($"Handler for command of type {typeof(TCommand)} is missing.");

        return await handler.HandleAsync(command);
    }

    public async Task<Result<TResult>> ExecuteCommandAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        if (command == null)
            throw new ArgumentException("Argument is null", nameof(command));

        var handler = serviceProvider.GetService<ICommandHandler<TCommand, TResult>>()
            ?? throw new NotImplementedException($"Handler for command of type {typeof(TCommand)} with result type {typeof(TResult)} is missing.");

        return await handler.HandleAsync(command);
    }
}