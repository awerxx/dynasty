using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Core.Abstractions;

namespace Dynasty.Carrington.Blake.Application.Budgeting.SetAccountBalance;

public sealed class SetAccountBalanceCommandHandler(
    IAccountBalanceRepository balances,
    IUnitOfWork unitOfWork,
    ICurrentUser user,
    TimeProvider timeProvider)
    : ICommandHandler<SetAccountBalanceCommand>
{
    public async Task Handle(SetAccountBalanceCommand command, CancellationToken cancellationToken)
    {
        DateTimeOffset now = timeProvider.GetUtcNow();
        AccountBalance? balance = await balances.FindByOwner(user.UserId, cancellationToken);

        if (balance is null)
        {
            balances.Add(new AccountBalance(Guid.CreateVersion7(), user.UserId, command.Balance, now));
        }
        else
        {
            balance.Set(command.Balance, now);
        }

        await unitOfWork.SaveChanges(cancellationToken);
    }
}
