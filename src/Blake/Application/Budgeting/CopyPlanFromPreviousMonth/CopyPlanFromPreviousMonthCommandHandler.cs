using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.CopyPlanFromPreviousMonth;

/// <summary>
///     Copies every item of the previous month into the target month as a fresh, unsettled item.
///     The copy is planned at the previous item's best known amount (the real one if it was settled),
///     which is how last month's corrections feed into this month's plan. Items whose kind and name
///     already exist in the target month are skipped. Returns how many items were created.
/// </summary>
public sealed class CopyPlanFromPreviousMonthCommandHandler(
    IPlannedItemRepository items,
    IUnitOfWork unitOfWork,
    ICurrentUser user)
    : ICommandHandler<CopyPlanFromPreviousMonthCommand, int>
{
    public async Task<int> Handle(CopyPlanFromPreviousMonthCommand command, CancellationToken cancellationToken)
    {
        YearMonth target = command.Month;
        YearMonth source = target.Previous();

        IReadOnlyList<PlannedItem> both = await items.GetForMonths(user.UserId, [source, target], cancellationToken);

        HashSet<(PlannedItemKind Kind, string Name)> existing = both
            .Where(item => item.Month == target)
            .Select(Key)
            .ToHashSet();

        int copied = 0;

        foreach (PlannedItem previous in both.Where(item => item.Month == source))
        {
            if (!existing.Add(Key(previous)))
            {
                continue;
            }

            items.Add(new PlannedItem(
                Guid.CreateVersion7(),
                user.UserId,
                target,
                previous.Kind,
                previous.Name,
                previous.EffectiveAmount));

            copied++;
        }

        if (copied > 0)
        {
            await unitOfWork.SaveChanges(cancellationToken);
        }

        return copied;
    }

    private static (PlannedItemKind Kind, string Name) Key(PlannedItem item)
    {
        return (item.Kind, item.Name.ToUpperInvariant());
    }
}
