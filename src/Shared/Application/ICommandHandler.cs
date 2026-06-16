using NatrixServices.Shared.Core;

namespace NatrixServices.Shared.Application;

public interface ICommand { }

public interface ICommand<TResult> { }

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    public Task<Result> HandleAsync(TCommand command);
}

public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public Task<Result<TResult>> HandleAsync(TCommand command);
}
