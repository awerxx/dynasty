namespace Dynasty.Carrington.Blake.Application.Budgeting.GetAccountBalance;

public sealed record GetAccountBalanceQuery;

public sealed record AccountBalanceView(bool IsSet, decimal Balance, DateTimeOffset? UpdatedAt);
