using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Domain.Budgeting;

using Microsoft.EntityFrameworkCore;

namespace Dynasty.Carrington.Blake.Infrastructure.Database;

public sealed class BlakeDbContext(DbContextOptions<BlakeDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<PlannedItem> PlannedItems => Set<PlannedItem>();

    public DbSet<AccountBalance> AccountBalances => Set<AccountBalance>();

    Task IUnitOfWork.SaveChanges(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlakeDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
