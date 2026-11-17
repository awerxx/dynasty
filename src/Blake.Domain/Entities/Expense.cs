
namespace Dynasty.Carrington.Blake.Domain.Entities;


public sealed class Expense
{

    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string OwnerId { get;  set; }
    public Guid AccountId { get;  set; }
    public Account? Account { get;  set; }
    public Guid CategoryId { get;  set; }
    public ExpenseCategory? Category { get;  set; }

    public Guid? BudgetId { get;  set; }
    public Budget? Budget { get;  set; }

    public string Name { get;  set; }
    public string? Comment { get;  set; }

    public decimal Amount { get;  set; }

    public DateOnly SpentOn { get;  set; }

    public bool IsCompanyExpense { get;  set; }
    public decimal? NetAmount { get;  set; }
   
    public DateTime CreatedAt { get;  set; }

    public bool IsPlanned => BudgetId is not null;

    
}
