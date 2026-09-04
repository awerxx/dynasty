using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Application.Expenses.GetMonthlyExpenses;
using Dynasty.Carrington.Blake.Domain.Expenses;

namespace Dynasty.Carrington.Blake.Tests.Expenses;

public class GetMonthlyExpensesQueryHandlerTests
{
    private const int Year = 2026;

    [Fact]
    public async Task Groups_expenses_into_the_month_they_were_incurred()
    {
        GetMonthlyExpensesQueryHandler handler = HandlerFor(
            ExpenseOn(1, 3, 100m),
            ExpenseOn(1, 28, 50.50m),
            ExpenseOn(3, 15, 20m));

        IReadOnlyList<MonthlyExpenseSummary> summaries = await handler.Handle(
            new GetMonthlyExpensesQuery(Year),
            TestContext.Current.CancellationToken);

        Assert.Collection(
            summaries,
            january =>
            {
                Assert.Equal(1, january.Month);
                Assert.Equal(150.50m, january.Total);
                Assert.Equal(2, january.ExpenseCount);
            },
            march =>
            {
                Assert.Equal(3, march.Month);
                Assert.Equal(20m, march.Total);
                Assert.Equal(1, march.ExpenseCount);
            });
    }

    [Fact]
    public async Task Orders_summaries_by_month()
    {
        GetMonthlyExpensesQueryHandler handler = HandlerFor(
            ExpenseOn(11, 1, 10m),
            ExpenseOn(2, 1, 10m),
            ExpenseOn(7, 1, 10m));

        IReadOnlyList<MonthlyExpenseSummary> summaries = await handler.Handle(
            new GetMonthlyExpensesQuery(Year),
            TestContext.Current.CancellationToken);

        Assert.Equal([2, 7, 11], summaries.Select(summary => summary.Month));
    }

    [Fact]
    public async Task Omits_months_without_expenses()
    {
        GetMonthlyExpensesQueryHandler handler = HandlerFor(ExpenseOn(5, 9, 1m));

        IReadOnlyList<MonthlyExpenseSummary> summaries = await handler.Handle(
            new GetMonthlyExpensesQuery(Year),
            TestContext.Current.CancellationToken);

        MonthlyExpenseSummary only = Assert.Single(summaries);
        Assert.Equal(5, only.Month);
    }

    [Fact]
    public async Task Returns_nothing_when_the_year_has_no_expenses()
    {
        GetMonthlyExpensesQueryHandler handler = HandlerFor();

        IReadOnlyList<MonthlyExpenseSummary> summaries = await handler.Handle(
            new GetMonthlyExpensesQuery(Year),
            TestContext.Current.CancellationToken);

        Assert.Empty(summaries);
    }

    [Fact]
    public async Task Stamps_every_summary_with_the_requested_year()
    {
        GetMonthlyExpensesQueryHandler handler = HandlerFor(
            ExpenseOn(4, 2, 5m),
            ExpenseOn(6, 2, 5m));

        IReadOnlyList<MonthlyExpenseSummary> summaries = await handler.Handle(
            new GetMonthlyExpensesQuery(Year),
            TestContext.Current.CancellationToken);

        Assert.All(summaries, summary => Assert.Equal(Year, summary.Year));
    }

    private static GetMonthlyExpensesQueryHandler HandlerFor(params Expense[] expenses)
    {
        return new GetMonthlyExpensesQueryHandler(new FakeExpenseRepository(expenses));
    }

    private static Expense ExpenseOn(int month, int day, decimal amount)
    {
        return new Expense(Guid.NewGuid(), "Test expense", amount, ExpenseCategory.Other,
            new DateOnly(Year, month, day));
    }

    private sealed class FakeExpenseRepository(IReadOnlyList<Expense> expenses) : IExpenseRepository
    {
        public Task<IReadOnlyList<Expense>> GetForYear(int year, CancellationToken cancellationToken)
        {
            IReadOnlyList<Expense> forYear = expenses
                .Where(expense => expense.IncurredOn.Year == year)
                .ToList();

            return Task.FromResult(forYear);
        }
    }
}