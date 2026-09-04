namespace Dynasty.Carrington.Core.Abstractions;

/// <summary>
///     Handles a single read-only use case. Implementations must not mutate state.
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}