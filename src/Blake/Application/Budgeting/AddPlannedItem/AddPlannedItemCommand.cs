using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting.AddPlannedItem;

public sealed record AddPlannedItemCommand(
    YearMonth Month,
    PlannedItemKind Kind,
    string Name,
    decimal PlannedAmount);
