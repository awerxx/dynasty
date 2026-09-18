using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Infrastructure.Database.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dynasty.Carrington.Blake.Infrastructure.Database;

public static class BlakeDatabaseServiceCollectionExtensions
{
    private const string InMemoryDatabaseName = "blake";

    /// <summary>
    ///     Registers the Blake database. Swapping the in-memory store for a real one is a change to the
    ///     provider call below; nothing in the model configuration is provider specific.
    /// </summary>
    public static IServiceCollection AddBlakeDatabase(this IServiceCollection services)
    {
        services.AddDbContext<BlakeDbContext>(options => options.UseInMemoryDatabase(InMemoryDatabaseName));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<BlakeDbContext>());
        services.AddScoped<IPlannedItemRepository, PlannedItemRepository>();
        services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();

        return services;
    }

    /// <summary>
    ///     Makes sure the database exists. With a real provider this is where migrations would run.
    /// </summary>
    public static async Task InitializeBlakeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        BlakeDbContext database = scope.ServiceProvider.GetRequiredService<BlakeDbContext>();

        await database.Database.EnsureCreatedAsync(cancellationToken);
    }
}
