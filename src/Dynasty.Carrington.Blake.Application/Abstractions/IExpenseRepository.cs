using Dynasty.Carrington.Blake.Domain.Expenses;

namespace Dynasty.Carrington.Blake.Application.Abstractions;

/// <summary>
///     Access to stored expenses. Implemented by the infrastructure layer.
/// </summary>
public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetForYear(int year, CancellationToken cancellationToken);
}