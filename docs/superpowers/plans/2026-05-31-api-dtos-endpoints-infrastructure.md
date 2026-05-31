# API — DTOs, Endpoints & Infrastructure Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Wire up the merged Airport API project — connection string, DI, Swagger, CORS — and update all DTOs and controllers to reflect the merged domain (Runway on flights, DateOnly/TimeOnly on routes, Name on airports, Duty Free shop ticket filtering).

**Architecture:** `AirportApp.ClassLibrary` owns all domain, repos, services, and a new `ServiceRegistrationExtensions` extension method. `AirportApp.Api` owns controllers and `Program.cs`. DTOs live in the ClassLibrary and are shared between the two layers. All controller request/response types use explicit, fully-spelled-out names — no abbreviations.

**Tech Stack:** .NET 10, ASP.NET Core, Entity Framework Core 10, SQL Server (LocalDB for dev), Swashbuckle/Swagger, System.Text.Json (DateOnly/TimeOnly supported natively in .NET 7+, no custom converters needed)

---

## File Map

| Action | File |
|--------|------|
| Create | `AirportApp/AirportApp.Api/appsettings.example.json` |
| Create | `AirportApp/AirportApp.ClassLibrary/ServiceRegistrationExtensions.cs` |
| Rewrite | `AirportApp/AirportApp.Api/Program.cs` |
| Modify | `AirportApp/AirportApp.ClassLibrary/Entity/Dto/FlightDTO.cs` |
| Modify | `AirportApp/AirportApp.ClassLibrary/Entity/Dto/RouteDTO.cs` |
| Modify | `AirportApp/AirportApp.ClassLibrary/Entity/Dto/AirportDTO.cs` |
| Modify | `AirportApp/AirportApp.Api/Controllers/FlightController.cs` |
| Modify | `AirportApp/AirportApp.Api/Controllers/FlightsController.cs` |
| Modify | `AirportApp/AirportApp.Api/Controllers/RoutesController.cs` |
| Modify | `AirportApp/AirportApp.Api/Controllers/AirportController.cs` |
| Modify | `AirportApp/AirportApp.Api/Controllers/TicketController.cs` |

---

## Task 1: Create appsettings.example.json

**Files:**
- Create: `AirportApp/AirportApp.Api/appsettings.example.json`

The real `appsettings.json` is gitignored. This example file is committed and serves as a template every developer copies locally.

- [ ] **Step 1: Create the example config file**

Create `AirportApp/AirportApp.Api/appsettings.example.json` with:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AirportAppDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:5043", "https://localhost:7278" ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- [ ] **Step 2: Copy to your local appsettings.json**

In a terminal from `AirportApp/AirportApp.Api/`:
```
copy appsettings.example.json appsettings.json
```
(On Unix: `cp appsettings.example.json appsettings.json`)

The `appsettings.json` is gitignored — it stays local. The example file gets committed.

- [ ] **Step 3: Commit**

```bash
git add AirportApp/AirportApp.Api/appsettings.example.json
git commit -m "feat: add appsettings.example.json with connection string, CORS, and logging config"
```

---

## Task 2: Create ServiceRegistrationExtensions.cs

**Files:**
- Create: `AirportApp/AirportApp.ClassLibrary/ServiceRegistrationExtensions.cs`

This extension method keeps `Program.cs` readable and lets the ClassLibrary own its own DI wiring. All services and repositories are registered as `Scoped` (one instance per HTTP request).

Note: `ComplaintTicketRepository` has a typo in its class name (`ComplaintTicketRepostitory`) — use the misspelled name in the registration until it is renamed separately.

- [ ] **Step 1: Create the extension method file**

Create `AirportApp/AirportApp.ClassLibrary/ServiceRegistrationExtensions.cs`:

