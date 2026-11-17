using Dynasty.Carrington.Blake.Domain.Common;

namespace Dynasty.Carrington.Blake.Domain.Entities;

/// <summary>Kategoria wydatków zdefiniowana przez użytkownika w obrębie jednego konta.</summary>
public sealed class ExpenseCategory
{
    public const int NameMaxLength = 100;

    private ExpenseCategory()
    {
        OwnerId = null!;
        Name = null!;
    }

    public ExpenseCategory(Account account, string name)
    {
        ArgumentNullException.ThrowIfNull(account);
        account.EnsureActive();

        Id = Guid.CreateVersion7();
        OwnerId = account.OwnerId;
        AccountId = account.Id;
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string OwnerId { get; private set; }
    public Guid AccountId { get; private set; }
    public Account? Account { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }

    public bool IsArchived => ArchivedAt is not null;

    public void Rename(string name)
    {
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
    }

    /// <summary>Kategoria znika z list wyboru, ale zostaje w historii (SPEC, reguła 5).</summary>
    public void Archive()
    {
        ArchivedAt ??= DateTime.UtcNow;
    }

    public void Restore()
    {
        ArchivedAt = null;
    }

    internal void EnsureUsableFor(Account account, bool allowArchived = false)
    {
        if (AccountId != account.Id)
            throw new DomainException("Kategoria nie należy do wybranego konta.");

        if (IsArchived && !allowArchived)
            throw new DomainException("Kategoria jest zarchiwizowana.");
    }
}
