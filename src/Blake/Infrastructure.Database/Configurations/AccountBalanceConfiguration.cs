using Dynasty.Carrington.Blake.Domain.Budgeting;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Configurations;

internal sealed class AccountBalanceConfiguration : IEntityTypeConfiguration<AccountBalance>
{
    public void Configure(EntityTypeBuilder<AccountBalance> builder)
    {
        builder.HasKey(balance => balance.Id);
        builder.Property(balance => balance.Id).ValueGeneratedNever();

        builder.Property(balance => balance.OwnerId)
            .IsRequired()
            .HasMaxLength(PlannedItemConfiguration.OwnerIdLength);

        builder.Property(balance => balance.Balance).HasPrecision(18, 2);

        builder.HasIndex(balance => balance.OwnerId).IsUnique();
    }
}
