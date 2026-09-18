using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

namespace Dynasty.Carrington.Blake.Tests.Budgeting.Fakes;

internal sealed class NoOpUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task SaveChanges(CancellationToken cancellationToken)
    {
        SaveCount++;

        return Task.CompletedTask;
    }
}

internal sealed class FakeCurrentUser(string userId) : ICurrentUser
{
    public string UserId { get; } = userId;
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public override DateTimeOffset GetUtcNow()
    {
        return now;
    }
}

internal static class Items
{
    public const string Owner = "owner-1";
    public const string OtherOwner = "owner-2";

    public static PlannedItem Income(YearMonth month, decimal planned, string name = "Wypłata", string owner = Owner)
    {
        return new PlannedItem(Guid.NewGuid(), owner, month, PlannedItemKind.Income, name, planned);
    }

    public static PlannedItem Expense(YearMonth month, decimal planned, string name = "Mieszkanie", string owner = Owner)
    {
        return new PlannedItem(Guid.NewGuid(), owner, month, PlannedItemKind.Expense, name, planned);
    }

    public static PlannedItem Settled(this PlannedItem item, decimal actual)
    {
        item.Settle(actual, item.Month.FirstDay());

        return item;
    }
}
