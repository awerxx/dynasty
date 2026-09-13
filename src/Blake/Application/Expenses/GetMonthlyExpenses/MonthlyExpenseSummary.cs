namespace Dynasty.Carrington.Blake.Application.Expenses.GetMonthlyExpenses;

/// <summary>
///     What was spent in one month.
/// </summary>
public sealed record MonthlyExpenseSummary(int Year, int Month, decimal Total, int ExpenseCount);