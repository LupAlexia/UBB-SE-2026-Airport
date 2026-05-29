using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class BookingService(IFlightTicketRepository ticketRepository, IAddOnRepository addOnRepository) : IBookingService
{
    private const string ActiveStatus = "Active";
    private const int FlightAndUserAndCountArgumentLength = 3;
    private const int FlightAndSecondArgumentLength = 2;
    private const int FlightArgumentIndex = 0;
    private const int SecondArgumentIndex = 1;
    private const int PassengerCountArgumentIndex = 2;
    private const int SeatsPerRow = 6;
    private const int AisleAfterColumn = 3;
    private const int DefaultInitialPassengerCount = 1;

    public List<FlightTicket> CreateTickets(Flight flight, Customer user, List<PassengerData> passengers, float basePrice)
    {
        var tickets = new List<FlightTicket>();
        foreach (var passenger in passengers)
        {
            tickets.Add(new FlightTicket
            {
                Flight = flight,
                User = user,
                PassengerFirstName = passenger.FirstName,
                PassengerLastName = passenger.LastName,
                PassengerEmail = passenger.Email,
                PassengerPhone = passenger.Phone,
                Seat = passenger.SelectedSeat,
                Price = basePrice,
                Status = ActiveStatus,
                SelectedAddOns = passenger.SelectedAddOns.ToList()
            });
        }
        return tickets;
    }

    public Task<string> ValidatePassengersAsync(List<PassengerData> passengers)
    {
        if (passengers == null || passengers.Count == 0)
            return Task.FromResult("At least one passenger is required.");

        for (int index = 0; index < passengers.Count; index++)
        {
            var passenger = passengers[index];
            int passengerNumber = index + 1;

            if (string.IsNullOrWhiteSpace(passenger.FirstName))
                return Task.FromResult($"Passenger {passengerNumber}: first name is required.");
            if (string.IsNullOrWhiteSpace(passenger.LastName))
                return Task.FromResult($"Passenger {passengerNumber}: last name is required.");
            if (!string.IsNullOrWhiteSpace(passenger.Email) && !ValidationHelper.IsValidEmail(passenger.Email))
                return Task.FromResult($"Passenger {passengerNumber}: email format is invalid.");
            if (string.IsNullOrWhiteSpace(passenger.SelectedSeat))
                return Task.FromResult($"Passenger {passengerNumber}: please select a seat.");
        }

        return Task.FromResult(string.Empty);
    }

    public Task<int> CalculateMaxPassengersAsync(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount)
    {
        int remainingCapacity = routeCapacity - occupiedSeatCount;
        if (requestedPassengerCount > 0)
            return Task.FromResult(Math.Min(requestedPassengerCount, remainingCapacity));
        return Task.FromResult(remainingCapacity);
    }

    public async Task<bool> SaveTicketsAsync(List<FlightTicket> tickets)
    {
        if (tickets == null || tickets.Count == 0)
            return false;

        bool duplicateSeat = tickets
            .Where(t => !string.IsNullOrWhiteSpace(t.Seat))
            .GroupBy(t => t.Seat)
            .Any(g => g.Count() > 1);

        if (duplicateSeat)
            return false;

        foreach (var ticket in tickets)
        {
            if (!string.IsNullOrWhiteSpace(ticket.Seat))
            {
                bool seatAvailable = await ticketRepository.IsSeatAvailableAsync(ticket.Flight?.Id ?? 0, ticket.Seat);
                if (!seatAvailable)
                    return false;
            }
        }

        var addOnIds = tickets.Select(t => t.SelectedAddOns?.Select(a => a.Id).ToList() ?? new List<int>()).ToList();
        return await ticketRepository.SaveBatchWithAddOnsAsync(tickets, addOnIds);
    }

    public async Task<List<AddOn>> GetAvailableAddOnsAsync()
    {
        return (await addOnRepository.GetAsync()).ToList();
    }

    public async Task<List<string>> GetOccupiedSeatsAsync(int flightId)
    {
        return (await ticketRepository.GetOccupiedSeatsAsync(flightId)).ToList();
    }

    public BookingParametersResult ParseBookingParameters(object parameter)
    {
        Flight? selectedFlight = null;
        Customer? user = null;
        int requestedPassengers = 0;

        if (parameter is object[] arguments && arguments.Length > 0)
        {
            selectedFlight = arguments[FlightArgumentIndex] as Flight;

            if (arguments.Length >= FlightAndUserAndCountArgumentLength)
            {
                user = arguments[SecondArgumentIndex] as Customer;
                if (arguments[PassengerCountArgumentIndex] is int count)
                    requestedPassengers = count;
            }
            else if (arguments.Length >= FlightAndSecondArgumentLength)
            {
                if (arguments[SecondArgumentIndex] is int count)
                    requestedPassengers = count;
                else
                    user = arguments[SecondArgumentIndex] as Customer;
            }
        }

        user ??= UserSession.CurrentUser;

        return new BookingParametersResult
        {
            Flight = selectedFlight!,
            User = user!,
            RequestedPassengers = requestedPassengers
        };
    }

    public void StorePendingBooking(Flight flight, int requestedPassengers)
    {
        UserSession.PendingBookingParameters = new object[] { flight, requestedPassengers };
    }

    public Task<(List<SeatDescriptor> Layout, int RowCount)> BuildSeatMapLayoutAsync(int capacity)
    {
        var layout = new List<SeatDescriptor>();
        char[] seatLetters = { 'A', 'B', 'C', 'D', 'E', 'F' };
        int rowCount = (capacity + SeatsPerRow - 1) / SeatsPerRow;

        for (int row = 0; row < rowCount; row++)
        {
            for (int seatIndex = 0; seatIndex < SeatsPerRow; seatIndex++)
            {
                int column = seatIndex < AisleAfterColumn ? seatIndex : seatIndex + 1;
                layout.Add(new SeatDescriptor
                {
                    Row = row,
                    Column = column,
                    Label = $"{row + 1}{seatLetters[seatIndex]}"
                });
            }
        }

        return Task.FromResult((layout, rowCount));
    }

    public IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat)
    {
        var updated = currentSeats.ToList();

        if (updated[targetPassengerIndex] == clickedSeat)
        {
            updated[targetPassengerIndex] = string.Empty;
        }
        else
        {
            for (int index = 0; index < updated.Count; index++)
            {
                if (updated[index] == clickedSeat)
                    updated[index] = string.Empty;
            }
            updated[targetPassengerIndex] = clickedSeat;
        }

        return updated;
    }

    public void ApplyAddOnUpdates(IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove)
    {
        foreach (var addOn in toAdd)
        {
            if (!currentAddOns.Contains(addOn))
                currentAddOns.Add(addOn);
        }
        foreach (var addOn in toRemove)
            currentAddOns.Remove(addOn);
    }

    public Task<int> GetInitialPassengerCountAsync(int maxPassengers, int requestedCount)
    {
        int initial = requestedCount > 0 ? requestedCount : DefaultInitialPassengerCount;
        return Task.FromResult(Math.Min(initial, maxPassengers));
    }

    public async Task<List<AddOn>> GetAddOnsByIdsAsync(List<int> ids)
    {
        return (await addOnRepository.GetByIdsAsync(ids)).ToList();
    }
}
