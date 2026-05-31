# API — DTOs, Endpoints & Infrastructure Design

**Date:** 2026-05-31  
**Owner:** Kun Mihai  
**Branch:** mihai/api-integration

---

## Context

Two projects were merged into a single solution at `AirportApp/`:
- **924** — WinUI desktop app with complaint tickets, shops, duty-free, CRUD flights/routes/airports
- **921** — ASP.NET API focused on routes, runways, duty-free shop tickets (simpler domain model)

The merged project has a `ClassLibrary` containing all domain models, services, and repositories, and an `AirportApp.Api` project with controllers. The API currently has an empty `Program.cs` (no DI, no DB, no CORS), no `appsettings.json`, outdated DTOs, and several controllers that need updating to reflect the merged domain.

---

## Coding Standards

All new and modified code must follow these rules:
- Explicit, fully-spelled-out variable and parameter names — no abbreviations
- No inline comments unless a constraint or workaround is non-obvious
- No docstrings or multi-line comment blocks

---

## Section 1 — Infrastructure: Program.cs and appsettings

### appsettings.example.json (new file, committed)

`AirportApp.Api/appsettings.example.json` — the real `appsettings.json` is gitignored.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AirportAppDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5043", "https://localhost:7278"]
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

### Program.cs

Registers:
1. `AppDbContext` with SQL Server connection string from configuration
2. All services and repositories via `AddAirportServices()` extension method
3. CORS policy reading allowed origins from configuration
4. Controllers with JSON converters for `DateOnly` and `TimeOnly`
5. Swagger with title "Airport API" version "v1"

```
builder.Services.AddDbContext<AppDbContext>(...)
builder.Services.AddAirportServices()       // extension in ClassLibrary
builder.Services.AddCors(...)
builder.Services.AddControllers()
    .AddJsonOptions(...)                    // DateOnly + TimeOnly converters
builder.Services.AddEndpointsApiExplorer()
builder.Services.AddSwaggerGen(...)

app.UseCors(...)
app.UseSwagger() / UseSwaggerUI()           // always, not just Development
app.UseHttpsRedirection()
app.UseAuthorization()
app.MapControllers()
```

### ServiceRegistrationExtensions.cs (new file in ClassLibrary)

Located at `AirportApp.ClassLibrary/ServiceRegistrationExtensions.cs`.  
Registers all repository and service pairs using `AddScoped`:

