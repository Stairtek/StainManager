using StainManager.Domain.Common;

namespace StainManager.Application.Common.RequestHandling;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for query handlers.
}