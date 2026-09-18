using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

internal sealed class InMemoryPlannedItemRepository(params IEnumerable<PlannedItem> seed) : IPlannedItemRepository
{
    public List<PlannedItem> Items { get; } = seed.ToList();

    public Task<PlannedItem?> Find(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.SingleOrDefault(item => item.Id == id));
    }

    public Task<IReadOnlyList<PlannedItem>> GetForMonths(
        string ownerId,
        IReadOnlyCollection<YearMonth> months,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<PlannedItem> found = Items
            .Where(item => item.OwnerId == ownerId && months.Contains(item.Month))
            .ToList();

        return Task.FromResult(found);
    }

    public void Add(PlannedItem item)
    {
        Items.Add(item);
    }

    public void Remove(PlannedItem item)
    {
        Items.Remove(item);
    }
}
