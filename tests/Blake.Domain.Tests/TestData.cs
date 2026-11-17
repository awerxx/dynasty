using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Tests;

internal static class TestData
{
    public const string OwnerId = "owner-a";
    public static readonly DateOnly November = new(2026, 11, 1);

    public static Account Account(bool isCompany = false) =>
        new(OwnerId, isCompany ? "Firma" : "Bank 1", Currency.PLN, isCompany);

    public static ExpenseCategory Category(Account account, string name = "Leasing") => new(account, name);

    public static Budget Budget(Account account, ExpenseCategory category, decimal amount = 1500m) =>
        new(account, category, "Leasing auta", amount, November);
}
