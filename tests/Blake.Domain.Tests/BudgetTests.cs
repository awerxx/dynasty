using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Tests;

public class BudgetTests
{
    [Test]
    public async Task Constructor_MidMonthDate_NormalizesToFirstDay()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);

        // Act
        var budget = new Budget(account, category, "Leasing", 1500m, new DateOnly(2026, 12, 17));

        // Assert
        await Assert.That(budget.Month).IsEqualTo(new DateOnly(2026, 12, 1));
        await Assert.That(budget.Status).IsEqualTo(BudgetStatus.NotStarted);
        await Assert.That(budget.OwnerId).IsEqualTo(account.OwnerId);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(10.005)]
    public async Task Constructor_InvalidAmount_Throws(double amount)
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);

        // Act
        var act = () => new Budget(account, category, "Leasing", (decimal)amount, TestData.November);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task Constructor_CategoryFromOtherAccount_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var otherCategory = TestData.Category(TestData.Account(isCompany: true));

        // Act
        var act = () => new Budget(account, otherCategory, "Leasing", 1500m, TestData.November);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task Constructor_ArchivedCategory_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        category.Archive();

        // Act
        var act = () => new Budget(account, category, "Leasing", 1500m, TestData.November);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task MarkReady_NotStarted_SetsInProgress()
    {
        // Arrange
        var account = TestData.Account();
        var budget = TestData.Budget(account, TestData.Category(account));

        // Act
        budget.MarkReady();

        // Assert
        await Assert.That(budget.Status).IsEqualTo(BudgetStatus.InProgress);
    }

    [Test]
    public async Task MarkReady_AlreadyInProgress_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var budget = TestData.Budget(account, TestData.Category(account));
        budget.MarkReady();

        // Act
        var act = () => budget.MarkReady();

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task Reopen_InProgress_SetsNotStarted()
    {
        // Arrange
        var account = TestData.Account();
        var budget = TestData.Budget(account, TestData.Category(account));
        budget.MarkReady();

        // Act
        budget.Reopen();

        // Assert
        await Assert.That(budget.Status).IsEqualTo(BudgetStatus.NotStarted);
    }

    [Test]
    public async Task Update_DoneBudget_Throws()
    {
        // Arrange
        var account = TestData.Account();
        var category = TestData.Category(account);
        var budget = TestData.Budget(account, category);
        Expense.CreateFromBudget(budget, account, category, 1500m, new DateOnly(2026, 11, 10));

        // Act
        var act = () => budget.Update(category, "Leasing", 1600m, TestData.November, null, null);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }
}
