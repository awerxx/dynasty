using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting;

internal static class OwnedPlannedItems
{
    /// <summary>
    ///     Loads an item the current user owns, or throws <see cref="PlannedItemNotFoundException" />.
    /// </summary>
    public static async Task<PlannedItem> GetOwned(
        IPlannedItemRepository items,
        ICurrentUser user,
        Guid id,
        CancellationToken cancellationToken)
    {
        PlannedItem? item = await items.Find(id, cancellationToken);

        if (item is null || !item.IsOwnedBy(user.UserId))
        {
            throw new PlannedItemNotFoundException(id);
        }

        return item;
    }
}
