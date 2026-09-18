using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

using Microsoft.EntityFrameworkCore;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Repositories;

internal sealed class AccountBalanceRepository(BlakeDbContext database) : IAccountBalanceRepository
{
    public Task<AccountBalance?> FindByOwner(string ownerId, CancellationToken cancellationToken)
    {
        return database.AccountBalances.FirstOrDefaultAsync(
            balance => balance.OwnerId == ownerId,
            cancellationToken);
    }

    public void Add(AccountBalance balance)
    {
        database.AccountBalances.Add(balance);
    }
}
