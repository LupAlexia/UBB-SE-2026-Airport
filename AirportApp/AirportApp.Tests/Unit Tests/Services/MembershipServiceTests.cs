using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class MembershipServiceTests
{
    private const int DefaultUserId = 1;
    private const int DefaultMembershipId = 1;
    private const int SecondMembershipId = 2;
    private const float DefaultDiscountPercentage = 20.0f;
    private const string SuccessfulPurchaseMessage = "Your membership purchase was completed successfully.";
    private const string FailedPurchaseMessage = "Membership purchase could not be completed. Please try again.";

    private ICustomerRepository _customerRepository = null!;
    private IMembershipRepository _membershipRepository = null!;
    private MembershipService _membershipService = null!;

    [SetUp]
    public void SetUp()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _membershipRepository = Substitute.For<IMembershipRepository>();
        _membershipService = new MembershipService(_customerRepository, _membershipRepository);
    }

    [TearDown]
    public void TearDown()
    {
        UserSession.Clear();
    }

    private static Membership CreateMembership(int membershipId) =>
        new Membership { Id = membershipId, Name = "Membership " + membershipId, FlightDiscountPercentage = DefaultDiscountPercentage };

    private static MembershipAddonDiscount CreateAddonDiscount(Membership membership) =>
        new MembershipAddonDiscount(membership, new AddOn { Id = 1, Name = "Luggage" }, DefaultDiscountPercentage);

    [Test]
    public async Task GetAllMembershipsAsync_MultipleMemberships_ReturnsAllMemberships()
    {
        var firstMembership = CreateMembership(DefaultMembershipId);
        var secondMembership = CreateMembership(SecondMembershipId);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { firstMembership, secondMembership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(Arg.Any<int>()).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        var result = (await _membershipService.GetAllMembershipsAsync()).ToList();

        Assert.That(result, Is.EquivalentTo(new[] { firstMembership, secondMembership }));
    }

    [Test]
    public async Task GetAllMembershipsAsync_SingleMembership_PopulatesAddonDiscounts()
    {
        var membership = CreateMembership(DefaultMembershipId);
        var discount = CreateAddonDiscount(membership);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { membership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount> { discount }));

        var result = (await _membershipService.GetAllMembershipsAsync()).ToList();

        Assert.That(result[0].AddonDiscounts, Contains.Item(discount));
    }

    [Test]
    public async Task GetAllMembershipsAsync_SingleMembership_CallsGetDiscountsByMembershipId()
    {
        var membership = CreateMembership(DefaultMembershipId);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { membership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        await _membershipService.GetAllMembershipsAsync();

        await _membershipRepository.Received(1).GetDiscountsByMembershipIdAsync(DefaultMembershipId);
    }

    [Test]
    public async Task GetAllMembershipsAsync_EmptyMembershipList_ReturnsEmptyCollection()
    {
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership>()));

        var result = await _membershipService.GetAllMembershipsAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllMembershipsAsync_EmptyMembershipList_DoesNotCallGetDiscounts()
    {
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership>()));

        await _membershipService.GetAllMembershipsAsync();

        await _membershipRepository.DidNotReceive().GetDiscountsByMembershipIdAsync(Arg.Any<int>());
    }

    [Test]
    public async Task GetAllMembershipsAsync_MultipleMemberships_FillsAddonDiscountsForFirstMembership()
    {
        var firstMembership = CreateMembership(DefaultMembershipId);
        var secondMembership = CreateMembership(SecondMembershipId);
        var firstDiscount = CreateAddonDiscount(firstMembership);
        var secondDiscount = CreateAddonDiscount(secondMembership);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { firstMembership, secondMembership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount> { firstDiscount }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(SecondMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount> { secondDiscount }));

        var result = (await _membershipService.GetAllMembershipsAsync()).ToList();

        Assert.That(result[0].AddonDiscounts, Contains.Item(firstDiscount));
    }

    [Test]
    public async Task GetAllMembershipsAsync_MultipleMemberships_FillsAddonDiscountsForSecondMembership()
    {
        var firstMembership = CreateMembership(DefaultMembershipId);
        var secondMembership = CreateMembership(SecondMembershipId);
        var firstDiscount = CreateAddonDiscount(firstMembership);
        var secondDiscount = CreateAddonDiscount(secondMembership);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { firstMembership, secondMembership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount> { firstDiscount }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(SecondMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount> { secondDiscount }));

        var result = (await _membershipService.GetAllMembershipsAsync()).ToList();

        Assert.That(result[1].AddonDiscounts, Contains.Item(secondDiscount));
    }

    [Test]
    public async Task GetAllMembershipsAsync_MultipleMemberships_CallsGetDiscountsForFirstMembership()
    {
        var firstMembership = CreateMembership(DefaultMembershipId);
        var secondMembership = CreateMembership(SecondMembershipId);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { firstMembership, secondMembership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(Arg.Any<int>()).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        await _membershipService.GetAllMembershipsAsync();

        await _membershipRepository.Received(1).GetDiscountsByMembershipIdAsync(DefaultMembershipId);
    }

    [Test]
    public async Task GetAllMembershipsAsync_MultipleMemberships_CallsGetDiscountsForSecondMembership()
    {
        var firstMembership = CreateMembership(DefaultMembershipId);
        var secondMembership = CreateMembership(SecondMembershipId);
        _membershipRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Membership>>(new List<Membership> { firstMembership, secondMembership }));
        _membershipRepository.GetDiscountsByMembershipIdAsync(Arg.Any<int>()).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        await _membershipService.GetAllMembershipsAsync();

        await _membershipRepository.Received(1).GetDiscountsByMembershipIdAsync(SecondMembershipId);
    }

    [Test]
    public async Task UpgradeUserMembershipAsync_ValidUserAndMembership_CallsUpdateMembershipOnUserRepository()
    {
        var membership = CreateMembership(DefaultMembershipId);
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        await _membershipService.UpgradeUserMembershipAsync(DefaultUserId, DefaultMembershipId);

        await _customerRepository.Received(1).UpdateMembershipAsync(DefaultUserId, DefaultMembershipId);
    }

    [Test]
    public async Task UpgradeUserMembershipAsync_MembershipFound_ReturnsMembership()
    {
        var membership = CreateMembership(DefaultMembershipId);
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        var result = await _membershipService.UpgradeUserMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result, Is.EqualTo(membership));
    }

    [Test]
    public async Task UpgradeUserMembershipAsync_MembershipFound_FillsAddonDiscountsOnReturnedMembership()
    {
        var membership = CreateMembership(DefaultMembershipId);
        var discount = CreateAddonDiscount(membership);
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount> { discount }));

        var result = await _membershipService.UpgradeUserMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result!.AddonDiscounts, Contains.Item(discount));
    }

    [Test]
    public async Task UpgradeUserMembershipAsync_MembershipNotFound_ReturnsNull()
    {
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(null));

        var result = await _membershipService.UpgradeUserMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpgradeUserMembershipAsync_MembershipNotFound_DoesNotCallGetDiscounts()
    {
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(null));

        await _membershipService.UpgradeUserMembershipAsync(DefaultUserId, DefaultMembershipId);

        await _membershipRepository.DidNotReceive().GetDiscountsByMembershipIdAsync(Arg.Any<int>());
    }

    [Test]
    public async Task PurchaseMembershipAsync_SuccessfulUpgrade_ReturnsSucceededTrue()
    {
        var membership = CreateMembership(DefaultMembershipId);
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        var result = await _membershipService.PurchaseMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public async Task PurchaseMembershipAsync_SuccessfulUpgrade_ReturnsSuccessMessage()
    {
        var membership = CreateMembership(DefaultMembershipId);
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        var result = await _membershipService.PurchaseMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result.Message, Is.EqualTo(SuccessfulPurchaseMessage));
    }

    [Test]
    public async Task PurchaseMembershipAsync_SuccessfulUpgradeWithCurrentUserInSession_UpdatesUserSessionMembership()
    {
        var membership = CreateMembership(DefaultMembershipId);
        UserSession.CurrentUser = new Customer { Id = DefaultUserId, Email = "user@test.com" };
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        await _membershipService.PurchaseMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(UserSession.CurrentUser.Membership, Is.EqualTo(membership));
    }

    [Test]
    public async Task PurchaseMembershipAsync_SuccessfulUpgradeWithNoCurrentUserInSession_ReturnsSucceededTrue()
    {
        var membership = CreateMembership(DefaultMembershipId);
        UserSession.CurrentUser = null;
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId).Returns(Task.CompletedTask);
        _membershipRepository.GetByIdAsync(DefaultMembershipId).Returns(Task.FromResult<Membership?>(membership));
        _membershipRepository.GetDiscountsByMembershipIdAsync(DefaultMembershipId).Returns(Task.FromResult<IEnumerable<MembershipAddonDiscount>>(new List<MembershipAddonDiscount>()));

        var result = await _membershipService.PurchaseMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public async Task PurchaseMembershipAsync_RepositoryThrowsException_ReturnsSucceededFalse()
    {
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId)
            .Returns(Task.FromException(new InvalidOperationException("Repository failure")));

        var result = await _membershipService.PurchaseMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result.Succeeded, Is.False);
    }

    [Test]
    public async Task PurchaseMembershipAsync_RepositoryThrowsException_ReturnsFailureMessage()
    {
        _customerRepository.UpdateMembershipAsync(DefaultUserId, DefaultMembershipId)
            .Returns(Task.FromException(new InvalidOperationException("Repository failure")));

        var result = await _membershipService.PurchaseMembershipAsync(DefaultUserId, DefaultMembershipId);

        Assert.That(result.Message, Is.EqualTo(FailedPurchaseMessage));
    }
}
