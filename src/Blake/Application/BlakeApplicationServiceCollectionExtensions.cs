using Dynasty.Carrington.Blake.Application.Budgeting.AddPlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.CopyPlanFromPreviousMonth;
using Dynasty.Carrington.Blake.Application.Budgeting.DeletePlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.GetAccountBalance;
using Dynasty.Carrington.Blake.Application.Budgeting.GetMonthOverview;
using Dynasty.Carrington.Blake.Application.Budgeting.GetMonthsOutlook;
using Dynasty.Carrington.Blake.Application.Budgeting.SetAccountBalance;
using Dynasty.Carrington.Blake.Application.Budgeting.SettlePlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.UnsettlePlannedItem;
using Dynasty.Carrington.Blake.Application.Budgeting.UpdatePlannedItem;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BlakeApplicationServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the Blake use cases. The host still has to provide
    ///     <see cref="Blake.Application.Abstractions.ICurrentUser" />, <see cref="TimeProvider" />,
    ///     <see cref="Blake.Application.Abstractions.IUnitOfWork" /> and the repositories.
    /// </summary>
    public static IServiceCollection AddBlakeApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddPlannedItemCommand, Guid>, AddPlannedItemCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePlannedItemCommand>, UpdatePlannedItemCommandHandler>();
        services.AddScoped<ICommandHandler<SettlePlannedItemCommand>, SettlePlannedItemCommandHandler>();
        services.AddScoped<ICommandHandler<UnsettlePlannedItemCommand>, UnsettlePlannedItemCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePlannedItemCommand>, DeletePlannedItemCommandHandler>();
        services.AddScoped<ICommandHandler<SetAccountBalanceCommand>, SetAccountBalanceCommandHandler>();
        services.AddScoped<ICommandHandler<CopyPlanFromPreviousMonthCommand, int>, CopyPlanFromPreviousMonthCommandHandler>();

        services.AddScoped<IQueryHandler<GetMonthOverviewQuery, MonthOverview>, GetMonthOverviewQueryHandler>();
        services.AddScoped<IQueryHandler<GetMonthsOutlookQuery, IReadOnlyList<MonthSummary>>, GetMonthsOutlookQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountBalanceQuery, AccountBalanceView>, GetAccountBalanceQueryHandler>();

        return services;
    }
}
