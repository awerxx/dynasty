using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Persistence.Configurations;

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.OwnerId).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(ColumnLimits.Name);
        builder.Property(e => e.Comment).HasMaxLength(ColumnLimits.Comment);
        builder.Property(e => e.Amount).HasColumnType(ColumnLimits.Money);
        builder.Property(e => e.NetAmount).HasColumnType(ColumnLimits.Money);
        builder.Property(e => e.SpentOn).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        // Relacja z Account: AccountConfiguration (cascade).
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // NO ACTION: jak w BudgetConfiguration.
        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // Pozycja planu → co najwyżej jeden wydatek. Usunięcie pozycji zostawia wydatek jako nieplanowany.
        builder.HasOne(e => e.Budget)
            .WithOne(b => b.Expense)
            .HasForeignKey<Expense>(e => e.BudgetId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.OwnerId, e.SpentOn });
        builder.HasIndex(e => e.AccountId);
        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.BudgetId).IsUnique();
    }
}
