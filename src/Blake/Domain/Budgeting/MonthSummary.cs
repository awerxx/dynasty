namespace Dynasty.Carrington.Blake.Domain.Budgeting;

/// <summary>
///     How a month looks: what is still expected to come in and go out, and where the balance
///     lands. Past months carry history only, so their balances are <c>null</c>.
/// </summary>
public sealed record MonthSummary(
    YearMonth Month,
    bool IsPast,
    bool IsCurrent,
    decimal? OpeningBalance,
    decimal RemainingIncome,
    decimal RemainingExpenses,
    decimal SettledIncome,
    decimal SettledExpenses,
    int UnsettledCount,
    decimal? ClosingBalance);
