using Dynasty.Carrington.Blake.Domain.Expenses;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Configurations;

internal sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(expense => expense.Id);

        builder.Property(expense => expense.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(expense => expense.Amount)
            .HasPrecision(18, 2);

        builder.Property(expense => expense.Category)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(expense => expense.IncurredOn);
    }
}