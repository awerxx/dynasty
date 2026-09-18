using System.Globalization;

using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Web.Formatting;

/// <summary>
///     Polish presentation of money, dates and domain names.
/// </summary>
public static class Pl
{
    public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("pl-PL");

    public static string Money(decimal amount)
    {
        return amount.ToString("C2", Culture);
    }

    public static string Money(decimal? amount)
    {
        return amount.HasValue ? Money(amount.Value) : "—";
    }

    /// <summary>
    ///     "wrzesień 2026". A pattern without a day component yields the nominative month name.
    /// </summary>
    public static string MonthName(YearMonth month)
    {
        return month.FirstDay().ToDateTime(TimeOnly.MinValue).ToString("MMMM yyyy", Culture);
    }

    /// <summary>
    ///     "Wrzesień", for pickers.
    /// </summary>
    public static string MonthNameOnly(int month)
    {
        return Culture.TextInfo.ToTitleCase(Culture.DateTimeFormat.GetMonthName(month));
    }

    public static string Date(DateOnly date)
    {
        return date.ToString("dd.MM.yyyy", Culture);
    }

    public static string ShortDate(DateOnly date)
    {
        return date.ToString("dd.MM", Culture);
    }

    public static string Timestamp(DateTimeOffset at)
    {
        return at.ToLocalTime().ToString("dd.MM.yyyy HH:mm", Culture);
    }

    public static string KindName(PlannedItemKind kind)
    {
        return kind switch
        {
            PlannedItemKind.Income => "Przychód",
            PlannedItemKind.Expense => "Koszt",
            _ => kind.ToString()
        };
    }

    /// <summary>
    ///     Polish plural of "pozycja": 1 pozycja, 2 pozycje, 5 pozycji, 22 pozycje.
    /// </summary>
    public static string ItemsCount(int count)
    {
        int ones = count % 10;
        int tens = count % 100;

        string noun = count == 1
            ? "pozycja"
            : ones is >= 2 and <= 4 && tens is < 12 or > 14
                ? "pozycje"
                : "pozycji";

        return $"{count} {noun}";
    }
}