- `IFlightRepository` → `FlightRepository`
- `IFlightService` → `FlightService`
- `IFlightRouteService` → `FlightRouteService`
- `IFlightSearchService` → `FlightSearchService`
- `IRouteRepository` → `RouteRepository`
- `IRouteService` → `RouteService`
- `IRunwayRepository` → `RunwayRepository`
- `IRunwayService` → `RunwayService`
- `IAirportRepository` → `AirportRepository`
- `IAirportService` → `AirportService`
- `IGateRepository` → `GateRepository`
- `IGateService` → `GateService`
- `ICompanyRepository` → `CompanyRepository`
- `ICompanyService` → `CompanyService`
- `IComplaintTicketRepository` → `ComplaintTicketRepository` (note: file is `ComplaintTicketRepostitory.cs`)
- `IComplaintTicketService` → `ComplaintTicketService`
- `IComplaintTicketCategoryRepository` → `ComplaintTicketCategoryRepository`
- `IComplaintTicketCategoryService` → `ComplaintTicketCategoryService`
- `IComplaintTicketSubcategoryRepository` → `ComplaintTicketSubcategoryRepository`
- `IComplaintTicketSubcategoryService` → `ComplaintTicketSubcategoryService`
- `IUserRepository` → `UserRepository`
- `IUserService` → `UserService`
- `IEmployeeRepository` → `EmployeeRepository`
- `IEmployeeService` → `EmployeeService`
- `IEmployeeFlightRepository` → `EmployeeFlightRepository`
- `IEmployeeFlightService` → `EmployeeFlightService`
- `IManagerRepository` → `ManagerRepository`
- `IManagerService` → `ManagerService`
- `IShopRepository` → `ShopRepository`
- `IShopService` → `ShopService`
- `IShopItemRepository` → `ShopItemRepository`
- `IShopItemService` → `ShopItemService`
- `IReviewRepository` → `ReviewRepository`
- `IReviewService` → `ReviewService`
- `IReservationRepository` → `ReservationRepository`
- `IReservationService` → `ReservationService`
- `IBookingService` → `BookingService`
- `ICancellationService` → `CancellationService`
- `ICartRepository` → `CartRepository`
- `ICartService` → `CartService`
- `IClientRepository` → `ClientRepository`
- `IClientService` → `ClientService`
- `IMessageRepository` → `MessageRepository`
- `IMessageService` → `MessageService`
- `IChatRepository` → `ChatRepository`
- `IChatService` → `ChatService`
- `IAdministratorRepository` → `AdministratorRepository`
- `IAdministratorService` → `AdministratorService`
- `IFAQRepository` → `FAQRepository`
- `IFAQService` → `FAQService`
- `IDecisionTreeRepository` → `DecisionTreeRepository`
- `IDecisionTreeService` → `DecisionTreeService`
- `IMembershipRepository` → `MembershipRepository`
- `IMembershipService` → `MembershipService`
- `IPricingService` → `PricingService`
- `IDashboardService` → `DashboardService`
- Also register `AutoMapper` profiles (`TicketMappingProfile`, `ChatMappingProfile`, `MessageMappingProfile`, `ReviewMappingProfile`, `FAQEntryMappingProfile`, `EmployeeMappingProfile`, `UserMappingProfile`)

---

## Section 2 — DTO Changes

### FlightDTO

**File:** `AirportApp.ClassLibrary/Entity/Dto/FlightDTO.cs`

Add `runwayId` field:

```csharp
public record FlightDTO(
    int id,
    int routeId,
    int gateId,
    int runwayId,
    DateTime date,
    string flightNumber,
    RouteDTO? route);
```

### RouteDTO

**File:** `AirportApp.ClassLibrary/Entity/Dto/RouteDTO.cs`

Replace the two combined `DateTime` fields with four separate `DateOnly`/`TimeOnly` fields, and add `recurrenceInterval`:

```csharp
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
```

This is a breaking change for `FlightController.cs` which constructed `RouteDTO` by combining `flight.Date + route.DepartureTime` — those callers will be updated to pass `route.StartDate`, `route.EndDate`, `route.DepartureTime`, `route.ArrivalTime` directly.

### AirportDTO

**File:** `AirportApp.ClassLibrary/Entity/Dto/AirportDTO.cs`

Add `name` field:

```csharp
public record AirportDTO(int id, string airportCode, string city, string name);
```

---

## Section 3 — Controller Updates

### FlightsController (POST/PUT — add Runway field)

**File:** `AirportApp.Api/Controllers/FlightsController.cs`

Introduce a `FlightRequestDTO` record (defined at the bottom of the file) to replace raw `Flight` entity in POST/PUT:

```csharp
public record FlightRequestDTO(
    string FlightNumber,
    int RouteId,
    DateTime DepartureDate,
    int RunwayId,
    int GateId);
```

- `POST /api/flights` — bind `[FromBody] FlightRequestDTO`, call `flightService.AddFlightAsync(...)`, return `FlightDTO`
- `PUT /api/flights/{flightId}` — bind `[FromBody] FlightRequestDTO`, call `flightService.UpdateFlightAsync(...)`, return `NoContent`
- GET methods continue to return `Flight` entity (no change)

### FlightController (921 search — update RouteDTO construction)

**File:** `AirportApp.Api/Controllers/FlightController.cs`

