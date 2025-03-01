using StainManager.Domain.Common;

namespace StainManager.Application.Common.RequestHandling;

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