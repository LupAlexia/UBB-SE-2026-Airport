using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class RouteServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IRouteService
{
    private const string BaseUrl = "api/routes";

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        var dtos = await GetListAsync<RouteDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        var dto = await GetOptionalAsync<RouteDTO>($"{BaseUrl}/{routeId}");
        return dto is null ? null : MapToEntity(dto);
    }

    public async Task<int> AddWithInitialFlightAsync(int companyId, int airportId, string routeType, int recurrenceInterval,
        DateTime startDate, DateTime endDate, TimeOnly departureTime, TimeOnly arrivalTime,
        int capacity, string flightNumber, int runwayId, int gateId)
    {
        var payload = new
        {
            CompanyId = companyId,
            AirportId = airportId,
            RouteType = routeType,
            RecurrenceInterval = recurrenceInterval,
            StartDate = startDate,
            EndDate = endDate,
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime,
            Capacity = capacity,
            FlightNumber = flightNumber,
            RunwayId = runwayId,
            GateId = gateId
        };
        return await PostForResultAsync<object, int>($"{BaseUrl}/add-with-flight", payload);
    }

    public string NormalizeFlightType(string? routeType)
    {
        if (string.IsNullOrEmpty(routeType)) return "One-Way";
        return string.Equals(routeType, "RoundTrip", StringComparison.OrdinalIgnoreCase) ? "Round-Trip" : "One-Way";
    }

    public string GetRelevantTime(Route? route)
    {
        if (route is null) return string.Empty;
        return route.DepartureTime.ToString("HH:mm");
    }

    public static Route MapToEntity(RouteDTO dto)
    {
        var route = new Route
        {
            Id = dto.id,
            RouteType = dto.routeType,
            StartDate = dto.startDate,
            EndDate = dto.endDate,
            DepartureTime = dto.departureTime,
            ArrivalTime = dto.arrivalTime,
            Capacity = dto.capacity,
            RecurrenceInterval = dto.recurrenceInterval
        };

        if (dto.airport is not null)
        {
            route.Airport = new Airport(dto.airport.Id, dto.airport.AirportCode, dto.airport.City, dto.airport.Name);
        }

        if (dto.company is not null)
        {
            route.Company = new Company(dto.company.id, dto.company.name);
        }

        return route;
    }

    public static RouteDTO MapToDto(Route route)
    {
        var airportDto = route.Airport is not null
            ? new AirportDTO(route.Airport.Id, route.Airport.AirportCode, route.Airport.City, route.Airport.Name)
            : null;
        var companyDto = route.Company is not null
            ? new CompanyDTO(route.Company.Id, route.Company.Name)
            : null;

        return new RouteDTO(
            route.Id,
            route.RouteType,
            route.StartDate,
            route.EndDate,
            route.DepartureTime,
            route.ArrivalTime,
            route.Capacity,
            route.RecurrenceInterval,
            airportDto,
            companyDto);
    }
}
