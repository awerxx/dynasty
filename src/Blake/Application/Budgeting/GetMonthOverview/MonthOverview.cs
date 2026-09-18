using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting.GetMonthOverview;

public sealed record PlannedItemView(
    Guid Id,
    PlannedItemKind Kind,
    string Name,
    decimal PlannedAmount,
    decimal? ActualAmount,
    DateOnly? SettledOn,
    bool IsSettled);

/// <summary>
///     Everything the month page shows: the projection for the month, its items, and the hints the
///     page uses to nudge the user (no balance set yet, an empty month that could be seeded).
/// </summary>
public sealed record MonthOverview(
    MonthSummary Summary,
    IReadOnlyList<PlannedItemView> Items,
    bool HasAccountBalance,
    bool CanCopyFromPreviousMonth);
