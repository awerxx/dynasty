using Dynasty.Carrington.Blake.Domain.Common;
using Dynasty.Carrington.Blake.Domain.Entities;
using Dynasty.Carrington.Blake.Domain.Enums;

namespace Dynasty.Carrington.Blake.Domain.Tests;

public class AccountTests
{
    [Test]
    public async Task Constructor_ValidData_SetsIdAndCreatedAt()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var account = new Account(TestData.OwnerId, "  Konto EUR ", Currency.EUR, isCompany: false);

        // Assert
        await Assert.That(account.Id).IsNotEqualTo(Guid.Empty);
        await Assert.That(account.Id.Version).IsEqualTo(7);
        await Assert.That(account.Name).IsEqualTo("Konto EUR");
        await Assert.That(account.CreatedAt).IsGreaterThanOrEqualTo(before);
        await Assert.That(account.Balance).IsEqualTo(0m);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Constructor_EmptyName_Throws(string name)
    {
        // Arrange
        // Act
        var act = () => new Account(TestData.OwnerId, name, Currency.PLN, isCompany: false);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task SetBalance_NegativeValue_IsAllowed()
    {
        // Arrange
        var account = TestData.Account();

        // Act
        account.SetBalance(-120.50m);

        // Assert
        await Assert.That(account.Balance).IsEqualTo(-120.50m);
    }

    [Test]
    public async Task SetBalance_MoreThanTwoDecimals_Throws()
    {
        // Arrange
        var account = TestData.Account();

        // Act
        var act = () => account.SetBalance(10.001m);

        // Assert
        await Assert.That(act).Throws<DomainException>();
    }

    [Test]
    public async Task Disable_ActiveAccount_SetsDisabledAt()
    {
        // Arrange
        var account = TestData.Account();

        // Act
        account.Disable();

        // Assert
        await Assert.That(account.DisabledAt).IsNotNull();
        await Assert.That(account.IsActive).IsFalse();
    }

    [Test]
    public async Task Enable_DisabledAccount_ClearsDisabledAt()
    {
        // Arrange
        var account = TestData.Account();
        account.Disable();

        // Act
        account.Enable();

        // Assert
        await Assert.That(account.DisabledAt).IsNull();
    }
}
