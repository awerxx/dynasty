namespace Dynasty.Carrington.Blake.Application.Abstractions;

/// <summary>
///     The user on whose behalf a use case runs. Every budget entity is scoped to this id.
/// </summary>
public interface ICurrentUser
{
    string UserId { get; }
}
