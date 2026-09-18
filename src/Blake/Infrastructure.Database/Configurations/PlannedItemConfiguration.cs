using Dynasty.Carrington.Blake.Domain.Budgeting;
using Dynasty.Carrington.Blake.Infrastructure.Database.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Configurations;

internal sealed class PlannedItemConfiguration : IEntityTypeConfiguration<PlannedItem>
{
    /// <summary>
    ///     Length of an ASP.NET Core Identity user key.
    /// </summary>
    internal const int OwnerIdLength = 450;

    public void Configure(EntityTypeBuilder<PlannedItem> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).ValueGeneratedNever();

        builder.Property(item => item.OwnerId)
            .IsRequired()
            .HasMaxLength(OwnerIdLength);

        builder.Property(item => item.Month)
            .HasConversion(new YearMonthConverter());

        builder.Property(item => item.Kind)
            .HasConversion<string>()
            .HasMaxLength(16);

        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(PlannedItem.MaxNameLength);

        builder.Property(item => item.PlannedAmount).HasPrecision(18, 2);
        builder.Property(item => item.ActualAmount).HasPrecision(18, 2);

        builder.Ignore(item => item.IsSettled);
        builder.Ignore(item => item.EffectiveAmount);

        builder.HasIndex(item => new { item.OwnerId, item.Month });
    }
}
