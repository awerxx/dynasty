using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Entities;

/// <summary>Konto użytkownika: bank, gotówka, oszczędności, konto walutowe, firmowe.</summary>
public sealed class Account
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private readonly List<ExpenseCategory> _categories = [];
    private readonly List<Budget> _budgets = [];
    private readonly List<Expense> _expenses = [];

    private Account()
    {
        OwnerId = null!;
        Name = null!;
    }

    public Account(string ownerId, string name, Currency currency, bool isCompany, string? description = null)
    {
        Id = Guid.CreateVersion7();
        OwnerId = Guard.OwnerId(ownerId);
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        Currency = currency;
        IsCompany = isCompany;
        Description = Guard.Optional(description, DescriptionMaxLength, "Opis");
        CreatedAt = DateTime.UtcNow;
        BalanceUpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public string OwnerId { get; private set; }
    public string Name { get; private set; }
    public Currency Currency { get; private set; }
    public bool IsCompany { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime BalanceUpdatedAt { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DisabledAt { get; private set; }

    public bool IsActive => DisabledAt is null;

    public IReadOnlyCollection<ExpenseCategory> Categories => _categories;
    public IReadOnlyCollection<Budget> Budgets => _budgets;
    public IReadOnlyCollection<Expense> Expenses => _expenses;

    public void Update(string name, bool isCompany, string? description)
    {
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        IsCompany = isCompany;
        Description = Guard.Optional(description, DescriptionMaxLength, "Opis");
    }

    /// <summary>Ręczne ustawienie salda (np. po sprawdzeniu w banku). Saldo może być ujemne.</summary>
    public void SetBalance(decimal balance)
    {
        Balance = Guard.TwoDecimalPlaces(balance, "Saldo");
        BalanceUpdatedAt = DateTime.UtcNow;
    }

    public void Disable()
    {
        DisabledAt ??= DateTime.UtcNow;
    }

    public void Enable()
    {
        DisabledAt = null;
    }

    internal void EnsureActive()
    {
        if (!IsActive)
            throw new DomainException("Konto jest wyłączone.");
    }
}
