using Dynasty.Carrington.Core.Domain;

namespace Dynasty.Carrington.Blake.Domain.Budgeting;

/// <summary>
///     The current account balance of one owner, maintained by hand. One per owner.
/// </summary>
public sealed class AccountBalance : Entity<Guid>
{
    public AccountBalance(Guid id, string ownerId, decimal balance, DateTimeOffset updatedAt)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);

        OwnerId = ownerId;
        Balance = balance;
        UpdatedAt = updatedAt;
    }

    private AccountBalance()
    {
        OwnerId = string.Empty;
    }

    public string OwnerId { get; private set; }

    /// <summary>
    ///     May be negative (overdraft).
    /// </summary>
    public decimal Balance { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Set(decimal balance, DateTimeOffset at)
    {
        Balance = balance;
        UpdatedAt = at;
    }
}
