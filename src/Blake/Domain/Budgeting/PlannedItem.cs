using Dynasty.Carrington.Core.Domain;

namespace Dynasty.Carrington.Blake.Domain.Budgeting;

/// <summary>
///     A planned income or cost in a given month. Once the money actually moves the item is settled
///     with the real amount; settled items are already reflected in the account balance and therefore
///     no longer count towards the projection.
/// </summary>
public sealed class PlannedItem : Entity<Guid>
{
    public const int MaxNameLength = 100;

    public PlannedItem(
        Guid id,
        string ownerId,
        YearMonth month,
        PlannedItemKind kind,
        string name,
        decimal plannedAmount)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);

        OwnerId = ownerId;
        Month = month;
        Kind = kind;
        Name = ValidName(name);
        PlannedAmount = ValidAmount(plannedAmount);
    }

    private PlannedItem()
    {
        OwnerId = string.Empty;
        Name = string.Empty;
    }

    public string OwnerId { get; private set; }

    public YearMonth Month { get; private set; }

    public PlannedItemKind Kind { get; private set; }

    public string Name { get; private set; }

    public decimal PlannedAmount { get; private set; }

    /// <summary>
    ///     The amount that was really paid or received. Set exactly when the item is settled.
    /// </summary>
    public decimal? ActualAmount { get; private set; }

    public DateOnly? SettledOn { get; private set; }

    public bool IsSettled => SettledOn.HasValue;

    /// <summary>
    ///     The best known amount: the real one once settled, the planned one before that.
    /// </summary>
    public decimal EffectiveAmount => ActualAmount ?? PlannedAmount;

    public bool IsOwnedBy(string ownerId)
    {
        return string.Equals(OwnerId, ownerId, StringComparison.Ordinal);
    }

    public void Update(string name, PlannedItemKind kind, decimal plannedAmount, YearMonth month)
    {
        Name = ValidName(name);
        Kind = kind;
        PlannedAmount = ValidAmount(plannedAmount);
        Month = month;
    }

    /// <summary>
    ///     Records the real amount. Settling an already settled item simply corrects it.
    /// </summary>
    public void Settle(decimal actualAmount, DateOnly on)
    {
        ActualAmount = ValidAmount(actualAmount);
        SettledOn = on;
    }

    public void Unsettle()
    {
        ActualAmount = null;
        SettledOn = null;
    }

    private static string ValidName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string trimmed = name.Trim();

        if (trimmed.Length > MaxNameLength)
        {
            throw new ArgumentException($"The name must be at most {MaxNameLength} characters.", nameof(name));
        }

        return trimmed;
    }

    private static decimal ValidAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        return amount;
    }
}
