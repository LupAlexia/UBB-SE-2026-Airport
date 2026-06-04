using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AirportApp.ClassLibrary;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddAirportServices(this IServiceCollection services)
    {
        RegisterRepositories(services);
        services.AddScoped<IBotStrategy, NullBotStrategy>();
        services.AddScoped<BotEngineIdentity>();
        RegisterServices(services);
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ServiceRegistrationExtensions).Assembly));
        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IAddOnRepository, AddOnRepository>();
        services.AddScoped<IAdministratorRepository, AdministratorRepository>();
        services.AddScoped<IAirportRepository, EfAirportRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IComplaintTicketCategoryRepository, ComplaintTicketCategoryRepository>();
        services.AddScoped<IComplaintTicketRepository, ComplaintTicketRepository>();
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
        services.AddScoped<IMembershipPricingStrategyFactory, MembershipPricingStrategyFactory>();
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
