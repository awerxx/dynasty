using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Persistence.Configurations;

public sealed class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.OwnerId).IsRequired();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(ColumnLimits.Name);
        builder.Property(b => b.Comment).HasMaxLength(ColumnLimits.Comment);
        builder.Property(b => b.Amount).HasColumnType(ColumnLimits.Money);
        builder.Property(b => b.Month).IsRequired();
        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(ColumnLimits.Enum);
        builder.Property(b => b.CreatedAt).IsRequired();

        // Relacja z Account: AccountConfiguration (cascade); z Expense: ExpenseConfiguration.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // NO ACTION zamiast RESTRICT: blokuje usunięcie używanej kategorii,
        // ale nie psuje kaskady przy usuwaniu całego konta (sprawdzane na końcu polecenia).
        builder.HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(b => new { b.OwnerId, b.Month });
        builder.HasIndex(b => b.AccountId);
        builder.HasIndex(b => b.CategoryId);
    }
}
