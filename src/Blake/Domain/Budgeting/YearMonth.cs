namespace Dynasty.Carrington.Blake.Domain.Budgeting;

/// <summary>
///     A calendar month, the unit the budget is planned in.
/// </summary>
public readonly record struct YearMonth : IComparable<YearMonth>
{
    public YearMonth(int year, int month)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(year, 9999);
        ArgumentOutOfRangeException.ThrowIfLessThan(month, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);

        Year = year;
        Month = month;
    }

    public int Year { get; }

    public int Month { get; }

    public static YearMonth From(DateOnly date)
    {
        return new YearMonth(date.Year, date.Month);
    }

    /// <summary>
    ///     Rebuilds a month from its <c>yyyyMM</c> key as produced by <see cref="ToKey" />.
    /// </summary>
    public static YearMonth FromKey(int key)
    {
        return new YearMonth(key / 100, key % 100);
    }

    public int ToKey()
    {
        return Year * 100 + Month;
    }

    public YearMonth Next()
    {
        return AddMonths(1);
    }

    public YearMonth Previous()
    {
        return AddMonths(-1);
    }

    public YearMonth AddMonths(int count)
    {
        DateOnly shifted = FirstDay().AddMonths(count);

        return new YearMonth(shifted.Year, shifted.Month);
    }

    public DateOnly FirstDay()
    {
        return new DateOnly(Year, Month, 1);
    }

    /// <summary>
    ///     Every month from <paramref name="from" /> up to and including <paramref name="toInclusive" />, in order.
    /// </summary>
    public static IReadOnlyList<YearMonth> Range(YearMonth from, YearMonth toInclusive)
    {
        if (toInclusive < from)
        {
            throw new ArgumentException("The end of the range precedes its start.", nameof(toInclusive));
        }

        List<YearMonth> months = [];

        for (YearMonth month = from; month <= toInclusive; month = month.Next())
        {
            months.Add(month);
        }

        return months;
    }

    public static YearMonth Min(YearMonth left, YearMonth right)
    {
        return left <= right ? left : right;
    }

    public int CompareTo(YearMonth other)
    {
        return ToKey().CompareTo(other.ToKey());
    }

    public static bool operator <(YearMonth left, YearMonth right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(YearMonth left, YearMonth right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(YearMonth left, YearMonth right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(YearMonth left, YearMonth right)
    {
        return left.CompareTo(right) >= 0;
    }

    public override string ToString()
    {
        return $"{Year:0000}-{Month:00}";
    }
}
