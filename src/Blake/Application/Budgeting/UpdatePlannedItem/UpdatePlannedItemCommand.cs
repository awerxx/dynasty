using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting.UpdatePlannedItem;

public sealed record UpdatePlannedItemCommand(
    Guid Id,
    YearMonth Month,
    PlannedItemKind Kind,
    string Name,
    decimal PlannedAmount);
