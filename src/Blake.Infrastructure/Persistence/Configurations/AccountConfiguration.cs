using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.OwnerId).IsRequired();
        builder.Property(a => a.Name).IsRequired().HasMaxLength(ColumnLimits.Name);
        builder.Property(a => a.Description).HasMaxLength(ColumnLimits.Comment);
        builder.Property(a => a.Currency)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(ColumnLimits.Currency);
        builder.Property(a => a.Balance).HasColumnType(ColumnLimits.Money);
        builder.Property(a => a.BalanceUpdatedAt).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Categories)
            .WithOne(c => c.Account)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Budgets)
            .WithOne(b => b.Account)
            .HasForeignKey(b => b.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Expenses)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.OwnerId);
    }
}