```csharp
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace AirportApp.ClassLibrary;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddAirportServices(this IServiceCollection services)
    {
        RegisterRepositories(services);
        RegisterServices(services);
        services.AddAutoMapper(typeof(ServiceRegistrationExtensions).Assembly);
        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IAddOnRepository, AddOnRepository>();
        services.AddScoped<IAdministratorRepository, AdministratorRepository>();
        services.AddScoped<IAirportRepository, AirportRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IComplaintTicketCategoryRepository, ComplaintTicketCategoryRepository>();
        services.AddScoped<IComplaintTicketRepository, ComplaintTicketRepostitory>();
        services.AddScoped<IComplaintTicketSubcategoryRepository, ComplaintTicketSubcategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDecisionTreeRepository, DecisionTreeRepository>();
        services.AddScoped<IEmployeeFlightRepository, EmployeeFlightRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IFAQRepository, FAQRepository>();
        services.AddScoped<IFlightRepository, FlightRepository>();
        services.AddScoped<IFlightTicketRepository, FlightTicketRepository>();
        services.AddScoped<IGateRepository, GateRepository>();
        services.AddScoped<IManagerRepository, ManagerRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IRunwayRepository, RunwayRepository>();
        services.AddScoped<ISenderRepository, SenderRepository>();
        services.AddScoped<IShopItemRepository, ShopItemRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IAirportService, AirportService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ICancellationService, CancellationService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IComplaintTicketCategoryService, ComplaintTicketCategoryService>();
        services.AddScoped<IComplaintTicketService, ComplaintTicketService>();
        services.AddScoped<IComplaintTicketSubcategoryService, ComplaintTicketSubcategoryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IDecisionTreeService, DecisionTreeService>();
        services.AddScoped<IEmployeeFlightService, EmployeeFlightService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IFAQService, FAQService>();
        services.AddScoped<IFlightRouteService, FlightRouteService>();
        services.AddScoped<IFlightSearchService, FlightSearchService>();
        services.AddScoped<IFlightService, FlightService>();
        services.AddScoped<IGateService, GateService>();
        services.AddScoped<IManagerService, ManagerService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IRunwayService, RunwayService>();
        services.AddScoped<IShopItemService, ShopItemService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IUserService, UserService>();
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add AirportApp/AirportApp.ClassLibrary/ServiceRegistrationExtensions.cs
git commit -m "feat: add ServiceRegistrationExtensions to wire all repos and services"
```

---

## Task 3: Rewrite Program.cs

**Files:**
- Modify: `AirportApp/AirportApp.Api/Program.cs`

The entire file is replaced. CORS reads allowed origins from configuration. Swagger is always visible (not just in Development) because this is an academic project. `DateOnly` and `TimeOnly` are serialised correctly by System.Text.Json natively on .NET 10 — no custom converters needed.

- [ ] **Step 1: Replace Program.cs**

Replace the entire contents of `AirportApp/AirportApp.Api/Program.cs` with:

```csharp
using AirportApp.ClassLibrary;
using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder applicationBuilder = WebApplication.CreateBuilder(args);

applicationBuilder.Services.AddDbContext<AppDbContext>(dbContextOptions =>
    dbContextOptions.UseSqlServer(
        applicationBuilder.Configuration.GetConnectionString("DefaultConnection")));

applicationBuilder.Services.AddAirportServices();

string[] allowedCorsOrigins = applicationBuilder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5043"];

applicationBuilder.Services.AddCors(corsOptions =>
    corsOptions.AddPolicy("AirportCorsPolicy", corsPolicy =>
        corsPolicy
            .WithOrigins(allowedCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

applicationBuilder.Services.AddControllers();
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen(swaggerOptions =>
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Airport API",
        Version = "v1"
    }));

WebApplication application = applicationBuilder.Build();

application.UseSwagger();
application.UseSwaggerUI(swaggerUiOptions =>
    swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Airport API v1"));

application.UseCors("AirportCorsPolicy");
application.UseHttpsRedirection();
application.UseAuthorization();
application.MapControllers();

application.Run();
```

- [ ] **Step 2: Commit**

```bash
git add AirportApp/AirportApp.Api/Program.cs
git commit -m "feat: configure Program.cs with DB context, DI, CORS, and Swagger"
```

---

## Task 4: Build verification after infrastructure

**Files:** (none modified — verification only)

- [ ] **Step 1: Build the solution**

From the solution root (`UBB-SE-2026-Airport/`):
```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected output ends with:
```
Build succeeded.
    0 Error(s)
