using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.UnsettlePlannedItem;

public sealed class UnsettlePlannedItemCommandHandler(
    IPlannedItemRepository items,
    IUnitOfWork unitOfWork,
    ICurrentUser user)
    : ICommandHandler<UnsettlePlannedItemCommand>
{
    public async Task Handle(UnsettlePlannedItemCommand command, CancellationToken cancellationToken)
    {
        PlannedItem item = await OwnedPlannedItems.GetOwned(items, user, command.Id, cancellationToken);

        item.Unsettle();

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
