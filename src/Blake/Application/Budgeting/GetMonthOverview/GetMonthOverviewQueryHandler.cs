using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.GetMonthOverview;

public sealed class GetMonthOverviewQueryHandler(
    IPlannedItemRepository items,
    IAccountBalanceRepository balances,
    ICurrentUser user,
    TimeProvider timeProvider)
    : IQueryHandler<GetMonthOverviewQuery, MonthOverview>
{
    public async Task<MonthOverview> Handle(GetMonthOverviewQuery query, CancellationToken cancellationToken)
    {
        YearMonth current = timeProvider.CurrentMonth();
        YearMonth target = query.Month;
        YearMonth previous = target.Previous();

        // The projection needs every month from "now" up to the target; the previous month is
        // loaded as well so the page can offer to copy its plan.
        IReadOnlyList<YearMonth> months = YearMonth.Range(YearMonth.Min(current, previous), target);
        IReadOnlyList<PlannedItem> loaded = await items.GetForMonths(user.UserId, months, cancellationToken);
        AccountBalance? balance = await balances.FindByOwner(user.UserId, cancellationToken);

        MonthSummary summary = BudgetProjection
            .Project(current, balance?.Balance ?? 0m, loaded, target, target)
            .Single();

        List<PlannedItemView> views = loaded
            .Where(item => item.Month == target)
            .OrderBy(item => item.Kind)
            .ThenBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
            .Select(item => new PlannedItemView(
                item.Id,
                item.Kind,
                item.Name,
                item.PlannedAmount,
                item.ActualAmount,
                item.SettledOn,
                item.IsSettled))
            .ToList();

        bool previousHasItems = loaded.Any(item => item.Month == previous);

        return new MonthOverview(
            summary,
            views,
            balance is not null,
            views.Count == 0 && previousHasItems);
    }
}
