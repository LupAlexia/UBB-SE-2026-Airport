using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public static class MappingExtensions
    {
        public static AirportDTO ToDto(this Airport airport)
        {
            if (airport == null) return null!;
            return new AirportDTO(airport.Id, airport.AirportCode, airport.City, airport.Name);
        }

        public static GateDTO ToDto(this Gate gate)
        {
            if (gate == null) return null!;
            return new GateDTO(gate.Id, gate.GateName);
        }

        public static RunwayDTO ToDto(this Runway runway)
        {
            if (runway == null) return null!;
            return new RunwayDTO(runway.Id, runway.Name);
        }

        public static CompanyDTO ToDto(this Company company)
        {
            if (company == null) return null!;
            return new CompanyDTO(company.Id, company.Name);
        }

        public static RouteDTO ToDto(this Route route)
        {
            if (route == null) return null!;

            AirportDTO? airportDto = route.Airport != null ? route.Airport.ToDto() : null;
            CompanyDTO? companyDto = route.Company != null ? route.Company.ToDto() : null;

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

        public static FlightDTO ToDto(this Flight flight)
        {
            if (flight == null) return null!;

            RouteDTO? routeDto = flight.Route != null ? flight.Route.ToDto() : null;
            RunwayDTO? runwayDto = flight.Runway is { Id: > 0 } runway ? runway.ToDto() : null;
            GateDTO? gateDto = flight.Gate is { Id: > 0 } gate ? gate.ToDto() : null;
            return new FlightDTO(
                flight.Id,
                flight.Route?.Id ?? 0,
                flight.Gate?.Id ?? 0,
                flight.Gate?.GateName ?? string.Empty,
                flight.Runway?.Id ?? 0,
                flight.Runway?.Name ?? string.Empty,
                flight.Date,
                flight.FlightNumber,
                routeDto,
                runwayDto,
                gateDto);
        }
    }
}