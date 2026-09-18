using Dynasty.Carrington.Blake.Application.Budgeting.GetMonthOverview;
using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Web.Components.Budget;

/// <summary>
///     Mutable model behind <see cref="PlannedItemDialog" />, used for both adding and editing.
/// </summary>
public sealed class PlannedItemForm
{
    public Guid? Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public PlannedItemKind Kind { get; set; } = PlannedItemKind.Expense;

    public decimal PlannedAmount { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public YearMonth ToMonth()
    {
        return new YearMonth(Year, Month);
    }

    public static PlannedItemForm For(YearMonth month, PlannedItemKind kind)
    {
        return new PlannedItemForm { Kind = kind, Year = month.Year, Month = month.Month };
    }

    public static PlannedItemForm For(PlannedItemView item, YearMonth month)
    {
        return new PlannedItemForm
        {
            Id = item.Id,
            Name = item.Name,
            Kind = item.Kind,
            PlannedAmount = item.PlannedAmount,
            Year = month.Year,
            Month = month.Month
        };
    }
}
