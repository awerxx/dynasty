using Dynasty.Carrington.Blake.Domain.Expenses;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Seeding;

/// <summary>
///     Dummy household expenses for the current year. Deterministic, so restarting the app reproduces
///     the same figures.
/// </summary>
internal static class BlakeSeedData
{
    private const int RandomSeed = 20260904;

    private static readonly (ExpenseCategory Category, string Description, decimal Low, decimal High)[] Templates =
    [
        (ExpenseCategory.Housing, "Rent", 1200m, 1200m),
        (ExpenseCategory.Utilities, "Electricity and gas", 60m, 190m),
        (ExpenseCategory.Utilities, "Internet", 45m, 45m),
        (ExpenseCategory.Groceries, "Weekly groceries", 90m, 180m),
        (ExpenseCategory.Groceries, "Weekly groceries", 90m, 180m),
        (ExpenseCategory.Transport, "Fuel", 40m, 120m),
        (ExpenseCategory.Leisure, "Streaming and books", 15m, 70m),
        (ExpenseCategory.Health, "Pharmacy", 12m, 95m),
        (ExpenseCategory.Other, "Household bits", 20m, 130m)
    ];

    public static IReadOnlyList<Expense> CreateExpenses(int year)
    {
        Random random = new(RandomSeed);
        List<Expense> expenses = [];

        for (int month = 1; month <= 12; month++)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int count = random.Next(5, Templates.Length + 1);

            foreach ((ExpenseCategory category, string description, decimal low, decimal high) in Templates.Take(count))
            {
                decimal amount = low == high
                    ? low
                    : Math.Round(low + ((high - low) * (decimal)random.NextDouble()), 2);

                DateOnly incurredOn = new(year, month, random.Next(1, daysInMonth + 1));

                expenses.Add(new Expense(Guid.CreateVersion7(), description, amount, category, incurredOn));
            }
        }

        return expenses;
    }
}