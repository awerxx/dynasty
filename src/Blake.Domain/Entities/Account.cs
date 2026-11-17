using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string OwnerId { get;  set; }
    public string Name { get;  set; }
    public Currency Currency { get;  set; }
    public bool IsCompany { get;  set; }
    public decimal Balance { get;  set; }
    public DateTime BalanceUpdatedAt { get;  set; }
    public string? Description { get;  set; }
    public DateTime CreatedAt { get;  set; }
    public DateTime? DisabledAt { get;  set; }

    public List<ExpenseCategory> Categories { get;  set; } = new();
    public List<Budget> Budgets { get;  set; } = new();
    public List<Expense> Expenses { get;  set; } = new();
}
