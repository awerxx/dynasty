using Dynasty.Carrington.Blake.Application.Expenses.GetMonthlyExpenses;
using Dynasty.Carrington.Core.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BlakeApplicationServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the Blake use cases. The host still has to register an
    ///     <see cref="Blake.Application.Abstractions.IExpenseRepository" /> implementation.
    /// </summary>
    public static IServiceCollection AddBlakeApplication(this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetMonthlyExpensesQuery, IReadOnlyList<MonthlyExpenseSummary>>,
            GetMonthlyExpensesQueryHandler>();

        return services;
    }
}