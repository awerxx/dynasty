using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.AddPlannedItem;

public sealed class AddPlannedItemCommandHandler(
    IPlannedItemRepository items,
    IUnitOfWork unitOfWork,
    ICurrentUser user)
    : ICommandHandler<AddPlannedItemCommand, Guid>
{
    public async Task<Guid> Handle(AddPlannedItemCommand command, CancellationToken cancellationToken)
    {
        PlannedItem item = new(
            Guid.CreateVersion7(),
            user.UserId,
            command.Month,
            command.Kind,
            command.Name,
            command.PlannedAmount);

        items.Add(item);
        await unitOfWork.SaveChanges(cancellationToken);

        return item.Id;
    }
}
