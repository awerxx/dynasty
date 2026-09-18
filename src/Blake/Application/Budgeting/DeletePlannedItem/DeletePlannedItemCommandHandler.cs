using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.DeletePlannedItem;

public sealed class DeletePlannedItemCommandHandler(
    IPlannedItemRepository items,
    IUnitOfWork unitOfWork,
    ICurrentUser user)
    : ICommandHandler<DeletePlannedItemCommand>
{
    public async Task Handle(DeletePlannedItemCommand command, CancellationToken cancellationToken)
    {
        PlannedItem item = await OwnedPlannedItems.GetOwned(items, user, command.Id, cancellationToken);

        items.Remove(item);

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
