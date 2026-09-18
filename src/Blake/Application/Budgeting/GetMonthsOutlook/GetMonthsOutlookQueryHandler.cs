using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.GetMonthsOutlook;

public sealed class GetMonthsOutlookQueryHandler(
    IPlannedItemRepository items,
    IAccountBalanceRepository balances,
    ICurrentUser user,
    TimeProvider timeProvider)
    : IQueryHandler<GetMonthsOutlookQuery, IReadOnlyList<MonthSummary>>
{
    public async Task<IReadOnlyList<MonthSummary>> Handle(
        GetMonthsOutlookQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(query.Count);

        YearMonth current = timeProvider.CurrentMonth();
        YearMonth last = query.From.AddMonths(query.Count - 1);

        IReadOnlyList<YearMonth> months = YearMonth.Range(YearMonth.Min(current, query.From), last);
        IReadOnlyList<PlannedItem> loaded = await items.GetForMonths(user.UserId, months, cancellationToken);
        AccountBalance? balance = await balances.FindByOwner(user.UserId, cancellationToken);

        return BudgetProjection.Project(current, balance?.Balance ?? 0m, loaded, query.From, last);
    }
}
