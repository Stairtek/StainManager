namespace StainManager.Application.Common.RequestHandling;

public interface IQuery<TResponse>
    : IRequest<Result<TResponse>>
{
    // This interface is intentionally left blank.
    // It serves as a marker interface for queries.
}