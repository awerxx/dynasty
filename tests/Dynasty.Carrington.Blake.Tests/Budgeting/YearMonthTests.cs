using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Tests.Budgeting;

public class YearMonthTests
{
    [Fact]
    public void Next_and_previous_cross_year_boundaries()
    {
        Assert.Equal(new YearMonth(2027, 1), new YearMonth(2026, 12).Next());
        Assert.Equal(new YearMonth(2025, 12), new YearMonth(2026, 1).Previous());
    }

    [Fact]
    public void Key_round_trips()
    {
        YearMonth month = new(2026, 9);

        Assert.Equal(202609, month.ToKey());
        Assert.Equal(month, YearMonth.FromKey(month.ToKey()));
    }

    [Fact]
    public void Orders_by_year_then_month()
    {
        YearMonth earlier = new(2025, 12);
        YearMonth later = new(2026, 1);

        Assert.True(earlier < later);
        Assert.True(later >= earlier);
        Assert.Equal(earlier, YearMonth.Min(later, earlier));
    }

    [Fact]
    public void Range_is_inclusive_and_ordered()
    {
        IReadOnlyList<YearMonth> range = YearMonth.Range(new YearMonth(2026, 11), new YearMonth(2027, 2));

        Assert.Equal(
            [new YearMonth(2026, 11), new YearMonth(2026, 12), new YearMonth(2027, 1), new YearMonth(2027, 2)],
            range);
    }

    [Fact]
    public void Range_rejects_a_reversed_span()
    {
        Assert.Throws<ArgumentException>(() => YearMonth.Range(new YearMonth(2026, 2), new YearMonth(2026, 1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Rejects_invalid_months(int month)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new YearMonth(2026, month));
    }
}
