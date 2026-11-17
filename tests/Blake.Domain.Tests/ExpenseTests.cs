using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Tests;

public class ExpenseTests
{
    private static readonly DateOnly PaidOn = new(2026, 11, 10);

    [Test]
    public async Task CreateUnplanned_ValidData_HasNoBudget()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account, "Jedzenie");

        // Act
        var expense = Expense.CreateUnplanned(account, category, "Pizza", 59.99m, PaidOn);

        // Assert
        await Assert.That(expense.BudgetId).IsNull();
        await Assert.That(expense.IsPlanned).IsFalse();
        await Assert.That(expense.OwnerId).IsEqualTo(account.OwnerId);
        await Assert.That(expense.IsCompanyExpense).IsFalse();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-5)]
    [Arguments(1.999)]
    public async Task CreateUnplanned_InvalidAmount_Throws(double amount)
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);

        // Act
        var act = () => Expense.CreateUnplanned(account, category, "Pizza", (decimal)amount, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task CreateUnplanned_CategoryFromOtherAccount_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var otherCategory = TestData.Category(TestData.Account(isCompany: true));

        // Act
        var act = () => Expense.CreateUnplanned(account, otherCategory, "Pizza", 50m, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task CreateUnplanned_DisabledAccount_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        account.Disable();

        // Act
        var act = () => Expense.CreateUnplanned(account, category, "Pizza", 50m, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task CreateUnplanned_CompanyAccountWithoutNet_Throws()
    {
        // Arrange
        var account = TestData.Account(isCompany: true);
        var category = TestData.Category(account, "Paliwo");

        // Act
        var act = () => Expense.CreateUnplanned(account, category, "Tankowanie", 246m, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task CreateUnplanned_NetGreaterThanGross_Throws()
    {
        // Arrange
        var account = TestData.Account(isCompany: true);
        var category = TestData.Category(account, "Paliwo");

        // Act
        var act = () => Expense.CreateUnplanned(account, category, "Tankowanie", 200m, PaidOn, netAmount: 246m);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task CreateUnplanned_CompanyExpenseWithNet_StoresInvoice()
    {
        // Arrange
        var account = TestData.Account(isCompany: true);
        var category = TestData.Category(account, "Paliwo");

        // Act
        var expense = Expense.CreateUnplanned(account, category, "Tankowanie", 246m, PaidOn,
            netAmount: 200m, invoiceNumber: "FV/11/2026");

        // Assert
        await Assert.That(expense.IsCompanyExpense).IsTrue();
        await Assert.That(expense.NetAmount).IsEqualTo(200m);
        await Assert.That(expense.InvoiceNumber).IsEqualTo("FV/11/2026");
    }

    [Test]
    public async Task CreateUnplanned_PersonalExpenseOnCompanyAccount_DropsNet()
    {
        // Arrange
        var account = TestData.Account(isCompany: true);
        var category = TestData.Category(account, "Inne");

        // Act
        var expense = Expense.CreateUnplanned(account, category, "Kawa", 15m, PaidOn,
            isCompanyExpense: false, netAmount: 12m);

        // Assert
        await Assert.That(expense.IsCompanyExpense).IsFalse();
        await Assert.That(expense.NetAmount).IsNull();
    }

    [Test]
    public async Task CreateFromBudget_ValidBudget_LinksAndMarksDone()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        var budget = TestData.Budget(account, category, amount: 1500m);
        budget.MarkReady();

        // Act
        var expense = Expense.CreateFromBudget(budget, account, category, 1580m, PaidOn);

        // Assert
        await Assert.That(expense.BudgetId).IsEqualTo(budget.Id);
        await Assert.That(expense.Name).IsEqualTo(budget.Name);
        await Assert.That(budget.Status).IsEqualTo(BudgetStatus.Done);
        await Assert.That(expense.DifferenceFromBudget(budget)).IsEqualTo(-80m);
    }

    [Test]
    public async Task CreateFromBudget_BudgetAlreadyDone_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        var budget = TestData.Budget(account, category);
        Expense.CreateFromBudget(budget, account, category, 1500m, PaidOn);

        // Act
        var act = () => Expense.CreateFromBudget(budget, account, category, 1500m, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task CreateFromBudget_OtherAccount_ThrowsAndKeepsStatus()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        var budget = TestData.Budget(account, category);
        var otherAccount = TestData.Account();
        var otherCategory = TestData.Category(otherAccount);

        // Act
        var act = () => Expense.CreateFromBudget(budget, otherAccount, otherCategory, 1500m, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
        await Assert.That(budget.Status).IsEqualTo(BudgetStatus.NotStarted);
    }

    [Test]
    public async Task CreateFromBudget_InvalidAmount_KeepsStatus()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        var budget = TestData.Budget(account, category);

        // Act
        var act = () => Expense.CreateFromBudget(budget, account, category, 0m, PaidOn);

        // Assert
        await Assert.That(act).Throws<DomainException>();
        await Assert.That(budget.Status).IsEqualTo(BudgetStatus.NotStarted);
    }

    [Test]
    public async Task CreateFromBudget_CategoryArchivedAfterPlanning_Succeeds()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        var budget = TestData.Budget(account, category);
        category.Archive();

        // Act
        var expense = Expense.CreateFromBudget(budget, account, category, 1500m, PaidOn);

        // Assert
        await Assert.That(expense.CategoryId).IsEqualTo(category.Id);
    }
}
