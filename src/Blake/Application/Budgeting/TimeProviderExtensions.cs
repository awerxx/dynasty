using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Budgeting;

internal static class TimeProviderExtensions
{
    public static DateOnly Today(this TimeProvider timeProvider)
    {
        return DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    }

    public static YearMonth CurrentMonth(this TimeProvider timeProvider)
    {
        return YearMonth.From(timeProvider.Today());
    }
}