```

If there are errors about missing service/repository implementations (e.g., a service interface has no concrete class), investigate that specific pair — the class file may exist with a different namespace than expected. Fix the registration and rebuild before continuing.

---

## Task 5: Update FlightDTO — add runwayId

**Files:**
- Modify: `AirportApp/AirportApp.ClassLibrary/Entity/Dto/FlightDTO.cs`

The `Flight` domain model already has a `Runway` navigation property. The DTO needs to expose its identifier.

- [ ] **Step 1: Add runwayId to FlightDTO**

Replace the entire file:

```csharp
using System;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record FlightDTO(
        int id,
        int routeId,
        int gateId,
        int runwayId,
        DateTime date,
        string flightNumber,
        RouteDTO? route);
}
```

- [ ] **Step 2: Commit**

```bash
git add AirportApp/AirportApp.ClassLibrary/Entity/Dto/FlightDTO.cs
git commit -m "feat: add runwayId field to FlightDTO"
```

---

## Task 6: Update RouteDTO — DateOnly and TimeOnly

**Files:**
- Modify: `AirportApp/AirportApp.ClassLibrary/Entity/Dto/RouteDTO.cs`

The old DTO used combined `DateTime` values (921 format). The merged domain model stores `DateOnly StartDate`, `DateOnly EndDate`, `TimeOnly DepartureTime`, `TimeOnly ArrivalTime` separately. The DTO must match.

- [ ] **Step 1: Replace RouteDTO**

Replace the entire file:

```csharp
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
```

- [ ] **Step 2: Commit**

```bash
git add AirportApp/AirportApp.ClassLibrary/Entity/Dto/RouteDTO.cs
git commit -m "feat: update RouteDTO to use DateOnly and TimeOnly instead of DateTime"
```

---

## Task 7: Update AirportDTO — add name

**Files:**
- Modify: `AirportApp/AirportApp.ClassLibrary/Entity/Dto/AirportDTO.cs`

The `Airport` domain model has a `Name` property. The DTO was missing it.

- [ ] **Step 1: Add name to AirportDTO**

Replace the entire file:

```csharp
namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record AirportDTO(int id, string airportCode, string city, string name);
}
```

- [ ] **Step 2: Commit**

```bash
git add AirportApp/AirportApp.ClassLibrary/Entity/Dto/AirportDTO.cs
git commit -m "feat: add name field to AirportDTO"
```

---

## Task 8: Build verification after DTO changes

**Files:** (none modified — verification only)

The DTO changes are breaking changes for any code that constructs these records. Build now to discover all call sites that need updating before touching controllers one by one.

- [ ] **Step 1: Build and collect all errors**

```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected: build errors in `FlightController.cs`, `FlightsController.cs`, `RoutesController.cs`, `AirportController.cs` — these are the files fixed in Tasks 9–12. Any error outside those four files is unexpected and must be investigated before continuing.

---

## Task 9: Update FlightController (921 search controller)

**Files:**
- Modify: `AirportApp/AirportApp.Api/Controllers/FlightController.cs`

This controller manually constructs `FlightDTO` and `RouteDTO`. Both records changed shape. Callers of `AirportDTO` must also pass `name` now. Variable names are updated to remove abbreviations.

- [ ] **Step 1: Replace the file**

Replace the entire contents of `AirportApp/AirportApp.Api/Controllers/FlightController.cs`:

```csharp
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
```

- [ ] **Step 2: Build to verify this file compiles cleanly**

```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected: remaining errors only in `FlightsController.cs`, `RoutesController.cs`, `AirportController.cs`.

- [ ] **Step 3: Commit**

```bash
git add AirportApp/AirportApp.Api/Controllers/FlightController.cs
git commit -m "feat: update FlightController to use updated FlightDTO and RouteDTO signatures"
```

---

## Task 10: Update FlightsController (924 CRUD controller)

**Files:**
- Modify: `AirportApp/AirportApp.Api/Controllers/FlightsController.cs`

The POST and PUT actions currently accept a raw `Flight` domain entity as the request body, which forces the client to know about EF navigation properties. Replace with a `FlightRequestDTO` request record that contains only the scalar identifiers the client actually needs to provide. All parameter names are explicit — no abbreviations.

- [ ] **Step 1: Replace the file**

Replace the entire contents of `AirportApp/AirportApp.Api/Controllers/FlightsController.cs`:

```csharp
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController(IFlightService flightService) : ControllerBase
{
    private const string NullFlightDataErrorMessage = "Flight data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
    {
        return Ok(await flightService.GetAllFlightsAsync());
    }

    [HttpGet("{flightId:int}")]
    public async Task<ActionResult<Flight>> GetFlightById(int flightId)
    {
        Flight? flight = await flightService.GetFlightByIdAsync(flightId);

        if (flight == null)
        {
            return NotFound();
        }

        return Ok(flight);
    }

    [HttpGet("by-route/{routeId:int}")]
    public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsByRouteId(int routeId)
    {
        return Ok(await flightService.GetFlightsByRouteIdAsync(routeId));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddFlight([FromBody] FlightRequestDTO flightRequestData)
    {
        if (flightRequestData == null)
        {
            return BadRequest(NullFlightDataErrorMessage);
        }

        int newFlightIdentifier = await flightService.AddFlightAsync(
            flightRequestData.FlightNumber,
            flightRequestData.RouteId,
            flightRequestData.DepartureDate,
            flightRequestData.RunwayId,
            flightRequestData.GateId);

        return Ok(newFlightIdentifier);
    }

    [HttpPut("{flightId:int}")]
    public async Task<IActionResult> UpdateFlight(int flightId, [FromBody] FlightRequestDTO flightRequestData)
    {
        if (flightRequestData == null)
        {
            return BadRequest(NullFlightDataErrorMessage);
        }

        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return NotFound();
        }

        await flightService.UpdateFlightAsync(
            flightId,
            flightRequestData.DepartureDate,
            flightRequestData.FlightNumber,
            flightRequestData.RunwayId,
            flightRequestData.GateId);

        return NoContent();
    }

    [HttpDelete("{flightId:int}")]
    public async Task<IActionResult> DeleteFlightUsingId(int flightId)
    {
        if (await flightService.GetFlightByIdAsync(flightId) == null)
        {
            return NotFound();
        }

        await flightService.DeleteFlightAsync(flightId);
        return NoContent();
    }
}

