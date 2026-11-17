using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Persistence.Configurations;

public sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.OwnerId).IsRequired();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(ColumnLimits.Name);
        builder.Property(c => c.CreatedAt).IsRequired();

        // Relacja z Account: AccountConfiguration (cascade).
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => new { c.AccountId, c.Name }).IsUnique();
    }
}
