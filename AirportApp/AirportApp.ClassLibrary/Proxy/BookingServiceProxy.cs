using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class BookingServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IBookingService
{
    private const string AddOnBaseUrl = "api/addon";
    private const string FlightTicketBaseUrl = "api/flightticket";

    public List<FlightTicket> CreateTickets(Flight flight, Customer user, List<PassengerData> passengers, float basePrice)
    {
        var tickets = new List<FlightTicket>();
        foreach (var passenger in passengers)
        {
            var ticket = new FlightTicket
            {
                Flight = flight,
                User = user,
                PassengerFirstName = passenger.FirstName,
                PassengerLastName = passenger.LastName,
                PassengerEmail = passenger.Email,
                PassengerPhone = passenger.Phone,
                Seat = passenger.SelectedSeat,
                Price = basePrice,
                Status = "Active",
                SelectedAddOns = passenger.SelectedAddOns is not null ? new List<AddOn>(passenger.SelectedAddOns) : []
            };
            tickets.Add(ticket);
        }
        return tickets;
    }

    public async Task<bool> SaveTicketsAsync(List<FlightTicket> tickets)
    {
        var ticketTransferObjectList = new List<FlightTicketDTO>();
        foreach (FlightTicket ticket in tickets)
        {
            var addOnTransferObjectList = new List<AddOnDTO>();
            if (ticket.SelectedAddOns is not null)
            {
                foreach (AddOn addOn in ticket.SelectedAddOns)
                {
                    addOnTransferObjectList.Add(new AddOnDTO(addOn.Id, addOn.Name, addOn.BasePrice));
                }
            }

            ticketTransferObjectList.Add(new FlightTicketDTO(
                ticket.Id,
                ticket.User.Id,
                ticket.Flight.Id,
                ticket.Seat,
                ticket.Price,
                ticket.Status,
                ticket.PassengerFirstName,
                ticket.PassengerLastName,
                ticket.PassengerEmail,
                ticket.PassengerPhone,
                addOnTransferObjectList));
        }

        var requestBody = new SaveTicketsRequestDTO { Tickets = ticketTransferObjectList };
        return await PostForResultAsync<SaveTicketsRequestDTO, bool>($"{FlightTicketBaseUrl}/batch", requestBody);
    }

    public async Task<List<AddOn>> GetAvailableAddOnsAsync()
    {
        var dtos = await GetListAsync<AddOnDTO>(AddOnBaseUrl);
        return dtos.Select(dto => new AddOn { Id = dto.id, Name = dto.name, BasePrice = dto.basePrice }).ToList();
    }

    public async Task<List<AddOn>> GetAddOnsByIdsAsync(List<int> ids)
    {
        var dtos = await PostForResultAsync<List<int>, List<AddOnDTO>>($"{AddOnBaseUrl}/by-ids", ids);
        return dtos.Select(dto => new AddOn { Id = dto.id, Name = dto.name, BasePrice = dto.basePrice }).ToList();
    }

    public async Task<List<string>> GetOccupiedSeatsAsync(int flightId)
    {
        return await GetListAsync<string>($"{FlightTicketBaseUrl}/flight/{flightId}/occupied-seats");
    }

    public async Task<string> ValidatePassengersAsync(List<PassengerData> passengers)
    {
        var dtos = passengers.Select(passenger => new PassengerDataDTO
        {
            FirstName = passenger.FirstName ?? string.Empty,
            LastName = passenger.LastName ?? string.Empty,
            Email = passenger.Email ?? string.Empty,
            Phone = passenger.Phone ?? string.Empty,
            SelectedSeat = passenger.SelectedSeat ?? string.Empty,
            SelectedAddOns = passenger.SelectedAddOns?.Select(addon => new PricingAddOnDTO { Id = addon.Id, BasePrice = addon.BasePrice }).ToList() ?? []
        }).ToList();

        return await PostForResultAsync<List<PassengerDataDTO>, string>("api/booking/validate-passengers", dtos);
    }

    public async Task<int> CalculateMaxPassengersAsync(int routeCapacity, int occupiedSeatCount, int requestedPassengerCount)
    {
        return await GetRequiredAsync<int>($"api/booking/calculate-max-passengers?routeCapacity={routeCapacity}&occupiedSeatCount={occupiedSeatCount}&requestedPassengerCount={requestedPassengerCount}");
    }

    public BookingParametersResult ParseBookingParameters(object parameter)
    {
        Flight? selectedFlight = null;
        Customer? user = null;
        int requestedPassengers = 0;

        if (parameter is object[] arguments && arguments.Length > 0)
        {
            selectedFlight = arguments[0] as Flight;
            if (arguments.Length >= 3)
            {
                user = arguments[1] as Customer;
                if (arguments[2] is int count) requestedPassengers = count;
            }
            else if (arguments.Length >= 2)
            {
                if (arguments[1] is int count) requestedPassengers = count;
                else user = arguments[1] as Customer;
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
        UserSession.PendingBookingParameters = [flight, requestedPassengers];
    }

    public async Task<(List<SeatDescriptor> Layout, int RowCount)> BuildSeatMapLayoutAsync(int capacity)
    {
        var response = await GetRequiredAsync<SeatMapResponseDTO>($"api/booking/build-seat-map?capacity={capacity}");
        return (response.Layout, response.RowCount);
    }

    public IList<string> ApplySeatSelection(IList<string> currentSeats, int targetPassengerIndex, string clickedSeat)
    {
        var updated = new List<string>(currentSeats);
        if (updated[targetPassengerIndex] == clickedSeat)
        {
            updated[targetPassengerIndex] = string.Empty;
        }
        else
        {
            for (int index = 0; index < updated.Count; index++)
            {
                if (updated[index] == clickedSeat) updated[index] = string.Empty;
            }
            updated[targetPassengerIndex] = clickedSeat;
        }
        return updated;
    }

    public void ApplyAddOnUpdates(IList<AddOn> currentAddOns, IEnumerable<AddOn> toAdd, IEnumerable<AddOn> toRemove)
    {
        foreach (var addOn in toAdd)
        {
            if (!currentAddOns.Contains(addOn)) currentAddOns.Add(addOn);
        }
        foreach (var addOn in toRemove)
        {
            currentAddOns.Remove(addOn);
        }
    }

    public async Task<int> GetInitialPassengerCountAsync(int maxPassengers, int requestedCount)
    {
        return await GetRequiredAsync<int>($"api/booking/initial-passenger-count?maxPassengers={maxPassengers}&requestedCount={requestedCount}");
    }
}
