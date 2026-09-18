namespace Dynasty.Carrington.Blake.Application.Budgeting;

/// <summary>
///     Thrown when an item does not exist or belongs to someone else. The two cases are deliberately
///     indistinguishable so that ids of other users' items cannot be probed.
/// </summary>
public sealed class PlannedItemNotFoundException(Guid id)
    : Exception($"Planned item {id} was not found.")
{
    public Guid Id { get; } = id;
}
