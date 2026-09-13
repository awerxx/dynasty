namespace Dynasty.Carrington.Blake.Application.Expenses.GetMonthlyExpenses;

/// <summary>
///     Asks for the expense totals of every month in <paramref name="Year" /> that has any expenses.
/// </summary>
public sealed record GetMonthlyExpensesQuery(int Year);