using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Expenses;
using Dynasty.Carrington.Blake.Infrastructure.Database.Repositories;
using Dynasty.Carrington.Blake.Infrastructure.Database.Seeding;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dynasty.Carrington.Blake.Infrastructure.Database;

public static class BlakeDatabaseServiceCollectionExtensions
{
    private const string InMemoryDatabaseName = "blake";

    /// <summary>
    ///     Registers the Blake database. Swapping the in-memory store for a real one is a change to the
    ///     provider call below.
    /// </summary>
    public static IServiceCollection AddBlakeDatabase(this IServiceCollection services)
    {
        services.AddDbContext<BlakeDbContext>(options => options.UseInMemoryDatabase(InMemoryDatabaseName));
        services.AddScoped<IExpenseRepository, ExpenseRepository>();

        return services;
    }

    /// <summary>
    ///     Creates the database and fills it with dummy expenses when it is still empty.
    /// </summary>
    public static async Task InitializeBlakeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        BlakeDbContext database = scope.ServiceProvider.GetRequiredService<BlakeDbContext>();

        await database.Database.EnsureCreatedAsync(cancellationToken);

        if (await database.Expenses.AnyAsync(cancellationToken))
        {
            return;
        }

        IReadOnlyList<Expense> expenses = BlakeSeedData.CreateExpenses(DateTime.Today.Year);

        database.Expenses.AddRange(expenses);
        await database.SaveChangesAsync(cancellationToken);
    }
}