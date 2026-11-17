using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Entities;

namespace Dynasty.Carrington.Blake.Domain.Tests;

public class ExpenseCategoryTests
{
    [Test]
    public async Task Constructor_ValidAccount_CopiesOwnerAndAccount()
    {
        // Arrange
        var account = TestData.Account();

        // Act
        var category = new ExpenseCategory(account, "Podatki");

        // Assert
        await Assert.That(category.OwnerId).IsEqualTo(account.OwnerId);
        await Assert.That(category.AccountId).IsEqualTo(account.Id);
    }

    [Test]
    public async Task Constructor_DisabledAccount_Throws()
    {
        // Arrange
        var account = TestData.Account();
        account.Disable();

        // Act
        var act = () => new ExpenseCategory(account, "Podatki");

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task Archive_ActiveCategory_SetsArchivedAt()
    {
        // Arrange
        var category = TestData.Category(TestData.Account());

        // Act
        category.Archive();

        // Assert
        await Assert.That(category.IsArchived).IsTrue();
    }
}
