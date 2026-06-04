namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record FlightDTO(
        int id,
        int routeId,
        int gateId,
        string gateName,
        int runwayId,
        string runwayName,
        DateTime date,
        string flightNumber,
        RouteDTO? route,
        RunwayDTO? runway = null,
        GateDTO? gate = null);
}
