using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Entities;

/// <summary>
/// Pozycja planu na dany miesiąc (np. leasing 1500 zł). Po zapłacie powstaje dokładnie jeden
/// <see cref="Expense"/> z faktyczną kwotą, a pozycja przechodzi w <see cref="BudgetStatus.Done"/>.
/// </summary>
public sealed class Budget
{
    public const int NameMaxLength = 200;
    public const int CommentMaxLength = 1000;

    private Budget()
    {
        OwnerId = null!;
        Name = null!;
    }

    public Budget(
        Account account,
        ExpenseCategory category,
        string name,
        decimal amount,
        DateOnly month,
        DateOnly? dueDate = null,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);
        account.EnsureActive();
        category.EnsureUsableFor(account);

        Id = Guid.CreateVersion7();
        OwnerId = account.OwnerId;
        AccountId = account.Id;
        CategoryId = category.Id;
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        Amount = Guard.PositiveAmount(amount, "Kwota");
        Month = FirstDayOf(month);
        DueDate = dueDate;
        Comment = Guard.Optional(comment, CommentMaxLength, "Komentarz");
        Status = BudgetStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string OwnerId { get; private set; }
    public Guid AccountId { get; private set; }
    public Account? Account { get; private set; }
    public Guid CategoryId { get; private set; }
    public ExpenseCategory? Category { get; private set; }
    public string Name { get; private set; }
    public decimal Amount { get; private set; }

    /// <summary>Miesiąc planu — zawsze pierwszy dzień miesiąca.</summary>
    public DateOnly Month { get; private set; }

    /// <summary>Termin płatności (opcjonalny).</summary>
    public DateOnly? DueDate { get; private set; }

    public BudgetStatus Status { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>Faktyczny wydatek, gdy pozycja została zapłacona.</summary>
    public Expense? Expense { get; private set; }

    public void Update(
        ExpenseCategory category,
        string name,
        decimal amount,
        DateOnly month,
        DateOnly? dueDate,
        string? comment)
    {
        ArgumentNullException.ThrowIfNull(category);
        EnsureNotDone();
        if (category.AccountId != AccountId)
            throw new DomainException("Kategoria nie należy do wybranego konta.");
        if (category.IsArchived && category.Id != CategoryId)
            throw new DomainException("Kategoria jest zarchiwizowana.");

        CategoryId = category.Id;
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        Amount = Guard.PositiveAmount(amount, "Kwota");
        Month = FirstDayOf(month);
        DueDate = dueDate;
        Comment = Guard.Optional(comment, CommentMaxLength, "Komentarz");
    }

    /// <summary>Faktura otrzymana — gotowe do zapłaty.</summary>
    public void MarkReady()
    {
        if (Status != BudgetStatus.NotStarted)
            throw new DomainException("Tylko niezrealizowaną pozycję można oznaczyć jako gotową do zapłaty.");

        Status = BudgetStatus.InProgress;
    }

    /// <summary>Cofnięcie „gotowe do zapłaty” do „czeka na fakturę”.</summary>
    public void Reopen()
    {
        if (Status != BudgetStatus.InProgress)
            throw new DomainException("Cofnąć można tylko pozycję gotową do zapłaty.");

        Status = BudgetStatus.NotStarted;
    }

    /// <summary>Wywoływane wyłącznie przy tworzeniu wydatku z pozycji planu.</summary>
    internal void MarkDone()
    {
        EnsureNotDone();
        Status = BudgetStatus.Done;
    }

    internal void EnsureNotDone()
    {
        if (Status == BudgetStatus.Done)
            throw new DomainException("Pozycja planu jest już zapłacona.");
    }

    private static DateOnly FirstDayOf(DateOnly date) => new(date.Year, date.Month, 1);
}
