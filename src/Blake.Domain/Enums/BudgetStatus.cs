namespace Dynasty.Carrington.Blake.Domain.Enums;

/// <summary>Etap realizacji pozycji planu (np. leasing).</summary>
public enum BudgetStatus
{
    /// <summary>Zaplanowane, czeka na fakturę.</summary>
    NotStarted,

    /// <summary>Faktura otrzymana, gotowe do zapłaty.</summary>
    InProgress,

    /// <summary>Zapłacone — istnieje powiązany wydatek.</summary>
    Done
}
