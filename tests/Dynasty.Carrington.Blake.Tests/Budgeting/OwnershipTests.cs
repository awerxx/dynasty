using Dynasty.Carrington.Blake.Application.Budgeting;
using Dynasty.Carrington.Blake.Application.Budgeting.DeletePlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.GetMonthOverview;
using Dynasty.Carrington.Blake.Application.Budgeting.SetAccountBalance;
using Dynasty.Carrington.Blake.Application.Budgeting.SettlePlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.UnsettlePlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.UpdatePlannedItem;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

namespace Dynasty.Carrington.Blake.Tests.Budgeting;

public class OwnershipTests
{
    private static readonly YearMonth Month = new(2026, 9);
    private static readonly FixedTimeProvider Clock = new(new DateTimeOffset(2026, 9, 18, 12, 0, 0, TimeSpan.Zero));
    private static readonly FakeCurrentUser Me = new(Items.Owner);

    [Fact]
    public async Task Overview_only_shows_the_callers_items_and_balance()
    {
        InMemoryPlannedItemRepository items = new(
            Items.Expense(Month, 100m, "Moje"),
            Items.Expense(Month, 999m, "Cudze", Items.OtherOwner));
        InMemoryAccountBalanceRepository balances = new(
            new AccountBalance(Guid.NewGuid(), Items.Owner, 1000m, Clock.GetUtcNow()),
            new AccountBalance(Guid.NewGuid(), Items.OtherOwner, 50m, Clock.GetUtcNow()));

        MonthOverview overview = await new GetMonthOverviewQueryHandler(items, balances, Me, Clock).Handle(
            new GetMonthOverviewQuery(Month),
            TestContext.Current.CancellationToken);

        PlannedItemView only = Assert.Single(overview.Items);
        Assert.Equal("Moje", only.Name);
        Assert.Equal(1000m, overview.Summary.OpeningBalance);
        Assert.Equal(900m, overview.Summary.ClosingBalance);
    }

    [Fact]
    public async Task Settling_someone_elses_item_is_reported_as_not_found()
    {
        PlannedItem theirs = Items.Expense(Month, 100m, owner: Items.OtherOwner);
        InMemoryPlannedItemRepository items = new(theirs);

        await Assert.ThrowsAsync<PlannedItemNotFoundException>(() =>
            new SettlePlannedItemCommandHandler(items, new NoOpUnitOfWork(), Me).Handle(
                new SettlePlannedItemCommand(theirs.Id, 100m, new DateOnly(2026, 9, 18)),
                TestContext.Current.CancellationToken));

        Assert.False(theirs.IsSettled);
    }

    [Fact]
    public async Task Unsettling_updating_and_deleting_someone_elses_item_is_reported_as_not_found()
    {
        PlannedItem theirs = Items.Expense(Month, 100m, owner: Items.OtherOwner).Settled(100m);
        InMemoryPlannedItemRepository items = new(theirs);
        NoOpUnitOfWork unitOfWork = new();

        await Assert.ThrowsAsync<PlannedItemNotFoundException>(() =>
            new UnsettlePlannedItemCommandHandler(items, unitOfWork, Me).Handle(
                new UnsettlePlannedItemCommand(theirs.Id),
                TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<PlannedItemNotFoundException>(() =>
            new UpdatePlannedItemCommandHandler(items, unitOfWork, Me).Handle(
                new UpdatePlannedItemCommand(theirs.Id, Month, PlannedItemKind.Expense, "Zmienione", 1m),
                TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<PlannedItemNotFoundException>(() =>
            new DeletePlannedItemCommandHandler(items, unitOfWork, Me).Handle(
                new DeletePlannedItemCommand(theirs.Id),
                TestContext.Current.CancellationToken));

        Assert.True(theirs.IsSettled);
        Assert.Equal("Mieszkanie", theirs.Name);
        Assert.Single(items.Items);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Unknown_ids_are_reported_as_not_found()
    {
        await Assert.ThrowsAsync<PlannedItemNotFoundException>(() =>
            new DeletePlannedItemCommandHandler(new InMemoryPlannedItemRepository(), new NoOpUnitOfWork(), Me).Handle(
                new DeletePlannedItemCommand(Guid.NewGuid()),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Setting_the_balance_creates_it_for_a_new_owner_and_updates_it_afterwards()
    {
        InMemoryAccountBalanceRepository balances = new();
        NoOpUnitOfWork unitOfWork = new();
        SetAccountBalanceCommandHandler handler = new(balances, unitOfWork, Me, Clock);

        await handler.Handle(new SetAccountBalanceCommand(2500m), TestContext.Current.CancellationToken);
        await handler.Handle(new SetAccountBalanceCommand(-100m), TestContext.Current.CancellationToken);

        AccountBalance mine = Assert.Single(balances.Balances);
        Assert.Equal(Items.Owner, mine.OwnerId);
        Assert.Equal(-100m, mine.Balance);
        Assert.Equal(Clock.GetUtcNow(), mine.UpdatedAt);
        Assert.Equal(2, unitOfWork.SaveCount);
    }
}
