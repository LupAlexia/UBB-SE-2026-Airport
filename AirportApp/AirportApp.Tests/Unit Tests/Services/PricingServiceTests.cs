using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class PricingServiceTests
{
    private const float PricePerMinuteMultiplier = 1.25f;
    private const float MinimumFlightPrice = 40.0f;
    private const float ZeroPrice = 0f;
    private const float PercentageDivisor = 100.0f;
    private const float StandardFlightBasePrice = 100.0f;
    private const float FirstAddOnBasePrice = 50.0f;
    private const float SecondAddOnBasePrice = 25.0f;
    private const float ThirdAddOnBasePrice = 30.0f;
    private const float FlightDiscountPercentage = 10.0f;
    private const float AddOnDiscountPercentage = 20.0f;
    private const int ShortFlightDurationInMinutes = 10;
    private const int LongFlightDurationInMinutes = 100;
    private const int StandardFlightDurationInMinutes = 80;
    private const int DefaultAddOnId = 1;
    private const int SecondAddOnId = 2;
    private const int UnmatchedAddOnId = 99;
    private const float FloatComparisonTolerance = 0.001f;

    private PricingService _pricingService = null!;

    [SetUp]
    public void SetUp()
    {
        _pricingService = new PricingService(new MembershipPricingStrategyFactory());
    }

    private static Flight CreateFlightWithDurationInMinutes(int durationInMinutes)
    {
        var departureTime = new TimeOnly(10, 0);
        var arrivalTime = departureTime.AddMinutes(durationInMinutes);
        var route = new Route { DepartureTime = departureTime, ArrivalTime = arrivalTime };
        return new Flight { Route = route };
    }

    private static Customer CreateCustomerWithoutMembership() =>
        new Customer { Id = 1, Email = "test@test.com", Username = "testuser", PasswordHash = "hash", Membership = null };

    private static Customer CreateCustomerWithMembership(Membership membership) =>
        new Customer { Id = 1, Email = "test@test.com", Username = "testuser", PasswordHash = "hash", Membership = membership };

    private static Membership CreateMembershipWithFlightDiscount() =>
        new Membership { Id = 1, Name = "Premium", FlightDiscountPercentage = FlightDiscountPercentage };

    [Test]
    public async Task CalculateBasePriceAsync_NullFlight_ReturnsZero()
    {
        var result = await _pricingService.CalculateBasePriceAsync(null!);

        Assert.That(result, Is.EqualTo(ZeroPrice));
    }

    [Test]
    public async Task CalculateBasePriceAsync_FlightWithNullRoute_ReturnsZero()
    {
        var flight = new Flight { Route = null! };

        var result = await _pricingService.CalculateBasePriceAsync(flight);

        Assert.That(result, Is.EqualTo(ZeroPrice));
    }

    [Test]
    public async Task CalculateBasePriceAsync_ShortFlightBelowMinimum_ReturnsMinimumPrice()
    {
        var flight = CreateFlightWithDurationInMinutes(ShortFlightDurationInMinutes);

        var result = await _pricingService.CalculateBasePriceAsync(flight);

        Assert.That(result, Is.EqualTo(MinimumFlightPrice).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateBasePriceAsync_LongFlightAboveMinimum_ReturnsCalculatedPrice()
    {
        var flight = CreateFlightWithDurationInMinutes(LongFlightDurationInMinutes);
        float expectedPrice = LongFlightDurationInMinutes * PricePerMinuteMultiplier;

        var result = await _pricingService.CalculateBasePriceAsync(flight);

        Assert.That(result, Is.EqualTo(expectedPrice).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_NoMembershipNoAddOns_ReturnsBasePrice()
    {
        var ticket = new FlightTicket { Price = StandardFlightBasePrice, User = CreateCustomerWithoutMembership() };

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(StandardFlightBasePrice).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_NoMembershipWithAddOns_ReturnsBasePricePlusFullAddOnPrices()
    {
        var ticket = new FlightTicket
        {
            Price = StandardFlightBasePrice,
            User = CreateCustomerWithoutMembership(),
            SelectedAddOns = new List<AddOn>
            {
                new AddOn { BasePrice = FirstAddOnBasePrice },
                new AddOn { BasePrice = SecondAddOnBasePrice }
            }
        };
        float expectedTotal = StandardFlightBasePrice + FirstAddOnBasePrice + SecondAddOnBasePrice;

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(expectedTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_NullUserWithAddOns_ReturnsBasePricePlusFullAddOnPrices()
    {
        var ticket = new FlightTicket
        {
            Price = StandardFlightBasePrice,
            User = null,
            SelectedAddOns = new List<AddOn> { new AddOn { BasePrice = FirstAddOnBasePrice } }
        };
        float expectedTotal = StandardFlightBasePrice + FirstAddOnBasePrice;

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(expectedTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_MembershipWithFlightDiscount_AppliesFlightDiscountToBasePrice()
    {
        var membership = CreateMembershipWithFlightDiscount();
        var ticket = new FlightTicket
        {
            Price = StandardFlightBasePrice,
            User = CreateCustomerWithMembership(membership)
        };
        float expectedTotal = StandardFlightBasePrice - StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(expectedTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_MembershipWithMatchingAddOnDiscount_AppliesAddOnDiscount()
    {
        var membership = CreateMembershipWithFlightDiscount();
        var discountedAddOn = new AddOn { Id = DefaultAddOnId, Name = "Luggage", BasePrice = FirstAddOnBasePrice };
        var fullPriceAddOn = new AddOn { Id = SecondAddOnId, Name = "Meal", BasePrice = ThirdAddOnBasePrice };
        membership.AddonDiscounts = new List<MembershipAddonDiscount>
        {
            new MembershipAddonDiscount(membership, discountedAddOn, AddOnDiscountPercentage)
        };
        var ticket = new FlightTicket
        {
            Price = StandardFlightBasePrice,
            User = CreateCustomerWithMembership(membership),
            SelectedAddOns = new List<AddOn> { discountedAddOn, fullPriceAddOn }
        };
        float expectedFlightPrice = StandardFlightBasePrice - StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);
        float expectedDiscountedAddOn = FirstAddOnBasePrice - FirstAddOnBasePrice * (AddOnDiscountPercentage / PercentageDivisor);
        float expectedTotal = expectedFlightPrice + expectedDiscountedAddOn + ThirdAddOnBasePrice;

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(expectedTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_MembershipWithNullAddOnDiscounts_AppliesNoAddOnDiscount()
    {
        var membership = CreateMembershipWithFlightDiscount();
        membership.AddonDiscounts = null!;
        var addOn = new AddOn { Id = DefaultAddOnId, Name = "Luggage", BasePrice = FirstAddOnBasePrice };
        var ticket = new FlightTicket
        {
            Price = StandardFlightBasePrice,
            User = CreateCustomerWithMembership(membership),
            SelectedAddOns = new List<AddOn> { addOn }
        };
        float expectedFlightPrice = StandardFlightBasePrice - StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);
        float expectedTotal = expectedFlightPrice + FirstAddOnBasePrice;

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(expectedTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculateTotalPriceAsync_MembershipWithAddOnIdMismatch_AppliesNoAddOnDiscount()
    {
        var membership = CreateMembershipWithFlightDiscount();
        var selectedAddOn = new AddOn { Id = DefaultAddOnId, Name = "Luggage", BasePrice = FirstAddOnBasePrice };
        var unrelatedAddOn = new AddOn { Id = UnmatchedAddOnId, Name = "Unrelated", BasePrice = SecondAddOnBasePrice };
        membership.AddonDiscounts = new List<MembershipAddonDiscount>
        {
            new MembershipAddonDiscount(membership, unrelatedAddOn, AddOnDiscountPercentage)
        };
        var ticket = new FlightTicket
        {
            Price = StandardFlightBasePrice,
            User = CreateCustomerWithMembership(membership),
            SelectedAddOns = new List<AddOn> { selectedAddOn }
        };
        float expectedFlightPrice = StandardFlightBasePrice - StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);
        float expectedTotal = expectedFlightPrice + FirstAddOnBasePrice;

        var result = await _pricingService.CalculateTotalPriceAsync(ticket);

        Assert.That(result, Is.EqualTo(expectedTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_NullFlight_ReturnsDefaultBreakdown()
    {
        var customer = CreateCustomerWithoutMembership();

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(null!, customer, new List<FlightTicket>());

        Assert.That(breakdown.FinalTotal, Is.EqualTo(ZeroPrice));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_NullTickets_ReturnsDefaultBreakdown()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithoutMembership();

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, null!);

        Assert.That(breakdown.FinalTotal, Is.EqualTo(ZeroPrice));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_EmptyTickets_ReturnsDefaultBreakdown()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithoutMembership();

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, new List<FlightTicket>());

        Assert.That(breakdown.FinalTotal, Is.EqualTo(ZeroPrice));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_BasicUserWithSingleTicket_ReturnsCorrectBasePriceTotal()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithoutMembership();
        var tickets = new List<FlightTicket> { new FlightTicket() };

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.BasePriceTotal, Is.EqualTo(StandardFlightBasePrice).Within(FloatComparisonTolerance));
        Assert.That(breakdown.FinalTotal, Is.EqualTo(StandardFlightBasePrice).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_BasicUserWithSingleTicket_ReturnsZeroMembershipSavings()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithoutMembership();
        var tickets = new List<FlightTicket> { new FlightTicket() };

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.MembershipSavings, Is.EqualTo(ZeroPrice).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_MembershipUserWithSingleTicket_CalculatesCorrectFinalTotal()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithMembership(CreateMembershipWithFlightDiscount());
        var tickets = new List<FlightTicket> { new FlightTicket() };
        float expectedFinalTotal = StandardFlightBasePrice - StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.FinalTotal, Is.EqualTo(expectedFinalTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_MembershipUserWithSingleTicket_CalculatesMembershipSavingsCorrectly()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithMembership(CreateMembershipWithFlightDiscount());
        var tickets = new List<FlightTicket> { new FlightTicket() };
        float expectedSavings = StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.MembershipSavings, Is.EqualTo(expectedSavings).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_WithAddOns_IncludesAddOnsTotalCorrectly()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithoutMembership();
        var ticket = new FlightTicket
        {
            SelectedAddOns = new List<AddOn> { new AddOn { BasePrice = FirstAddOnBasePrice } }
        };
        var tickets = new List<FlightTicket> { ticket };

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.AddOnsTotal, Is.EqualTo(FirstAddOnBasePrice).Within(FloatComparisonTolerance));
        Assert.That(breakdown.FinalTotal, Is.EqualTo(StandardFlightBasePrice + FirstAddOnBasePrice).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_MultiplePassengers_MultipliesBasePriceTotalCorrectly()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithoutMembership();
        var tickets = new List<FlightTicket> { new FlightTicket(), new FlightTicket() };
        float expectedBasePriceTotal = StandardFlightBasePrice * tickets.Count;

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.BasePriceTotal, Is.EqualTo(expectedBasePriceTotal).Within(FloatComparisonTolerance));
        Assert.That(breakdown.FinalTotal, Is.EqualTo(expectedBasePriceTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_MembershipUserWithMultiplePassengers_CalculatesSavingsCorrectly()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var customer = CreateCustomerWithMembership(CreateMembershipWithFlightDiscount());
        var tickets = new List<FlightTicket> { new FlightTicket(), new FlightTicket() };
        float expectedSavings = StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor) * tickets.Count;
        float expectedFinalTotal = (StandardFlightBasePrice * tickets.Count) - expectedSavings;

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.MembershipSavings, Is.EqualTo(expectedSavings).Within(FloatComparisonTolerance));
        Assert.That(breakdown.FinalTotal, Is.EqualTo(expectedFinalTotal).Within(FloatComparisonTolerance));
    }

    [Test]
    public async Task CalculatePriceBreakdownAsync_ComplexDiscountScenario_CalculatesAllFieldsCorrectly()
    {
        var flight = CreateFlightWithDurationInMinutes(StandardFlightDurationInMinutes);
        var membership = CreateMembershipWithFlightDiscount();
        var addOn = new AddOn { Id = DefaultAddOnId, Name = "Luggage", BasePrice = FirstAddOnBasePrice };
        membership.AddonDiscounts = new List<MembershipAddonDiscount>
        {
            new MembershipAddonDiscount(membership, addOn, AddOnDiscountPercentage)
        };
        var customer = CreateCustomerWithMembership(membership);
        var tickets = new List<FlightTicket>
        {
            new FlightTicket { SelectedAddOns = new List<AddOn> { addOn } },
            new FlightTicket { SelectedAddOns = new List<AddOn> { addOn } }
        };
        float expectedBasePriceTotal = StandardFlightBasePrice * tickets.Count;
        float expectedAddOnsTotal = FirstAddOnBasePrice * tickets.Count;
        float discountedFlightPrice = StandardFlightBasePrice - StandardFlightBasePrice * (FlightDiscountPercentage / PercentageDivisor);
        float discountedAddOnPrice = FirstAddOnBasePrice - FirstAddOnBasePrice * (AddOnDiscountPercentage / PercentageDivisor);
        float expectedFinalTotal = (discountedFlightPrice + discountedAddOnPrice) * tickets.Count;
        float expectedSavings = (expectedBasePriceTotal + expectedAddOnsTotal) - expectedFinalTotal;

        var breakdown = await _pricingService.CalculatePriceBreakdownAsync(flight, customer, tickets);

        Assert.That(breakdown.BasePriceTotal, Is.EqualTo(expectedBasePriceTotal).Within(FloatComparisonTolerance));
        Assert.That(breakdown.AddOnsTotal, Is.EqualTo(expectedAddOnsTotal).Within(FloatComparisonTolerance));
        Assert.That(breakdown.MembershipSavings, Is.EqualTo(expectedSavings).Within(FloatComparisonTolerance));
        Assert.That(breakdown.FinalTotal, Is.EqualTo(expectedFinalTotal).Within(FloatComparisonTolerance));
    }
}
