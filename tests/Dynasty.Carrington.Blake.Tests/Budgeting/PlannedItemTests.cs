using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Tests.Budgeting;

public class PlannedItemTests
{
    private static readonly YearMonth Month = new(2026, 9);

    [Fact]
    public void Rejects_a_blank_name()
    {
        Assert.Throws<ArgumentException>(() => Create(name: "   "));
    }

    [Fact]
    public void Rejects_a_name_that_is_too_long()
    {
        Assert.Throws<ArgumentException>(() => Create(name: new string('x', PlannedItem.MaxNameLength + 1)));
    }

    [Fact]
    public void Trims_the_name()
    {
        Assert.Equal("Mieszkanie", Create(name: "  Mieszkanie ").Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Rejects_a_non_positive_planned_amount(decimal amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(amount: amount));
    }

    [Fact]
    public void Rejects_a_blank_owner()
    {
        Assert.Throws<ArgumentException>(() => Create(owner: ""));
    }

    [Fact]
    public void Starts_unsettled_with_the_planned_amount_as_effective()
    {
        PlannedItem item = Create(amount: 1000m);

        Assert.False(item.IsSettled);
        Assert.Null(item.ActualAmount);
        Assert.Null(item.SettledOn);
        Assert.Equal(1000m, item.EffectiveAmount);
    }

    [Fact]
    public void Settle_records_the_real_amount_and_date()
    {
        PlannedItem item = Create(amount: 1000m);
        DateOnly paidOn = new(2026, 9, 10);

        item.Settle(1050m, paidOn);

        Assert.True(item.IsSettled);
        Assert.Equal(1050m, item.ActualAmount);
        Assert.Equal(paidOn, item.SettledOn);
        Assert.Equal(1050m, item.EffectiveAmount);
        Assert.Equal(1000m, item.PlannedAmount);
    }

    [Fact]
    public void Settling_again_corrects_the_amount()
    {
        PlannedItem item = Create(amount: 1000m);
        item.Settle(1050m, new DateOnly(2026, 9, 10));

        item.Settle(990m, new DateOnly(2026, 9, 11));

        Assert.Equal(990m, item.ActualAmount);
        Assert.Equal(new DateOnly(2026, 9, 11), item.SettledOn);
    }

    [Fact]
    public void Settle_rejects_a_non_positive_amount()
    {
        PlannedItem item = Create();

        Assert.Throws<ArgumentOutOfRangeException>(() => item.Settle(0m, new DateOnly(2026, 9, 10)));
        Assert.False(item.IsSettled);
    }

    [Fact]
    public void Unsettle_clears_the_settlement()
    {
        PlannedItem item = Create();
        item.Settle(1050m, new DateOnly(2026, 9, 10));

        item.Unsettle();

        Assert.False(item.IsSettled);
        Assert.Null(item.ActualAmount);
        Assert.Null(item.SettledOn);
    }

    [Fact]
    public void Update_changes_every_planning_field()
    {
        PlannedItem item = Create();
        YearMonth next = Month.Next();

        item.Update("Czynsz", PlannedItemKind.Expense, 1200m, next);

        Assert.Equal("Czynsz", item.Name);
        Assert.Equal(PlannedItemKind.Expense, item.Kind);
        Assert.Equal(1200m, item.PlannedAmount);
        Assert.Equal(next, item.Month);
    }

    [Fact]
    public void Update_validates_like_the_constructor()
    {
        PlannedItem item = Create();

        Assert.Throws<ArgumentException>(() => item.Update("", PlannedItemKind.Expense, 1m, Month));
        Assert.Throws<ArgumentOutOfRangeException>(() => item.Update("Czynsz", PlannedItemKind.Expense, 0m, Month));
    }

    [Fact]
    public void Knows_its_owner()
    {
        PlannedItem item = Create(owner: "alice");

        Assert.True(item.IsOwnedBy("alice"));
        Assert.False(item.IsOwnedBy("bob"));
    }

    private static PlannedItem Create(string name = "Mieszkanie", decimal amount = 1000m, string owner = "owner")
    {
        return new PlannedItem(Guid.NewGuid(), owner, Month, PlannedItemKind.Expense, name, amount);
    }
}