public record FlightRequestDTO(
    string FlightNumber,
    int RouteId,
    DateTime DepartureDate,
    int RunwayId,
    int GateId);
```

- [ ] **Step 2: Build to verify this file compiles cleanly**

```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected: remaining errors only in `RoutesController.cs` and `AirportController.cs`.

- [ ] **Step 3: Commit**

```bash
git add AirportApp/AirportApp.Api/Controllers/FlightsController.cs
git commit -m "feat: replace raw Flight entity in POST/PUT with FlightRequestDTO including RunwayId"
```

---

## Task 11: Update RoutesController — return RouteDTO from GET, rename request record fields

**Files:**
- Modify: `AirportApp/AirportApp.Api/Controllers/RoutesController.cs`

Two changes:
1. `GetAll` and `GetById` currently return raw `Route` entities. Map them through `RouteDTO` so clients get a consistent, versioned contract.
2. `AddRouteWithFlightRequest` has abbreviated parameter names (`Dep`, `Arr`, `FlightNum`, `Interval`). Rename to full names. The service interface (`IRouteService.AddWithInitialFlightAsync`) takes `DateTime` for start/end — convert from the request's `DateOnly` at the call site.

- [ ] **Step 1: Replace the file**

Replace the entire contents of `AirportApp/AirportApp.Api/Controllers/RoutesController.cs`:

```csharp
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Route = AirportApp.ClassLibrary.Entity.Domain.Route;

namespace AirportAPI.Controllers.A5_Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController(IRouteService routeService) : ControllerBase
{
    private const string NullRouteDataErrorMessage = "Route data cannot be null.";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteDTO>>> GetAll()
    {
        IEnumerable<Route> routes = await routeService.GetAllRoutesAsync();
        return Ok(routes.Select(MapToRouteDataTransferObject));
    }

    [HttpGet("{routeId:int}")]
    public async Task<ActionResult<RouteDTO>> GetById(int routeId)
    {
        Route? route = await routeService.GetRouteByIdAsync(routeId);

        if (route == null)
        {
            return NotFound();
        }

        return Ok(MapToRouteDataTransferObject(route));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddWithInitialFlight([FromBody] AddRouteWithFlightRequest request)
    {
        if (request == null)
        {
            return BadRequest(NullRouteDataErrorMessage);
        }

        try
        {
            int routeIdentifier = await routeService.AddWithInitialFlightAsync(
                request.CompanyId,
                request.AirportId,
                request.RouteType,
                request.RecurrenceInterval,
                request.StartDate.ToDateTime(TimeOnly.MinValue),
                request.EndDate.ToDateTime(TimeOnly.MinValue),
                request.DepartureTime,
                request.ArrivalTime,
                request.Capacity,
                request.FlightNumber,
                request.RunwayId,
                request.GateId);

            return Ok(routeIdentifier);
        }
        catch (InvalidOperationException conflictException)
        {
            return Conflict(conflictException.Message);
        }
    }

    [HttpGet("normalize-type")]
    public ActionResult<string> NormalizeFlightType([FromQuery] string? routeType)
    {
        return Ok(new { value = routeService.NormalizeFlightType(routeType) });
    }

    [HttpPost("relevant-time")]
    public ActionResult<string> GetRelevantTime([FromBody] RouteTimeRequest request)
    {
        Route route = new()
        {
            RouteType = request.RouteType ?? string.Empty,
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime
        };

        return Ok(new { value = routeService.GetRelevantTime(route) });
    }

    private static RouteDTO MapToRouteDataTransferObject(Route route)
    {
        AirportDTO? airportDataTransferObject = route.Airport != null
            ? new AirportDTO(
                route.Airport.Id,
                route.Airport.AirportCode,
                route.Airport.City,
                route.Airport.Name)
            : null;

        CompanyDTO? companyDataTransferObject = route.Company != null
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
            airportDataTransferObject,
            companyDataTransferObject);
    }
}

public sealed record AddRouteWithFlightRequest(
    int CompanyId,
    int AirportId,
    string RouteType,
    int RecurrenceInterval,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOnly DepartureTime,
    TimeOnly ArrivalTime,
    int Capacity,
    string FlightNumber,
    int RunwayId,
    int GateId);

public sealed record RouteTimeRequest(string? RouteType, TimeOnly DepartureTime, TimeOnly ArrivalTime);
```

