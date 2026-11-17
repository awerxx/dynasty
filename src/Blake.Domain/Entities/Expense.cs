using Dynasty.Carrington.Blake.Domain.Common;

namespace Dynasty.Carrington.Blake.Domain.Entities;

/// <summary>
/// Faktyczny wydatek. Powstaje z pozycji planu (<see cref="CreateFromBudget"/>) albo jako wydatek
/// nieplanowany (<see cref="CreateUnplanned"/>). Dla wydatku firmowego <see cref="Amount"/> to kwota brutto.
/// </summary>
public sealed class Expense
{
    public const int NameMaxLength = 200;
    public const int CommentMaxLength = 1000;
    public const int InvoiceNumberMaxLength = 100;

    private Expense()
    {
        OwnerId = null!;
        Name = null!;
    }

    private Expense(
        Account account,
        ExpenseCategory category,
        Guid? budgetId,
        string name,
        decimal amount,
        DateOnly spentOn,
        bool? isCompanyExpense,
        decimal? netAmount,
        string? invoiceNumber,
        string? comment,
        bool allowArchivedCategory)
    {
        account.EnsureActive();
        category.EnsureUsableFor(account, allowArchivedCategory);

        Id = Guid.CreateVersion7();
        OwnerId = account.OwnerId;
        AccountId = account.Id;
        CategoryId = category.Id;
        BudgetId = budgetId;
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        Comment = Guard.Optional(comment, CommentMaxLength, "Komentarz");
        SpentOn = spentOn;
        CreatedAt = DateTime.UtcNow;
        SetAmounts(amount, isCompanyExpense ?? account.IsCompany, netAmount, invoiceNumber);
    }

    public Guid Id { get; private set; }
    public string OwnerId { get; private set; }
    public Guid AccountId { get; private set; }
    public Account? Account { get; private set; }
    public Guid CategoryId { get; private set; }
    public ExpenseCategory? Category { get; private set; }

    /// <summary>Pozycja planu, z której powstał wydatek; <c>null</c> = wydatek nieplanowany.</summary>
    public Guid? BudgetId { get; private set; }
    public Budget? Budget { get; private set; }

    public string Name { get; private set; }
    public string? Comment { get; private set; }

    /// <summary>Faktyczna kwota (brutto dla wydatku firmowego).</summary>
    public decimal Amount { get; private set; }

    /// <summary>Data zapłaty — wyznacza miesiąc wydatku.</summary>
    public DateOnly SpentOn { get; private set; }

    public bool IsCompanyExpense { get; private set; }
    public decimal? NetAmount { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsPlanned => BudgetId is not null;

    /// <param name="isCompanyExpense"><c>null</c> = przyjmij typ konta (<see cref="Entities.Account.IsCompany"/>).</param>
    public static Expense CreateUnplanned(
        Account account,
        ExpenseCategory category,
        string name,
        decimal amount,
        DateOnly spentOn,
        bool? isCompanyExpense = null,
        decimal? netAmount = null,
        string? invoiceNumber = null,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);

        return new Expense(account, category, null, name, amount, spentOn,
            isCompanyExpense, netAmount, invoiceNumber, comment, allowArchivedCategory: false);
    }

    /// <summary>Zapłata pozycji planu: tworzy wydatek z faktyczną kwotą i oznacza pozycję jako <c>Done</c>.</summary>
    /// <param name="account">Konto pozycji planu.</param>
    /// <param name="category">Kategoria pozycji planu.</param>
    public static Expense CreateFromBudget(
        Budget budget,
        Account account,
        ExpenseCategory category,
        decimal amount,
        DateOnly spentOn,
        bool? isCompanyExpense = null,
        decimal? netAmount = null,
        string? invoiceNumber = null,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(budget);
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(category);

        budget.EnsureNotDone();
        if (budget.AccountId != account.Id)
            throw new DomainException("Wydatek musi być na tym samym koncie co pozycja planu.");
        if (budget.CategoryId != category.Id)
            throw new DomainException("Wydatek musi mieć tę samą kategorię co pozycja planu.");

        // Pozycja planu mogła powstać przed archiwizacją kategorii — nadal da się ją zapłacić.
        var expense = new Expense(account, category, budget.Id, budget.Name, amount, spentOn,
            isCompanyExpense, netAmount, invoiceNumber, comment ?? budget.Comment, allowArchivedCategory: true);

        budget.MarkDone();
        return expense;
    }

    /// <summary>Różnica plan − wykonanie: dodatnia = wydano mniej niż planowano, ujemna = więcej.</summary>
    public decimal? DifferenceFromBudget(Budget budget)
    {
        ArgumentNullException.ThrowIfNull(budget);
        return budget.Id == BudgetId ? budget.Amount - Amount : null;
    }

    public void Update(
        string name,
        decimal amount,
        DateOnly spentOn,
        bool isCompanyExpense,
        decimal? netAmount,
        string? invoiceNumber,
        string? comment)
    {
        Name = Guard.Name(name, NameMaxLength, "Nazwa");
        Comment = Guard.Optional(comment, CommentMaxLength, "Komentarz");
        SpentOn = spentOn;
        SetAmounts(amount, isCompanyExpense, netAmount, invoiceNumber);
    }

    private void SetAmounts(decimal amount, bool isCompanyExpense, decimal? netAmount, string? invoiceNumber)
    {
        var gross = Guard.PositiveAmount(amount, "Kwota");

        if (isCompanyExpense)
        {
            if (netAmount is null)
                throw new DomainException("Wydatek firmowy wymaga kwoty netto.");

            var net = Guard.PositiveAmount(netAmount.Value, "Kwota netto");
            if (net > gross)
                throw new DomainException("Kwota netto nie może być większa od kwoty brutto.");

            NetAmount = net;
            InvoiceNumber = Guard.Optional(invoiceNumber, InvoiceNumberMaxLength, "Numer faktury");
        }
        else
        {
            NetAmount = null;
            InvoiceNumber = null;
        }

        Amount = gross;
        IsCompanyExpense = isCompanyExpense;
    }
}
