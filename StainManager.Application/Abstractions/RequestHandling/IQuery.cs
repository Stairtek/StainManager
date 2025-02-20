using StainManager.Domain.Abstractions;

namespace StainManager.Application.Abstractions.RequestHandling;

public interface IQuery<TResponse>
    : IRequest<Result<TResponse>>
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for queries.
}