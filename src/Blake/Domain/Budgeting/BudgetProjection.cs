namespace Dynasty.Carrington.Blake.Domain.Budgeting;

/// <summary>
///     Projects the account balance month by month. The current month starts from the real balance;
///     every later month opens with the projected close of the month before it. Settled items are
///     excluded because the balance already reflects them.
/// </summary>
public static class BudgetProjection
{
    /// <param name="current">The month today falls in.</param>
    /// <param name="currentBalance">The account balance right now.</param>
    /// <param name="items">Planned items covering at least <c>min(from, current)</c>..<paramref name="toInclusive" />.</param>
    /// <param name="from">First month to return.</param>
    /// <param name="toInclusive">Last month to return.</param>
    public static IReadOnlyList<MonthSummary> Project(
        YearMonth current,
        decimal currentBalance,
        IReadOnlyCollection<PlannedItem> items,
        YearMonth from,
        YearMonth toInclusive)
    {
        ILookup<YearMonth, PlannedItem> byMonth = items.ToLookup(item => item.Month);
        List<MonthSummary> summaries = [];
        decimal? carried = null;

        foreach (YearMonth month in YearMonth.Range(YearMonth.Min(from, current), toInclusive))
        {
            MonthSummary summary = Summarize(month, current, currentBalance, byMonth[month], carried);

            if (summary.ClosingBalance.HasValue)
            {
                carried = summary.ClosingBalance;
            }

            if (month >= from)
            {
                summaries.Add(summary);
            }
        }

        return summaries;
    }

    private static MonthSummary Summarize(
        YearMonth month,
        YearMonth current,
        decimal currentBalance,
        IEnumerable<PlannedItem> items,
        decimal? carried)
    {
        decimal remainingIncome = 0;
        decimal remainingExpenses = 0;
        decimal settledIncome = 0;
        decimal settledExpenses = 0;
        int unsettledCount = 0;

        foreach (PlannedItem item in items)
        {
            bool isIncome = item.Kind == PlannedItemKind.Income;

            if (item.IsSettled)
            {
                decimal actual = item.ActualAmount!.Value;

                if (isIncome)
                {
                    settledIncome += actual;
                }
                else
                {
                    settledExpenses += actual;
                }
            }
            else
            {
                unsettledCount++;

                if (isIncome)
                {
                    remainingIncome += item.PlannedAmount;
                }
                else
                {
                    remainingExpenses += item.PlannedAmount;
                }
            }
        }

        bool isPast = month < current;
        bool isCurrent = month == current;
        decimal? opening = isPast ? null : isCurrent ? currentBalance : carried;
        decimal? closing = opening + remainingIncome - remainingExpenses;

        return new MonthSummary(
            month,
            isPast,
            isCurrent,
            opening,
            remainingIncome,
            remainingExpenses,
            settledIncome,
            settledExpenses,
            unsettledCount,
            closing);
    }
}
