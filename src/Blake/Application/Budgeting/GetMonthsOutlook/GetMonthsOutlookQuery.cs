using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting.GetMonthsOutlook;

/// <summary>
///     The projection for <see cref="Count" /> consecutive months starting at <see cref="From" />.
/// </summary>
public sealed record GetMonthsOutlookQuery(YearMonth From, int Count);
