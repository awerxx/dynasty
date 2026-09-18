using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Application.Abstractions;

public interface IAccountBalanceRepository
{
    Task<AccountBalance?> FindByOwner(string ownerId, CancellationToken cancellationToken);

    void Add(AccountBalance balance);
}
