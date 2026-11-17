namespace Dynasty.Carrington.Blake.Domain.Common;

/// <summary>Naruszenie reguły biznesowej. Komunikat jest po polsku i może trafić do UI.</summary>
public sealed class DomainException(string message) : Exception(message);
