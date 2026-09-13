using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Expenses;

using Microsoft.EntityFrameworkCore;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Repositories;

internal sealed class ExpenseRepository(BlakeDbContext database) : IExpenseRepository
{
    public async Task<IReadOnlyList<Expense>> GetForYear(int year, CancellationToken cancellationToken)
    {
        List<Expense> expenses = await database.Expenses
            .AsNoTracking()
            .Where(expense => expense.IncurredOn.Year == year)
            .ToListAsync(cancellationToken);

        return expenses;
    }
}