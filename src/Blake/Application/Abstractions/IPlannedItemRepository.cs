using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Abstractions;

public interface IPlannedItemRepository
{
    Task<PlannedItem?> Find(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     All items of <paramref name="ownerId" /> that fall in any of <paramref name="months" />.
    /// </summary>
    Task<IReadOnlyList<PlannedItem>> GetForMonths(
        string ownerId,
        IReadOnlyCollection<YearMonth> months,
        CancellationToken cancellationToken);

    void Add(PlannedItem item);

    void Remove(PlannedItem item);
}
