using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Entities;

public sealed class Budget
{

    public Guid Id { get; set; }
    public string OwnerId { get;  set; }
    public Guid AccountId { get;  set; }
    public Account? Account { get;  set; }
    public Guid CategoryId { get;  set; }
    public ExpenseCategory? Category { get;  set; }
    public string Name { get;  set; }
    public decimal Amount { get;  set; }

    public DateOnly Month { get;  set; }

    public DateOnly? DueDate { get;  set; }

    public BudgetStatus Status { get;  set; }
    public string? Comment { get;  set; }
    public DateTime CreatedAt { get;  set; }

    public Expense? Expense { get;  set; }

}
