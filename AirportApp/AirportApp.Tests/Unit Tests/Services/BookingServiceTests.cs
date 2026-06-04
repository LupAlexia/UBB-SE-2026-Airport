using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class BookingServiceTests
{
    private const int DefaultFlightId = 1;
    private const float DefaultBasePrice = 150.0f;
    private const float StandardTicketPrice = 100.0f;
    private const int LargeFlightCapacity = 200;
    private const int HighOccupiedSeats = 195;
    private const int ModerateOccupiedSeats = 100;
    private const int ZeroRequestedPassengers = 0;
    private const int NormalRequestedPassengers = 5;
    private const int ExpectedMaximumPassengers = 5;
    private const string ActiveStatus = "Active";
    private const string Seat1A = "1A";
    private const string Seat1B = "1B";
    private const string Seat2B = "2B";
    private const int ExactMultipleCapacity = 180;
    private const int PartialMultipleCapacity = 182;
    private const int MinimumFlightCapacity = 6;
    private const int ExpectedExactMultipleRows = 30;
    private const int ExpectedPartialMultipleRows = 31;
    private const int ExpectedMinimumCapacityRows = 1;
    private const int ExpectedExactMultipleLayoutCount = 180;
    private const int ExpectedPartialMultipleLayoutCount = 186;

    private IFlightTicketRepository _ticketRepository = null!;
    private IAddOnRepository _addOnRepository = null!;
    private BookingService _bookingService = null!;

    [SetUp]
    public void SetUp()
    {
        _ticketRepository = Substitute.For<IFlightTicketRepository>();
        _addOnRepository = Substitute.For<IAddOnRepository>();
        _bookingService = new BookingService(_ticketRepository, _addOnRepository);
    }

    [TearDown]
    public void TearDown()
    {
        UserSession.Clear();
    }

    [Test]
    public void CreateTickets_ValidPassenger_AssignsPassengerFirstName()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var passenger = new PassengerData { FirstName = "Ionel", LastName = "Gheorghe", SelectedSeat = Seat1A, Email = "ionel@test.com" };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].PassengerFirstName, Is.EqualTo("Ionel"));
    }

    [Test]
    public void CreateTickets_ValidPassenger_AssignsPassengerLastName()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var passenger = new PassengerData { FirstName = "Ionel", LastName = "Gheorghe", SelectedSeat = Seat1A, Email = "ionel@test.com" };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].PassengerLastName, Is.EqualTo("Gheorghe"));
    }

    [Test]
    public void CreateTickets_ValidPassenger_AssignsBasePrice()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var passenger = new PassengerData { FirstName = "Ana", LastName = "Pop", SelectedSeat = Seat1A };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].Price, Is.EqualTo(DefaultBasePrice));
    }

    [Test]
    public void CreateTickets_ValidPassenger_AssignsActiveStatus()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var passenger = new PassengerData { FirstName = "Ana", LastName = "Pop", SelectedSeat = Seat1A };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].Status, Is.EqualTo(ActiveStatus));
    }

    [Test]
    public void CreateTickets_MultiplePassengers_CreatesOneTicketPerPassenger()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var passengers = new List<PassengerData>
        {
            new PassengerData { FirstName = "Ana", LastName = "Pop", SelectedSeat = Seat1A },
            new PassengerData { FirstName = "Ion", LastName = "Ion", SelectedSeat = Seat1B }
        };

        var tickets = _bookingService.CreateTickets(flight, user, passengers, DefaultBasePrice);

        Assert.That(tickets.Count, Is.EqualTo(2));
    }

    [Test]
    public void CreateTickets_ValidPassenger_AssignsFlight()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 42, Email = "user@test.com" };
        var passenger = new PassengerData { FirstName = "Maria", LastName = "Dan", SelectedSeat = Seat1A };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].Flight, Is.EqualTo(flight));
    }

    [Test]
    public void CreateTickets_ValidPassenger_AssignsUser()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 42, Email = "user@test.com" };
        var passenger = new PassengerData { FirstName = "Maria", LastName = "Dan", SelectedSeat = Seat1A };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].User, Is.EqualTo(user));
    }

    [Test]
    public void CreateTickets_PassengerWithAddOns_CopiesAddOnsToTicket()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        var addOn = new AddOn { Id = 1, Name = "Priority Boarding" };
        var passenger = new PassengerData
        {
            FirstName = "Ana",
            LastName = "Pop",
            SelectedSeat = Seat1A,
            SelectedAddOns = new List<AddOn> { addOn }
        };

        var tickets = _bookingService.CreateTickets(flight, user, new List<PassengerData> { passenger }, DefaultBasePrice);

        Assert.That(tickets[0].SelectedAddOns, Contains.Item(addOn));
    }

    [Test]
    public async Task ValidatePassengersAsync_EmptyList_ReturnsError()
    {
        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData>());

        Assert.That(error, Is.EqualTo("At least one passenger is required."));
    }

    [Test]
    public async Task ValidatePassengersAsync_NullList_ReturnsError()
    {
        var error = await _bookingService.ValidatePassengersAsync(null!);

        Assert.That(error, Is.EqualTo("At least one passenger is required."));
    }

    [Test]
    public async Task ValidatePassengersAsync_MissingFirstName_ReturnsError()
    {
        var passenger = new PassengerData { LastName = "Pop", SelectedSeat = Seat1A };

        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData> { passenger });

        Assert.That(error, Does.Contain("first name is required"));
    }

    [Test]
    public async Task ValidatePassengersAsync_MissingLastName_ReturnsError()
    {
        var passenger = new PassengerData { FirstName = "Vasile", SelectedSeat = Seat1A };

        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData> { passenger });

        Assert.That(error, Does.Contain("last name is required"));
    }

    [Test]
    public async Task ValidatePassengersAsync_NoSeatSelected_ReturnsError()
    {
        var passenger = new PassengerData { FirstName = "Vasile", LastName = "Pop", SelectedSeat = string.Empty };

        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData> { passenger });

        Assert.That(error, Does.Contain("please select a seat"));
    }

    [Test]
    public async Task ValidatePassengersAsync_InvalidEmail_ReturnsError()
    {
        var passenger = new PassengerData { FirstName = "Ion", LastName = "Pop", SelectedSeat = Seat1A, Email = "not-an-email" };

        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData> { passenger });

        Assert.That(error, Does.Contain("email format is invalid"));
    }

    [Test]
    public async Task ValidatePassengersAsync_ValidEmail_ReturnsEmpty()
    {
        var passenger = new PassengerData { FirstName = "Ion", LastName = "Pop", SelectedSeat = Seat1A, Email = "ion.pop@gmail.com" };

        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData> { passenger });

        Assert.That(error, Is.Empty);
    }

    [Test]
    public async Task ValidatePassengersAsync_EmptyEmail_ReturnsEmpty()
    {
        var passenger = new PassengerData { FirstName = "Ion", LastName = "Pop", SelectedSeat = Seat1A, Email = string.Empty };

        var error = await _bookingService.ValidatePassengersAsync(new List<PassengerData> { passenger });

        Assert.That(error, Is.Empty);
    }

    [Test]
    public async Task CalculateMaxPassengersAsync_NoRequestedPassengers_ReturnsRemainingCapacity()
    {
        var maximumPassengerCount = await _bookingService.CalculateMaxPassengersAsync(LargeFlightCapacity, HighOccupiedSeats, ZeroRequestedPassengers);

        Assert.That(maximumPassengerCount, Is.EqualTo(ExpectedMaximumPassengers));
    }

    [Test]
    public async Task CalculateMaxPassengersAsync_RequestedLessThanRemaining_ReturnsRequested()
    {
        var maximumPassengerCount = await _bookingService.CalculateMaxPassengersAsync(LargeFlightCapacity, ModerateOccupiedSeats, NormalRequestedPassengers);

        Assert.That(maximumPassengerCount, Is.EqualTo(NormalRequestedPassengers));
    }

    [Test]
    public async Task CalculateMaxPassengersAsync_RequestedExceedsRemaining_CapsAtRemaining()
    {
        var maximumPassengerCount = await _bookingService.CalculateMaxPassengersAsync(LargeFlightCapacity, HighOccupiedSeats, 10);

        Assert.That(maximumPassengerCount, Is.EqualTo(ExpectedMaximumPassengers));
    }

    [Test]
    public async Task SaveTicketsAsync_NullList_ReturnsFalse()
    {
        var result = await _bookingService.SaveTicketsAsync(null!);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SaveTicketsAsync_EmptyList_ReturnsFalse()
    {
        var result = await _bookingService.SaveTicketsAsync(new List<FlightTicket>());

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SaveTicketsAsync_DuplicateSeats_ReturnsFalse()
    {
        var tickets = new List<FlightTicket>
        {
            new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus },
            new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus }
        };

        var result = await _bookingService.SaveTicketsAsync(tickets);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SaveTicketsAsync_SeatNotAvailable_ReturnsFalse()
    {
        _ticketRepository.IsSeatAvailableAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(false);
        var tickets = new List<FlightTicket>
        {
            new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus }
        };

        var result = await _bookingService.SaveTicketsAsync(tickets);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SaveTicketsAsync_ValidTickets_ReturnsTrue()
    {
        _ticketRepository.IsSeatAvailableAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(true);
        _ticketRepository.SaveBatchWithAddOnsAsync(Arg.Any<List<FlightTicket>>(), Arg.Any<List<List<int>>>()).Returns(true);
        var tickets = new List<FlightTicket>
        {
            new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus },
            new FlightTicket { Seat = Seat1B, Price = StandardTicketPrice, Status = ActiveStatus }
        };

        var result = await _bookingService.SaveTicketsAsync(tickets);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task SaveTicketsAsync_ValidTickets_CallsRepositoryOnce()
    {
        _ticketRepository.IsSeatAvailableAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(true);
        _ticketRepository.SaveBatchWithAddOnsAsync(Arg.Any<List<FlightTicket>>(), Arg.Any<List<List<int>>>()).Returns(true);
        var tickets = new List<FlightTicket>
        {
            new FlightTicket { Seat = Seat1A, Price = StandardTicketPrice, Status = ActiveStatus }
        };

        await _bookingService.SaveTicketsAsync(tickets);

        await _ticketRepository.Received(1).SaveBatchWithAddOnsAsync(Arg.Any<List<FlightTicket>>(), Arg.Any<List<List<int>>>());
    }

    [Test]
    public void ParseBookingParameters_FullArgumentArray_ParsesFlight()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        object[] bookingArguments = { flight, user, NormalRequestedPassengers };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.Flight, Is.EqualTo(flight));
    }

    [Test]
    public void ParseBookingParameters_FullArgumentArray_ParsesUser()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        object[] bookingArguments = { flight, user, NormalRequestedPassengers };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.User, Is.EqualTo(user));
    }

    [Test]
    public void ParseBookingParameters_FullArgumentArray_ParsesRequestedPassengers()
    {
        var flight = new Flight { Id = DefaultFlightId };
        var user = new Customer { Id = 1, Email = "test@test.com" };
        object[] bookingArguments = { flight, user, NormalRequestedPassengers };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.RequestedPassengers, Is.EqualTo(NormalRequestedPassengers));
    }

    [Test]
    public void ParseBookingParameters_InvalidObject_ReturnsNullFlight()
    {
        var result = _bookingService.ParseBookingParameters("NotAnArray");

        Assert.That(result.Flight, Is.Null);
    }

    [Test]
    public void ParseBookingParameters_InvalidObject_ReturnsZeroPassengerCount()
    {
        var result = _bookingService.ParseBookingParameters("NotAnArray");

        Assert.That(result.RequestedPassengers, Is.EqualTo(0));
    }

    [Test]
    public void ParseBookingParameters_OnlyFlightArgument_ParsesFlight()
    {
        var flight = new Flight { Id = DefaultFlightId };
        object[] bookingArguments = { flight };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.Flight, Is.EqualTo(flight));
    }

    [Test]
    public void ParseBookingParameters_OnlyFlightArgument_ReturnsZeroPassengerCount()
    {
        var flight = new Flight { Id = DefaultFlightId };
        object[] bookingArguments = { flight };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.RequestedPassengers, Is.EqualTo(0));
    }

    [Test]
    public void ParseBookingParameters_FlightAndPassengerCount_ParsesFlight()
    {
        var flight = new Flight { Id = DefaultFlightId };
        object[] bookingArguments = { flight, NormalRequestedPassengers };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.Flight, Is.EqualTo(flight));
    }

    [Test]
    public void ParseBookingParameters_FlightAndPassengerCount_ParsesPassengerCount()
    {
        var flight = new Flight { Id = DefaultFlightId };
        object[] bookingArguments = { flight, NormalRequestedPassengers };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.RequestedPassengers, Is.EqualTo(NormalRequestedPassengers));
    }

    [Test]
    public void ParseBookingParameters_NoUserInArgs_FallsBackToSessionUser()
    {
        var sessionUser = new Customer { Id = 7, Email = "session@test.com" };
        UserSession.CurrentUser = sessionUser;
        var flight = new Flight { Id = DefaultFlightId };
        object[] bookingArguments = { flight };

        var result = _bookingService.ParseBookingParameters(bookingArguments);

        Assert.That(result.User, Is.EqualTo(sessionUser));
    }

    [Test]
    public void StorePendingBooking_ValidInput_StoresNonNullParameters()
    {
        var flight = new Flight { Id = DefaultFlightId };

        _bookingService.StorePendingBooking(flight, NormalRequestedPassengers);

        Assert.That(UserSession.PendingBookingParameters, Is.Not.Null);
    }

    [Test]
    public void StorePendingBooking_ValidInput_StoresFlightAsFirstElement()
    {
        var flight = new Flight { Id = DefaultFlightId };

        _bookingService.StorePendingBooking(flight, NormalRequestedPassengers);

        var pending = (object[])UserSession.PendingBookingParameters!;
        Assert.That(pending[0], Is.EqualTo(flight));
    }

    [Test]
    public void StorePendingBooking_ValidInput_StoresPassengerCountAsSecondElement()
    {
        var flight = new Flight { Id = DefaultFlightId };

        _bookingService.StorePendingBooking(flight, NormalRequestedPassengers);

        var pending = (object[])UserSession.PendingBookingParameters!;
        Assert.That(pending[1], Is.EqualTo(NormalRequestedPassengers));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_ExactMultipleCapacity_CalculatesCorrectRowCount()
    {
        var (_, rowCount) = await _bookingService.BuildSeatMapLayoutAsync(ExactMultipleCapacity);

        Assert.That(rowCount, Is.EqualTo(ExpectedExactMultipleRows));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_ExactMultipleCapacity_GeneratesCorrectLayoutCount()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(ExactMultipleCapacity);

        Assert.That(layout.Count, Is.EqualTo(ExpectedExactMultipleLayoutCount));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_NonMultipleCapacity_RoundsRowsUp()
    {
        var (_, rowCount) = await _bookingService.BuildSeatMapLayoutAsync(PartialMultipleCapacity);

        Assert.That(rowCount, Is.EqualTo(ExpectedPartialMultipleRows));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_NonMultipleCapacity_GeneratesCorrectLayoutCount()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(PartialMultipleCapacity);

        Assert.That(layout.Count, Is.EqualTo(ExpectedPartialMultipleLayoutCount));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_CalculatesOneRow()
    {
        var (_, rowCount) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(rowCount, Is.EqualTo(ExpectedMinimumCapacityRows));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsLabel1AToFirstSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[0].Label, Is.EqualTo("1A"));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsLabel1CToThirdSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[2].Label, Is.EqualTo("1C"));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsLabel1DToFourthSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[3].Label, Is.EqualTo("1D"));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsLabel1FToSixthSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[5].Label, Is.EqualTo("1F"));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsColumnZeroToFirstSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[0].Column, Is.EqualTo(0));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsColumnTwoToThirdSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[2].Column, Is.EqualTo(2));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsColumnFourToFourthSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[3].Column, Is.EqualTo(4));
    }

    [Test]
    public async Task BuildSeatMapLayoutAsync_MinimumCapacity_AssignsColumnSixToSixthSeat()
    {
        var (layout, _) = await _bookingService.BuildSeatMapLayoutAsync(MinimumFlightCapacity);

        Assert.That(layout[5].Column, Is.EqualTo(6));
    }

    [Test]
    public void ApplySeatSelection_SeatAlreadyAssignedToPassenger_ClearsSelectedPassengerSeat()
    {
        var seats = new List<string> { Seat1A, Seat1B };

        var updated = _bookingService.ApplySeatSelection(seats, 0, Seat1A);

        Assert.That(updated[0], Is.Empty);
    }

    [Test]
    public void ApplySeatSelection_SeatAlreadyAssignedToPassenger_LeavesOtherPassengerSeatUnchanged()
    {
        var seats = new List<string> { Seat1A, Seat1B };

        var updated = _bookingService.ApplySeatSelection(seats, 0, Seat1A);

        Assert.That(updated[1], Is.EqualTo(Seat1B));
    }

    [Test]
    public void ApplySeatSelection_NewSeatAssignedToPassenger_ClearsDuplicateSeat()
    {
        var seats = new List<string> { Seat1A, Seat2B };

        var updated = _bookingService.ApplySeatSelection(seats, 1, Seat1A);

        Assert.That(updated[0], Is.Empty);
    }

    [Test]
    public void ApplySeatSelection_NewSeatAssignedToPassenger_AssignsNewSeatToPassenger()
    {
        var seats = new List<string> { Seat1A, Seat2B };

        var updated = _bookingService.ApplySeatSelection(seats, 1, Seat1A);

        Assert.That(updated[1], Is.EqualTo(Seat1A));
    }

    [Test]
    public void ApplySeatSelection_UnoccupiedSeat_AssignsSeatToPassenger()
    {
        var seats = new List<string> { string.Empty, string.Empty };

        var updated = _bookingService.ApplySeatSelection(seats, 0, Seat1A);

        Assert.That(updated[0], Is.EqualTo(Seat1A));
    }

    [Test]
    public void ApplyAddOnUpdates_WithAddList_AddsNewAddOnToCollection()
    {
        var priorityBoarding = new AddOn { Id = 1, Name = "Priority Boarding" };
        var extraLuggage = new AddOn { Id = 2, Name = "Extra Luggage" };
        var currentAddOns = new List<AddOn> { priorityBoarding };

        _bookingService.ApplyAddOnUpdates(currentAddOns, new List<AddOn> { extraLuggage }, new List<AddOn> { priorityBoarding });

        Assert.That(currentAddOns, Contains.Item(extraLuggage));
    }

    [Test]
    public void ApplyAddOnUpdates_WithRemoveList_RemovesAddOnFromCollection()
    {
        var priorityBoarding = new AddOn { Id = 1, Name = "Priority Boarding" };
        var extraLuggage = new AddOn { Id = 2, Name = "Extra Luggage" };
        var currentAddOns = new List<AddOn> { priorityBoarding };

        _bookingService.ApplyAddOnUpdates(currentAddOns, new List<AddOn> { extraLuggage }, new List<AddOn> { priorityBoarding });

        Assert.That(currentAddOns, Does.Not.Contain(priorityBoarding));
    }

    [Test]
    public void ApplyAddOnUpdates_AddDuplicate_DoesNotCreateDuplicate()
    {
        var addOn = new AddOn { Id = 1, Name = "Priority Boarding" };
        var currentAddOns = new List<AddOn> { addOn };

        _bookingService.ApplyAddOnUpdates(currentAddOns, new List<AddOn> { addOn }, new List<AddOn>());

        Assert.That(currentAddOns.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetInitialPassengerCountAsync_RequestedWithinMax_ReturnsRequested()
    {
        var count = await _bookingService.GetInitialPassengerCountAsync(ExpectedMaximumPassengers, NormalRequestedPassengers);

        Assert.That(count, Is.EqualTo(NormalRequestedPassengers));
    }

    [Test]
    public async Task GetInitialPassengerCountAsync_RequestedExceedsMax_CapsAtMax()
    {
        var count = await _bookingService.GetInitialPassengerCountAsync(2, 5);

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetInitialPassengerCountAsync_ZeroRequested_FallsBackToOne()
    {
        var count = await _bookingService.GetInitialPassengerCountAsync(5, 0);

        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAvailableAddOnsAsync_Always_CallsRepository()
    {
        _addOnRepository.GetAsync().Returns(Task.FromResult<IEnumerable<AddOn>>(new List<AddOn>()));

        await _bookingService.GetAvailableAddOnsAsync();

        await _addOnRepository.Received(1).GetAsync();
    }

    [Test]
    public async Task GetAvailableAddOnsAsync_Always_ReturnsAddOnsFromRepository()
    {
        var addOns = new List<AddOn> { new AddOn { Id = 1, Name = "Meal" } };
        _addOnRepository.GetAsync().Returns(Task.FromResult<IEnumerable<AddOn>>(addOns));

        var result = await _bookingService.GetAvailableAddOnsAsync();

        Assert.That(result, Is.EqualTo(addOns));
    }

    [Test]
    public async Task GetOccupiedSeatsAsync_Always_CallsRepositoryWithFlightId()
    {
        _ticketRepository.GetOccupiedSeatsAsync(DefaultFlightId).Returns(Task.FromResult<IEnumerable<string>>(new List<string>()));

        await _bookingService.GetOccupiedSeatsAsync(DefaultFlightId);

        await _ticketRepository.Received(1).GetOccupiedSeatsAsync(DefaultFlightId);
    }

    [Test]
    public async Task GetOccupiedSeatsAsync_Always_ReturnsSeatsFromRepository()
    {
        var seats = new List<string> { Seat1A, Seat1B };
        _ticketRepository.GetOccupiedSeatsAsync(DefaultFlightId).Returns(Task.FromResult<IEnumerable<string>>(seats));

        var result = await _bookingService.GetOccupiedSeatsAsync(DefaultFlightId);

        Assert.That(result, Is.EqualTo(seats));
    }

    [Test]
    public async Task GetAddOnsByIdsAsync_Always_CallsRepositoryWithIds()
    {
        var addOnIds = new List<int> { 1, 2 };
        _addOnRepository.GetByIdsAsync(Arg.Any<IEnumerable<int>>()).Returns(Task.FromResult<IEnumerable<AddOn>>(new List<AddOn>()));

        await _bookingService.GetAddOnsByIdsAsync(addOnIds);

        await _addOnRepository.Received(1).GetByIdsAsync(Arg.Is<IEnumerable<int>>(x => x.SequenceEqual(addOnIds)));
    }
}
