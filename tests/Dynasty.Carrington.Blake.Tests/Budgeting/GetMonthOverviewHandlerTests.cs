using Dynasty.Carrington.Blake.Application.Budgeting.GetMonthOverview;
using Dynasty.Carrington.Blake.Application.Budgeting.GetMonthsOutlook;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

namespace Dynasty.Carrington.Blake.Tests.Budgeting;

public class GetMonthOverviewHandlerTests
{
    private static readonly YearMonth Current = new(2026, 9);
    private static readonly FixedTimeProvider Clock = new(new DateTimeOffset(2026, 9, 18, 12, 0, 0, TimeSpan.Zero));
    private static readonly FakeCurrentUser Me = new(Items.Owner);

    [Fact]
    public async Task Reports_a_missing_balance_and_projects_from_zero()
    {
        MonthOverview overview = await Handler(new InMemoryPlannedItemRepository(Items.Income(Current, 100m)))
            .Handle(new GetMonthOverviewQuery(Current), TestContext.Current.CancellationToken);

        Assert.False(overview.HasAccountBalance);
        Assert.Equal(0m, overview.Summary.OpeningBalance);
        Assert.Equal(100m, overview.Summary.ClosingBalance);
    }

    [Fact]
    public async Task Offers_to_copy_only_when_the_month_is_empty_and_the_previous_one_is_not()
    {
        YearMonth next = Current.Next();
        InMemoryPlannedItemRepository items = new(Items.Expense(Current, 100m));

        MonthOverview emptyNext = await Handler(items).Handle(
            new GetMonthOverviewQuery(next),
            TestContext.Current.CancellationToken);
        MonthOverview currentWithItems = await Handler(items).Handle(
            new GetMonthOverviewQuery(Current),
            TestContext.Current.CancellationToken);
        MonthOverview emptyAfterEmpty = await Handler(items).Handle(
            new GetMonthOverviewQuery(next.Next()),
            TestContext.Current.CancellationToken);

        Assert.True(emptyNext.CanCopyFromPreviousMonth);
        Assert.False(currentWithItems.CanCopyFromPreviousMonth);
        Assert.False(emptyAfterEmpty.CanCopyFromPreviousMonth);
    }

    [Fact]
    public async Task Lists_incomes_before_expenses_and_sorts_by_name()
    {
        InMemoryPlannedItemRepository items = new(
            Items.Expense(Current, 1m, "Zakupy"),
            Items.Expense(Current, 1m, "Auto"),
            Items.Income(Current, 1m, "Wypłata"));

        MonthOverview overview = await Handler(items).Handle(
            new GetMonthOverviewQuery(Current),
            TestContext.Current.CancellationToken);

        Assert.Equal(["Wypłata", "Auto", "Zakupy"], overview.Items.Select(item => item.Name));
    }

    [Fact]
    public async Task Outlook_projects_a_future_window_from_the_current_balance()
    {
        YearMonth next = Current.Next();
        InMemoryPlannedItemRepository items = new(
            Items.Expense(Current, 100m),
            Items.Expense(next, 200m));
        InMemoryAccountBalanceRepository balances = new(
            new AccountBalance(Guid.NewGuid(), Items.Owner, 1000m, Clock.GetUtcNow()));

        IReadOnlyList<MonthSummary> outlook = await new GetMonthsOutlookQueryHandler(items, balances, Me, Clock)
            .Handle(new GetMonthsOutlookQuery(next, 2), TestContext.Current.CancellationToken);

        Assert.Equal([next, next.Next()], outlook.Select(month => month.Month));
        Assert.Equal(900m, outlook[0].OpeningBalance);
        Assert.Equal(700m, outlook[0].ClosingBalance);
        Assert.Equal(700m, outlook[1].ClosingBalance);
    }

    private static GetMonthOverviewQueryHandler Handler(InMemoryPlannedItemRepository items)
    {
        return new GetMonthOverviewQueryHandler(items, new InMemoryAccountBalanceRepository(), Me, Clock);
    }
}
