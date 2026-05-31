using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightSearchService flightSearchService;

        public FlightController(IFlightSearchService flightSearchService)
        {
            this.flightSearchService = flightSearchService;
        }

        [HttpGet("{flightIdentifier}")]
        public async Task<ActionResult<FlightDTO>> GetByIdAsync(int flightIdentifier)
        {
            Flight? flight = await flightSearchService.GetFlightByIdAsync(flightIdentifier);
            if (flight == null)
            {
                return NotFound();
            }

            return Ok(MapToFlightDataTransferObject(flight));
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FlightDTO>>> GetByRouteAsync(
            [FromQuery] string location,
            [FromQuery] string routeType,
            [FromQuery] DateTime? date)
        {
            IEnumerable<Flight> flights = await flightSearchService.GetFlightsByRouteAsync(location, routeType, date);
            return Ok(flights.Select(MapToFlightDataTransferObject));
        }

        [HttpGet("{flightIdentifier}/occupied-seat-count")]
        public async Task<ActionResult<int>> GetOccupiedSeatCountAsync(int flightIdentifier)
        {
            int occupiedSeatCount = await flightSearchService.GetOccupiedSeatCountAsync(flightIdentifier);
            return Ok(occupiedSeatCount);
        }

        private static FlightDTO MapToFlightDataTransferObject(Flight flight)
        {
            RouteDTO? routeDataTransferObject = null;

            if (flight.Route != null)
            {
                AirportDTO? airportDataTransferObject = flight.Route.Airport != null
                    ? new AirportDTO(
                        flight.Route.Airport.Id,
                        flight.Route.Airport.AirportCode,
                        flight.Route.Airport.City,
                        flight.Route.Airport.Name)
                    : null;

                CompanyDTO? companyDataTransferObject = flight.Route.Company != null
                    ? new CompanyDTO(flight.Route.Company.Id, flight.Route.Company.Name)
                    : null;

                routeDataTransferObject = new RouteDTO(
                    flight.Route.Id,
                    flight.Route.RouteType,
                    flight.Route.StartDate,
                    flight.Route.EndDate,
                    flight.Route.DepartureTime,
                    flight.Route.ArrivalTime,
                    flight.Route.Capacity,
                    flight.Route.RecurrenceInterval,
                    airportDataTransferObject,
                    companyDataTransferObject);
            }

            return new FlightDTO(
                flight.Id,
                flight.Route?.Id ?? 0,
                flight.Gate?.Id ?? 0,
                flight.Runway?.Id ?? 0,
                flight.Date,
                flight.FlightNumber,
                routeDataTransferObject);
        }
    }
}
