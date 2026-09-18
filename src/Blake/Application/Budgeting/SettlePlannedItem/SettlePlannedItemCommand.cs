namespace Dynasty.Carrington.Blake.Application.Budgeting.SettlePlannedItem;

public sealed record SettlePlannedItemCommand(Guid Id, decimal ActualAmount, DateOnly SettledOn);
