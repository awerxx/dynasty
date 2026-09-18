using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting.CopyPlanFromPreviousMonth;

/// <summary>
///     Seeds <see cref="Month" /> with the plan of the month before it.
/// </summary>
public sealed record CopyPlanFromPreviousMonthCommand(YearMonth Month);
