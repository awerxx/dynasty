using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.GetAccountBalance;

public sealed class GetAccountBalanceQueryHandler(IAccountBalanceRepository balances, ICurrentUser user)
    : IQueryHandler<GetAccountBalanceQuery, AccountBalanceView>
{
    public async Task<AccountBalanceView> Handle(GetAccountBalanceQuery query, CancellationToken cancellationToken)
    {
        AccountBalance? balance = await balances.FindByOwner(user.UserId, cancellationToken);

        return balance is null
            ? new AccountBalanceView(false, 0m, null)
            : new AccountBalanceView(true, balance.Balance, balance.UpdatedAt);
    }
}
