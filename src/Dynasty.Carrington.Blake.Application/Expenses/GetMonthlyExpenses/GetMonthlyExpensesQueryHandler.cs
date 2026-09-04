using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Expenses;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Expenses.GetMonthlyExpenses;

public sealed class GetMonthlyExpensesQueryHandler(IExpenseRepository expenses)
    : IQueryHandler<GetMonthlyExpensesQuery, IReadOnlyList<MonthlyExpenseSummary>>
{
    public async Task<IReadOnlyList<MonthlyExpenseSummary>> Handle(
        GetMonthlyExpensesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Expense> yearsExpenses = await expenses.GetForYear(query.Year, cancellationToken);

        return yearsExpenses
            .GroupBy(expense => expense.IncurredOn.Month)
            .OrderBy(month => month.Key)
            .Select(month => new MonthlyExpenseSummary(
                query.Year,
                month.Key,
                month.Sum(expense => expense.Amount),
                month.Count()))
            .ToList();
    }
}