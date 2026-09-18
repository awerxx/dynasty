using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

using Microsoft.EntityFrameworkCore;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Repositories;

internal sealed class PlannedItemRepository(BlakeDbContext database) : IPlannedItemRepository
{
    public async Task<PlannedItem?> Find(Guid id, CancellationToken cancellationToken)
    {
        return await database.PlannedItems.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<PlannedItem>> GetForMonths(
        string ownerId,
        IReadOnlyCollection<YearMonth> months,
        CancellationToken cancellationToken)
    {
        // Only equality is used on Month so the query translates on every provider, including
        // relational ones where user-defined comparison operators on a converted type would not.
        YearMonth[] wanted = months.ToArray();

        return await database.PlannedItems
            .Where(item => item.OwnerId == ownerId && wanted.Contains(item.Month))
            .ToListAsync(cancellationToken);
    }

    public void Add(PlannedItem item)
    {
        database.PlannedItems.Add(item);
    }

    public void Remove(PlannedItem item)
    {
        database.PlannedItems.Remove(item);
    }
}