- [ ] **Step 2: Build to verify this file compiles cleanly**

```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected: remaining errors only in `AirportController.cs`.

- [ ] **Step 3: Commit**

```bash
git add AirportApp/AirportApp.Api/Controllers/RoutesController.cs
git commit -m "feat: update RoutesController to return RouteDTO and use explicit parameter names"
```

---

## Task 12: Update AirportController — return AirportDTO from GET

**Files:**
- Modify: `AirportApp/AirportApp.Api/Controllers/AirportController.cs`

The GET endpoints currently return raw `Airport` entities. Map them through `AirportDTO` (which now includes `name`). POST and PUT request records already include `AirportName` — no change needed there.

- [ ] **Step 1: Replace the file**

Replace the entire contents of `AirportApp/AirportApp.Api/Controllers/AirportController.cs`:

```csharp
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirportsController : ControllerBase
{
    private readonly IAirportService airportService;

    public AirportsController(IAirportService airportService)
    {
        this.airportService = airportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AirportDTO>>> GetAllAirports()
    {
        IEnumerable<Airport> airports = await airportService.GetAllAirportsAsync();
        return Ok(airports.Select(airport =>
            new AirportDTO(airport.Id, airport.AirportCode, airport.City, airport.Name)));
    }

    [HttpGet("{airportId}")]
    public async Task<ActionResult<AirportDTO>> GetAirportById(int airportId)
    {
        Airport? airport = await airportService.GetAirportByIdAsync(airportId);
        if (airport == null)
        {
            return NotFound();
        }

        return Ok(new AirportDTO(airport.Id, airport.AirportCode, airport.City, airport.Name));
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddAirport([FromBody] AddAirportRequest request)
    {
        try
        {
            Airport newAirport = new Airport(request.AirportCode, request.City, request.AirportName);
            await airportService.AddAirportAsync(newAirport);
            return Ok(newAirport.Id);
        }
        catch (ArgumentException argumentException)
        {
            return BadRequest(argumentException.Message);
        }
    }

    [HttpPut("{airportId}")]
    public async Task<IActionResult> UpdateAirport(int airportId, [FromBody] SaveAirportRequest request)
    {
        Airport airportToSave = new Airport(airportId, request.AirportCode, request.City, request.AirportName);
        await airportService.SaveAirportAsync(airportToSave);
        return Ok();
    }

    [HttpDelete("{airportId}")]
    public async Task<IActionResult> DeleteAirportUsingId(int airportId)
    {
        await airportService.DeleteAirportAsync(airportId);
        return Ok();
    }

    [HttpGet("{airportId}/has-flights")]
    public async Task<ActionResult<bool>> HasFlights(int airportId)
    {
        bool hasFlights = await airportService.HasFlightsAsync(airportId);
        return Ok(hasFlights);
    }

    [HttpGet("{airportId}/delete-warning")]
    public async Task<ActionResult<string>> GetDeleteWarningMessage(int airportId)
    {
        string warningMessage = await airportService.GetDeleteWarningMessageAsync(airportId);
        return Ok(new { WarningMessage = warningMessage });
    }

    public record AddAirportRequest(string AirportCode, string AirportName, string City);
    public record SaveAirportRequest(string AirportCode, string AirportName, string City);
}
```

- [ ] **Step 2: Build to verify the solution is clean**

```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected:
```
Build succeeded.
    0 Error(s)
```

- [ ] **Step 3: Commit**

```bash
git add AirportApp/AirportApp.Api/Controllers/AirportController.cs
git commit -m "feat: update AirportController GET responses to return AirportDTO including name"
```

---

## Task 13: Update TicketController — Duty Free shop filtering endpoints

**Files:**
- Modify: `AirportApp/AirportApp.Api/Controllers/TicketController.cs`

The 921 project tracked tickets against Duty Free shops by subcategory name. In the merged domain, `ComplaintTicketSubcategory.SubcategoryExternalReferenceId` holds the **Shop ID**. Add two endpoints to query tickets by shop. Filter in-memory using the already-injected `ticketService.GetAllTicketsAsync()` — no new repository methods needed.

- [ ] **Step 1: Add the two shop endpoints inside TicketController**

Inside `TicketController`, after the existing `GetTicketCountBySubcategory` method (line ~128), add:

```csharp
        [HttpGet("by-shop/{shopId:int}")]
        public async Task<ActionResult<IEnumerable<ComplaintTicket>>> GetByShopAsync(int shopId)
        {
            IEnumerable<ComplaintTicket> allTickets = await ticketService.GetAllTicketsAsync();

            IEnumerable<ComplaintTicket> ticketsForShop = allTickets
                .Where(ticket => ticket.Subcategory != null &&
                                 ticket.Subcategory.SubcategoryExternalReferenceId == shopId);

            return Ok(ticketsForShop);
        }

        [HttpGet("count/by-shop/{shopId:int}")]
        public async Task<ActionResult<int>> GetTicketCountByShopAsync(int shopId)
        {
            IEnumerable<ComplaintTicket> allTickets = await ticketService.GetAllTicketsAsync();

            int ticketCount = allTickets
                .Count(ticket => ticket.Subcategory != null &&
                                 ticket.Subcategory.SubcategoryExternalReferenceId == shopId);

            return Ok(ticketCount);
        }
```

- [ ] **Step 2: Build**

```
dotnet build AirportApp/AirportApp.Api/AirportApp.Api.csproj
```

Expected:
```
Build succeeded.
    0 Error(s)
```

- [ ] **Step 3: Commit**

```bash
git add AirportApp/AirportApp.Api/Controllers/TicketController.cs
git commit -m "feat: add by-shop ticket filtering endpoints for Duty Free shop subcategory support"
```

---

## Task 14: Swagger smoke test

**Files:** (none modified — verification only)

- [ ] **Step 1: Start the API**

From `AirportApp/AirportApp.Api/`:
```
dotnet run
```

Expected output includes a line like:
```
Now listening on: https://localhost:7278
```

- [ ] **Step 2: Open Swagger UI**

Navigate to `https://localhost:7278/swagger` in a browser.

Expected: Swagger UI loads showing "Airport API v1" with all controllers listed.

- [ ] **Step 3: Smoke test FlightDTO**

In Swagger, call `GET /api/Flight/{id}` with `id = 1`.

Expected response body includes `runwayId` as a field (value `1` based on seed data):
```json
{
  "id": 1,
  "routeId": 1,
  "gateId": 1,
  "runwayId": 1,
  "date": "2026-05-20T08:00:00",
  "flightNumber": "BA101",
  "route": {
    "startDate": "2026-01-01",
    "endDate": "2026-12-31",
    "departureTime": "08:00:00",
    "arrivalTime": "11:00:00",
    ...
  }
}
```

- [ ] **Step 4: Smoke test AirportDTO**

Call `GET /api/Airports`.

Expected: each airport object includes a `name` field, e.g.:
```json
{ "id": 1, "airportCode": "LHR", "city": "London", "name": "London Heathrow" }
```

- [ ] **Step 5: Smoke test RouteDTO**

Call `GET /api/routes`.

Expected: each route has `startDate`, `endDate`, `departureTime`, `arrivalTime` as separate fields — no combined DateTime.

- [ ] **Step 6: Smoke test shop ticket endpoint**

Call `GET /api/Ticket/by-shop/1`.

Expected: HTTP 200 with an array (empty is fine — no seed data links tickets to shops yet).

- [ ] **Step 7: Stop the API and do a final commit**

Stop with `Ctrl+C`, then:

```bash
git add .
git commit -m "feat: complete API DTO, endpoint, and infrastructure updates for merged project"
```