The `GetByIdAsync` and `GetByRouteAsync` methods manually construct `RouteDTO`. Update these constructions to match the new `RouteDTO` signature:
- Pass `route.StartDate`, `route.EndDate`, `route.DepartureTime`, `route.ArrivalTime` as separate fields
- Add `route.RecurrenceInterval`
- Add `runwayId` field (`flight.Runway?.Id ?? 0`) to `FlightDTO`
- Update `AirportDTO` construction to include `route.Airport.Name`

### RoutesController (GET responses — return RouteDTO)

**File:** `AirportApp.Api/Controllers/RoutesController.cs`

The `GetAll` and `GetById` actions currently return raw `Route` entities. Update them to map through `RouteDTO` so clients receive the consistent DTO format.

### AirportController (GET responses — return AirportDTO)

**File:** `AirportApp.Api/Controllers/AirportController.cs`

The `GetAllAirports` and `GetAirportById` actions return raw `Airport` entities. Map through `AirportDTO` (which now includes `name`).  
The `AddAirportRequest` and `SaveAirportRequest` records already include `AirportName` — no change needed to POST/PUT.

---

## Section 4 — TicketController Duty Free Shop Subcategory Endpoints

**File:** `AirportApp.Api/Controllers/TicketController.cs`

The 921 project tracked tickets against Duty Free shops by subcategory name. In the merged domain, `ComplaintTicketSubcategory.SubcategoryExternalReferenceId` stores the **Shop ID** that the subcategory belongs to.

Add two new endpoints:

**GET** `/api/ticket/by-shop/{shopId}` — returns all `ComplaintTicket` where `ticket.Subcategory.SubcategoryExternalReferenceId == shopId`

**GET** `/api/ticket/count/by-shop/{shopId}` — returns the count of tickets for a given shop

Both endpoints use the already-injected `ticketService.GetAllTicketsAsync()` and filter in-memory — no new repository or service methods required.

The existing `GET /api/ticket/count/subcategory?name=X` endpoint (merged from 921) remains unchanged.

---

## Data Flow Summary

```
Client
  │
  ├─ POST /api/flights          → FlightRequestDTO (with RunwayId)
  │                               → IFlightService.AddFlightAsync
  │
  ├─ GET  /api/flight/search    → FlightDTO (with runwayId, RouteDTO with DateOnly/TimeOnly)
  │
  ├─ GET  /api/airports         → AirportDTO (with name)
  │
  ├─ GET  /api/routes           → RouteDTO (DateOnly startDate/endDate + TimeOnly dep/arr)
  │
  ├─ POST /api/ticket           → CreateTicketDTO (existing, unchanged)
  ├─ GET  /api/ticket/by-shop/{shopId}        → IEnumerable<ComplaintTicket>
  └─ GET  /api/ticket/count/by-shop/{shopId}  → int
```

---

## Files to Create

| File | Purpose |
|---|---|
| `AirportApp.Api/appsettings.example.json` | Example config (committed) |
| `AirportApp.ClassLibrary/ServiceRegistrationExtensions.cs` | DI registration extension method |

## Files to Modify

| File | Change |
|---|---|
| `AirportApp.Api/Program.cs` | Full rewrite — DB, DI, CORS, Swagger, JSON converters |
| `AirportApp.ClassLibrary/Entity/Dto/FlightDTO.cs` | Add `runwayId` |
| `AirportApp.ClassLibrary/Entity/Dto/RouteDTO.cs` | Replace DateTime with DateOnly+TimeOnly, add recurrenceInterval |
| `AirportApp.ClassLibrary/Entity/Dto/AirportDTO.cs` | Add `name` |
| `AirportApp.Api/Controllers/FlightsController.cs` | POST/PUT use `FlightRequestDTO` |
| `AirportApp.Api/Controllers/FlightController.cs` | Update RouteDTO + FlightDTO construction |
| `AirportApp.Api/Controllers/RoutesController.cs` | Map GET responses to RouteDTO |
| `AirportApp.Api/Controllers/AirportController.cs` | Map GET responses to AirportDTO |
| `AirportApp.Api/Controllers/TicketController.cs` | Add by-shop endpoints |
