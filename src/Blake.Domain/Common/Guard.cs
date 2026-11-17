namespace Dynasty.Carrington.Blake.Domain.Common;

internal static class Guard
{
    public static string Name(string? value, int maxLength, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"Pole „{field}” jest wymagane.");

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"Pole „{field}” może mieć maksymalnie {maxLength} znaków.");

        return trimmed;
    }

    public static string? Optional(string? value, int maxLength, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"Pole „{field}” może mieć maksymalnie {maxLength} znaków.");

        return trimmed;
    }

    public static decimal PositiveAmount(decimal value, string field)
    {
        if (value <= 0)
            throw new DomainException($"Pole „{field}” musi być większe od zera.");

        return TwoDecimalPlaces(value, field);
    }

    public static decimal TwoDecimalPlaces(decimal value, string field)
    {
        if (decimal.Round(value, 2) != value)
            throw new DomainException($"Pole „{field}” może mieć maksymalnie 2 miejsca po przecinku.");

        return value;
    }

    public static string OwnerId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("OwnerId is required.", nameof(value));

        return value;
    }
}
