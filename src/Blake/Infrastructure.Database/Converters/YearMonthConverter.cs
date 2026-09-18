using Dynasty.Carrington.Blake.Domain.Budgeting;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dynasty.Carrington.Blake.Infrastructure.Database.Converters;

/// <summary>
///     Stores a <see cref="YearMonth" /> as its <c>yyyyMM</c> integer key, which sorts and compares
///     naturally in every provider.
/// </summary>
internal sealed class YearMonthConverter()
    : ValueConverter<YearMonth, int>(month => month.ToKey(), key => YearMonth.FromKey(key));
