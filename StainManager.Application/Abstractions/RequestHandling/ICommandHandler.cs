using StainManager.Domain.Abstractions;

namespace StainManager.Application.Abstractions.RequestHandling;

public interface ICommandHandler<TCommand>
    : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for command handlers.
}

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for command handlers.
}