using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Proxy;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Services.Interfaces;
using AirportApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace AirportApp
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static Window MainWindow { get; private set; } = null!;
        public static IAuthService AuthService { get; private set; } = null!;
        public static IFlightSearchService FlightSearchService { get; private set; } = null!;
        public static IBookingService BookingService { get; private set; } = null!;
        public static IPricingService PricingService { get; private set; } = null!;
        public static IDashboardService DashboardService { get; private set; } = null!;
        public static ICancellationService CancellationService { get; private set; } = null!;
        public static IMembershipService MembershipService { get; private set; } = null!;
        public static IChatService ChatService { get; private set; } = null!;
        public static IComplaintTicketService ComplaintTicketService { get; private set; } = null!;
        public static IComplaintTicketCategoryService ComplaintTicketCategoryService { get; private set; } = null!;
        public static IComplaintTicketSubcategoryService ComplaintTicketSubcategoryService { get; private set; } = null!;
        public static IDecisionTreeService DecisionTreeService { get; private set; } = null!;
        public static IEmployeeService EmployeeService { get; private set; } = null!;
        public static IFAQService FAQService { get; private set; } = null!;
        public static IMessageService MessageService { get; private set; } = null!;
        public static IReviewService ReviewService { get; private set; } = null!;
        public static IUserService UserService { get; private set; } = null!;
        public static INavigationService NavigationService { get; private set; } = null!;
        public User? User { get; private set; }
        public Employee? Employee { get; private set; }
        public bool IsEmployee { get; set; }

        private const string DefaultApiBaseUrl = "http://localhost:5171/";
        private Window? window;

        public App()
        {
            InitializeComponent();
            Services = ConfigureServices();
        }

        public async Task<bool> SetUserAsync(int userId)
        {
            User = null;
            Employee = null;

            if (IsEmployee)
            {
                var employeeService = Services.GetRequiredService<IEmployeeService>();
                Employee = await employeeService.GetEmployeeByIdAsync(userId);
                return Employee != null;
            }

            var userService = Services.GetRequiredService<IUserService>();
            User = await userService.GetByIdAsync(userId);
            return User != null;
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(ReadConfiguredApiBaseUrl())
            });

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AirportApp.ClassLibrary.ServiceRegistrationExtensions).Assembly));

            // 924 services
            services.AddSingleton<IAuthService, AuthServiceProxy>();
            services.AddSingleton<IBookingService, BookingServiceProxy>();
            services.AddSingleton<ICancellationService, CancellationServiceProxy>();
            services.AddSingleton<IChatService, ChatServiceProxy>();
            services.AddSingleton<IComplaintTicketService, ComplaintTicketServiceProxy>();
            services.AddSingleton<IComplaintTicketCategoryService, ComplaintTicketCategoryServiceProxy>();
            services.AddSingleton<IComplaintTicketSubcategoryService, ComplaintTicketSubcategoryServiceProxy>();
            services.AddSingleton<IDashboardService, DashboardServiceProxy>();
            services.AddSingleton<IDecisionTreeService, DecisionTreeServiceProxy>();
            services.AddSingleton<IEmployeeService, EmployeeServiceProxy>();
            services.AddSingleton<IFAQService, FAQServiceProxy>();
            services.AddSingleton<IFlightSearchService, FlightSearchServiceProxy>();
            services.AddSingleton<IMembershipService, MembershipServiceProxy>();
            services.AddSingleton<IMessageService, MessageServiceProxy>();
            services.AddSingleton<IPricingService, PricingServiceProxy>();
            services.AddSingleton<IReviewService, ReviewServiceProxy>();
            services.AddSingleton<IUserService, UserServiceProxy>();

            // 921 services
            services.AddSingleton<IAirportService, AirportServiceProxy>();
            services.AddSingleton<ICartService, CartServiceProxy>();
            services.AddSingleton<IClientService, ClientServiceProxy>();
            services.AddSingleton<ICompanyService, CompanyServiceProxy>();
            services.AddSingleton<IEmployeeFlightService, EmployeeFlightServiceProxy>();
            services.AddSingleton<IFlightRouteService, FlightRouteServiceProxy>();
            services.AddSingleton<IGateService, GateServiceProxy>();
            services.AddSingleton<IManagerService, ManagerServiceProxy>();
            services.AddSingleton<IReservationService, ReservationServiceProxy>();
            services.AddSingleton<IRunwayService, RunwayServiceProxy>();
            services.AddSingleton<IShopItemService, ShopItemServiceProxy>();
            services.AddSingleton<IShopService, ShopServiceProxy>();

            services.AddSingleton<INavigationService, Services.NavigationService>();
            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<MainWindow>();

            // Shared user session for DutyFree
            services.AddSingleton<AirportLib.Domain.User.UserSession>();

            // 924 ViewModels
            services.AddTransient<AirportApp.Src.ViewModel.AddReviewViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.AllReviewsViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.AuthViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.BookingViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.ChatViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.ChoosingPageViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.ComplaintTicketViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.DashboardViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.EnterYourIdViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.FAQViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.FlightSearchViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.LandingViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.MaiBouleViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.MembershipViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.MessageViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.PassengerFormViewModel>();
            services.AddTransient<Func<AirportApp.Src.ViewModel.PassengerFormViewModel>>(sp => () => sp.GetRequiredService<AirportApp.Src.ViewModel.PassengerFormViewModel>());
            services.AddTransient<AirportApp.Src.ViewModel.UpperBarViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.YouSureViewModel>();

            // 921 ViewModels
            services.AddTransient<AirportApp.Src.ViewModel.HomeViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.SelectCompanyViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.HeaderViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.StaffLoginViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.StaffPageViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.CompanyViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.AirportAdminViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.AirportDashboardViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.EmployeesDashboardViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.FlightsDashboardViewModel>();

            // DutyFree ViewModels and interfaces
            services.AddTransient<AirportApp.Src.ViewModel.IDutyFreeLandingViewModel,
                AirportApp.Src.ViewModel.DutyFreeLandingViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.IShopPageViewModel,
                AirportApp.Src.ViewModel.ShopPageViewModel>();
            services.AddTransient<AirportApp.Src.ViewModel.ICartViewModel,
                AirportApp.Src.ViewModel.CartViewModel>();

            // DutyFree factory registrations for shop-context ViewModels
            services.AddTransient<Func<AirportApp.ClassLibrary.Entity.Domain.Shop,
                AirportApp.Src.ViewModel.IShopItemsViewModel>>(sp => (shop) =>
                new AirportApp.Src.ViewModel.ShopItemsViewModel(
                    sp.GetRequiredService<IShopItemService>(),
                    sp.GetRequiredService<ICartService>(),
                    sp.GetRequiredService<AirportLib.Domain.User.UserSession>(),
                    shop));

            services.AddTransient<Func<AirportApp.ClassLibrary.Entity.Domain.ShopItem,
                AirportApp.ClassLibrary.Entity.Domain.Shop,
                AirportApp.Src.ViewModel.IItemDetailsViewModel>>(sp => (item, shop) =>
                new AirportApp.Src.ViewModel.ItemDetailsViewModel(
                    sp.GetRequiredService<ICartService>(),
                    sp.GetRequiredService<IShopItemService>(),
                    sp.GetRequiredService<AirportLib.Domain.User.UserSession>(),
                    item,
                    shop));

            var provider = services.BuildServiceProvider();

            AuthService = provider.GetRequiredService<IAuthService>();
            FlightSearchService = provider.GetRequiredService<IFlightSearchService>();
            BookingService = provider.GetRequiredService<IBookingService>();
            PricingService = provider.GetRequiredService<IPricingService>();
            DashboardService = provider.GetRequiredService<IDashboardService>();
            CancellationService = provider.GetRequiredService<ICancellationService>();
            MembershipService = provider.GetRequiredService<IMembershipService>();
            ChatService = provider.GetRequiredService<IChatService>();
            ComplaintTicketService = provider.GetRequiredService<IComplaintTicketService>();
            ComplaintTicketCategoryService = provider.GetRequiredService<IComplaintTicketCategoryService>();
            ComplaintTicketSubcategoryService = provider.GetRequiredService<IComplaintTicketSubcategoryService>();
            DecisionTreeService = provider.GetRequiredService<IDecisionTreeService>();
            EmployeeService = provider.GetRequiredService<IEmployeeService>();
            FAQService = provider.GetRequiredService<IFAQService>();
            MessageService = provider.GetRequiredService<IMessageService>();
            ReviewService = provider.GetRequiredService<IReviewService>();
            UserService = provider.GetRequiredService<IUserService>();
            NavigationService = provider.GetRequiredService<INavigationService>();

            return provider;
        }

        private static string ReadConfiguredApiBaseUrl()
        {
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(settingsPath))
            {
                return DefaultApiBaseUrl;
            }

            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));
                if (document.RootElement.TryGetProperty("ApiBaseUrl", out var apiBaseUrlElement))
                {
                    var apiBaseUrl = apiBaseUrlElement.GetString();
                    if (!string.IsNullOrWhiteSpace(apiBaseUrl))
                    {
                        return apiBaseUrl;
                    }
                }
            }
            catch (JsonException)
            {
            }

            return DefaultApiBaseUrl;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            window = Services.GetRequiredService<MainWindow>();
            MainWindow = window;
            window.Activate();
        }
    }
}
