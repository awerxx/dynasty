using Dynasty.Carrington.Blake.Domain.Expenses;

using Microsoft.EntityFrameworkCore;

namespace Dynasty.Carrington.Blake.Infrastructure.Database;

public sealed class BlakeDbContext(DbContextOptions<BlakeDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlakeDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}