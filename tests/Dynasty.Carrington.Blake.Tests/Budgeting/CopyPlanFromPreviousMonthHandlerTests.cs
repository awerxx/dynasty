using Dynasty.Carrington.Blake.Application.Budgeting.CopyPlanFromPreviousMonth;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

namespace Dynasty.Carrington.Blake.Tests.Budgeting;

public class CopyPlanFromPreviousMonthHandlerTests
{
    private static readonly YearMonth Target = new(2026, 10);
    private static readonly YearMonth Source = Target.Previous();

    [Fact]
    public async Task Copies_planned_amounts_of_unsettled_and_actual_amounts_of_settled_items()
    {
        InMemoryPlannedItemRepository repository = new(
            Items.Income(Source, 6000m, "Wypłata"),
            Items.Expense(Source, 1000m, "Mieszkanie").Settled(1050m));
        NoOpUnitOfWork unitOfWork = new();

        int copied = await Handler(repository, unitOfWork).Handle(
            new CopyPlanFromPreviousMonthCommand(Target),
            TestContext.Current.CancellationToken);

        Assert.Equal(2, copied);
        Assert.Equal(1, unitOfWork.SaveCount);

        List<PlannedItem> created = repository.Items.Where(item => item.Month == Target).ToList();
        Assert.Equal(2, created.Count);
        Assert.All(created, item =>
        {
            Assert.False(item.IsSettled);
            Assert.Equal(Items.Owner, item.OwnerId);
        });
        Assert.Equal(6000m, created.Single(item => item.Name == "Wypłata").PlannedAmount);
        Assert.Equal(1050m, created.Single(item => item.Name == "Mieszkanie").PlannedAmount);
    }

    [Fact]
    public async Task Creates_new_ids()
    {
        PlannedItem source = Items.Expense(Source, 100m);
        InMemoryPlannedItemRepository repository = new(source);

        await Handler(repository, new NoOpUnitOfWork()).Handle(
            new CopyPlanFromPreviousMonthCommand(Target),
            TestContext.Current.CancellationToken);

        PlannedItem copy = repository.Items.Single(item => item.Month == Target);
        Assert.NotEqual(source.Id, copy.Id);
    }

    [Fact]
    public async Task Skips_items_the_target_month_already_has()
    {
        InMemoryPlannedItemRepository repository = new(
            Items.Expense(Source, 1000m, "Mieszkanie"),
            Items.Expense(Source, 300m, "Jedzenie"),
            Items.Expense(Target, 1100m, "MIESZKANIE"));
        NoOpUnitOfWork unitOfWork = new();

        int copied = await Handler(repository, unitOfWork).Handle(
            new CopyPlanFromPreviousMonthCommand(Target),
            TestContext.Current.CancellationToken);

        Assert.Equal(1, copied);
        Assert.Equal(["MIESZKANIE", "Jedzenie"], repository.Items.Where(item => item.Month == Target).Select(item => item.Name));
    }

    [Fact]
    public async Task Does_nothing_when_the_previous_month_is_empty()
    {
        InMemoryPlannedItemRepository repository = new();
        NoOpUnitOfWork unitOfWork = new();

        int copied = await Handler(repository, unitOfWork).Handle(
            new CopyPlanFromPreviousMonthCommand(Target),
            TestContext.Current.CancellationToken);

        Assert.Equal(0, copied);
        Assert.Equal(0, unitOfWork.SaveCount);
        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task Ignores_other_users_items()
    {
        InMemoryPlannedItemRepository repository = new(Items.Expense(Source, 100m, owner: Items.OtherOwner));

        int copied = await Handler(repository, new NoOpUnitOfWork()).Handle(
            new CopyPlanFromPreviousMonthCommand(Target),
            TestContext.Current.CancellationToken);

        Assert.Equal(0, copied);
        Assert.Single(repository.Items);
    }

    private static CopyPlanFromPreviousMonthCommandHandler Handler(
        InMemoryPlannedItemRepository repository,
        NoOpUnitOfWork unitOfWork)
    {
        return new CopyPlanFromPreviousMonthCommandHandler(repository, unitOfWork, new FakeCurrentUser(Items.Owner));
    }
}
