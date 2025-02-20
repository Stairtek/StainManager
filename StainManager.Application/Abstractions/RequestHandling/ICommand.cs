using StainManager.Domain.Abstractions;

namespace StainManager.Application.Abstractions.RequestHandling;

public interface ICommand
    : IRequest<Result>
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for command handlers.
}

public interface ICommand<TResponse>
    : IRequest<Result<TResponse>>
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for command handlers.
}