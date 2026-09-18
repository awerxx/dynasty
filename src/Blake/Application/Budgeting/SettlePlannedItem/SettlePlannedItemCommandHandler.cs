using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.SettlePlannedItem;

public sealed class SettlePlannedItemCommandHandler(
    IPlannedItemRepository items,
    IUnitOfWork unitOfWork,
    ICurrentUser user)
    : ICommandHandler<SettlePlannedItemCommand>
{
    public async Task Handle(SettlePlannedItemCommand command, CancellationToken cancellationToken)
    {
        PlannedItem item = await OwnedPlannedItems.GetOwned(items, user, command.Id, cancellationToken);

        item.Settle(command.ActualAmount, command.SettledOn);

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
