using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.UpdatePlannedItem;

public sealed class UpdatePlannedItemCommandHandler(
    IPlannedItemRepository items,
    IUnitOfWork unitOfWork,
    ICurrentUser user)
    : ICommandHandler<UpdatePlannedItemCommand>
{
    public async Task Handle(UpdatePlannedItemCommand command, CancellationToken cancellationToken)
    {
        PlannedItem item = await OwnedPlannedItems.GetOwned(items, user, command.Id, cancellationToken);

        item.Update(command.Name, command.Kind, command.PlannedAmount, command.Month);

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
