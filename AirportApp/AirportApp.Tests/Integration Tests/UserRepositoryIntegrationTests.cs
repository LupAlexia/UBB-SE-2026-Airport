using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Tests.IntegrationTests;

[TestFixture]
public class UserRepositoryIntegrationTests : RepositoryTestBase
{
    private UserRepository _userRepository = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _userRepository = new UserRepository(DbContext);
    }

    [Test]
    public async Task GetByIdAsync_WithExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            FullName = "Integration User",
            EmailAddress = "integration@repo.com"
        };
        int userId = await _userRepository.AddAsync(user);

        // Act
        var retrievedUser = await _userRepository.GetByIdAsync(userId);

        // Assert
        Assert.That(retrievedUser?.FullName, Is.EqualTo("Integration User"));
    }

    [Test]
    public void GetByIdAsync_WithNonExistentUser_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _userRepository.GetByIdAsync(999999);
        });
    }
}
