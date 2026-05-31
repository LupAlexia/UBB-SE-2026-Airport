using System;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record RouteDTO(
        int id,
        string routeType,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly departureTime,
        TimeOnly arrivalTime,
        int capacity,
        int recurrenceInterval,
        AirportDTO? airport = null,
        CompanyDTO? company = null);
}
