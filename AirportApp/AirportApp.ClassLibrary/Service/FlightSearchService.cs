using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class FlightSearchService(IFlightRepository flightRepository) : IFlightSearchService
{
    private const string DepartureRouteType = "Departure";
    private const string ArrivalRouteType = "Arrival";

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string location, bool isDeparture, DateTime? date, int? passengers)
    {
        if (string.IsNullOrWhiteSpace(location))
            return new List<Flight>();

        string flightType = isDeparture ? DepartureRouteType : ArrivalRouteType;
        var flights = (await flightRepository.SearchFlightsAsync(location, flightType, date)).ToList();

        if (passengers.HasValue && passengers.Value > 0)
        {
            var filteredFlights = new List<Flight>();
            foreach (var flight in flights)
            {
                int occupiedSeats = await flightRepository.GetOccupiedSeatCountAsync(flight.Id);
                int availableSeats = flight.Route!.Capacity - occupiedSeats;
                if (availableSeats >= passengers.Value)
                    filteredFlights.Add(flight);
            }
            return filteredFlights;
        }

        return flights;
    }

    public int? ParsePassengerCount(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (int.TryParse(input, out var parsed) && parsed > 0)
            return parsed;

        return 1;
    }

    public async Task<Flight?> GetFlightByIdAsync(int id)
    {
        return await flightRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Flight>> GetFlightsByRouteAsync(string location, string routeType, DateTime? date)
    {
        return await flightRepository.SearchFlightsAsync(location, routeType, date);
    }

    public async Task<int> GetOccupiedSeatCountAsync(int flightId)
    {
        return await flightRepository.GetOccupiedSeatCountAsync(flightId);
    }
}
