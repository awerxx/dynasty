using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

internal sealed class InMemoryAccountBalanceRepository(params IEnumerable<AccountBalance> seed)
    : IAccountBalanceRepository
{
    public List<AccountBalance> Balances { get; } = seed.ToList();

    public Task<AccountBalance?> FindByOwner(string ownerId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Balances.SingleOrDefault(balance => balance.OwnerId == ownerId));
    }

    public void Add(AccountBalance balance)
    {
        Balances.Add(balance);
    }
}
