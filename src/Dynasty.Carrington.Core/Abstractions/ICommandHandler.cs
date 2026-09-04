namespace Dynasty.Carrington.Core.Abstractions;

/// <summary>
///     Handles a single state-changing use case.
/// </summary>
public interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
///     Handles a single state-changing use case that returns a result, such as the id of a new entity.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}