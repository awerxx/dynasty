
namespace Dynasty.Carrington.Blake.Domain.Entities;

public sealed class ExpenseCategory
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string OwnerId { get; set; }
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }


}
