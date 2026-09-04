using Dynasty.Carrington.Core.Domain;

namespace Dynasty.Carrington.Blake.Domain.Expenses;

/// <summary>
///     A single amount of money spent on a given day.
/// </summary>
public sealed class Expense : Entity<Guid>
{
    public Expense(Guid id, string description, decimal amount, ExpenseCategory category, DateOnly incurredOn)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Description = description;
        Amount = amount;
        Category = category;
        IncurredOn = incurredOn;
    }

    private Expense()
    {
        Description = string.Empty;
    }

    public string Description { get; private set; }

    public decimal Amount { get; private set; }

    public ExpenseCategory Category { get; private set; }

    public DateOnly IncurredOn { get; private set; }
}