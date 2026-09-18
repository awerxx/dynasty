using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

namespace Dynasty.Carrington.Blake.Tests.Budgeting;

public class BudgetProjectionTests
{
    private static readonly YearMonth Current = new(2026, 9);
    private static readonly YearMonth Next = Current.Next();
    private static readonly YearMonth AfterNext = Next.Next();
    private static readonly YearMonth Previous = Current.Previous();

    [Fact]
    public void Counts_unsettled_items_and_ignores_settled_ones()
    {
        PlannedItem[] items =
        [
            Items.Income(Current, 3000m),
            Items.Expense(Current, 1000m, "Mieszkanie"),
            Items.Expense(Current, 500m, "Prąd").Settled(480m)
        ];

        MonthSummary month = Assert.Single(BudgetProjection.Project(Current, 1000m, items, Current, Current));

        Assert.True(month.IsCurrent);
        Assert.False(month.IsPast);
        Assert.Equal(1000m, month.OpeningBalance);
        Assert.Equal(3000m, month.RemainingIncome);
        Assert.Equal(1000m, month.RemainingExpenses);
        Assert.Equal(0m, month.SettledIncome);
        Assert.Equal(480m, month.SettledExpenses);
        Assert.Equal(2, month.UnsettledCount);
        Assert.Equal(3000m, month.ClosingBalance);
    }

    [Fact]
    public void Chains_each_closing_balance_into_the_next_opening()
    {
        PlannedItem[] items =
        [
            Items.Income(Current, 5000m), Items.Expense(Current, 2000m),
            Items.Income(Next, 5000m), Items.Expense(Next, 6000m),
            Items.Expense(AfterNext, 1000m)
        ];

        IReadOnlyList<MonthSummary> months = BudgetProjection.Project(Current, 1000m, items, Current, AfterNext);

        Assert.Collection(
            months,
            current =>
            {
                Assert.Equal(1000m, current.OpeningBalance);
                Assert.Equal(4000m, current.ClosingBalance);
            },
            next =>
            {
                Assert.Equal(4000m, next.OpeningBalance);
                Assert.Equal(3000m, next.ClosingBalance);
            },
            afterNext =>
            {
                Assert.Equal(3000m, afterNext.OpeningBalance);
                Assert.Equal(2000m, afterNext.ClosingBalance);
            });
    }

    [Fact]
    public void Still_chains_through_months_before_the_requested_window()
    {
        PlannedItem[] items =
        [
            Items.Expense(Current, 300m),
            Items.Expense(Next, 200m)
        ];

        MonthSummary afterNext = Assert.Single(BudgetProjection.Project(Current, 1000m, items, AfterNext, AfterNext));

        Assert.Equal(AfterNext, afterNext.Month);
        Assert.Equal(500m, afterNext.OpeningBalance);
        Assert.Equal(500m, afterNext.ClosingBalance);
    }

    [Fact]
    public void Past_months_are_history_only()
    {
        PlannedItem[] items =
        [
            Items.Income(Previous, 5000m).Settled(5100m),
            Items.Expense(Previous, 1000m).Settled(950m),
            Items.Expense(Previous, 200m, "Zapomniane")
        ];

        MonthSummary past = Assert.Single(BudgetProjection.Project(Current, 1000m, items, Previous, Previous));

        Assert.True(past.IsPast);
        Assert.Null(past.OpeningBalance);
        Assert.Null(past.ClosingBalance);
        Assert.Equal(5100m, past.SettledIncome);
        Assert.Equal(950m, past.SettledExpenses);
        Assert.Equal(200m, past.RemainingExpenses);
        Assert.Equal(1, past.UnsettledCount);
    }

    [Fact]
    public void Past_months_do_not_affect_the_current_projection()
    {
        PlannedItem[] items = [Items.Expense(Previous, 999m)];

        IReadOnlyList<MonthSummary> months = BudgetProjection.Project(Current, 1000m, items, Previous, Current);

        Assert.Equal(2, months.Count);
        Assert.Equal(1000m, months[1].OpeningBalance);
        Assert.Equal(1000m, months[1].ClosingBalance);
    }

    [Fact]
    public void Without_items_the_balance_carries_unchanged()
    {
        IReadOnlyList<MonthSummary> months = BudgetProjection.Project(Current, 1234.56m, [], Current, Next);

        Assert.All(months, month =>
        {
            Assert.Equal(1234.56m, month.OpeningBalance);
            Assert.Equal(1234.56m, month.ClosingBalance);
            Assert.Equal(0, month.UnsettledCount);
        });
    }

    [Fact]
    public void Items_of_other_months_are_not_mixed_in()
    {
        PlannedItem[] items =
        [
            Items.Expense(Current, 100m),
            Items.Expense(Next, 200m)
        ];

        MonthSummary current = Assert.Single(BudgetProjection.Project(Current, 0m, items, Current, Current));

        Assert.Equal(100m, current.RemainingExpenses);
        Assert.Equal(-100m, current.ClosingBalance);
    }
}
