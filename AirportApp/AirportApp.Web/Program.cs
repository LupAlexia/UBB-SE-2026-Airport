using AirportApp.ClassLibrary.Proxy;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5043/";

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/ChooseRole";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = ".AirportApp.Web.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Admin", "Employee", "Manager"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<WebUserSession>();

builder.Services.AddHttpClient<IUserService, UserServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IReviewService, ReviewServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IFAQService, FAQServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IMessageService, MessageServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IChatService, ChatServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IComplaintTicketService, ComplaintTicketServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IComplaintTicketCategoryService, ComplaintTicketCategoryServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IComplaintTicketSubcategoryService, ComplaintTicketSubcategoryServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IDashboardService, DashboardServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IAdministratorService, AdministratorServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IAuthService, AuthServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IMembershipService, MembershipServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IDecisionTreeService, DecisionTreeServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IFlightSearchService, FlightSearchServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IPricingService, PricingServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IBookingService, BookingServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddHttpClient<IAirportService, AirportServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<ICompanyService, CompanyServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IRunwayService, RunwayServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IGateService, GateServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IEmployeeService, EmployeeServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IFlightService, FlightServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IRouteService, RouteServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IFlightRouteService, FlightRouteServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IEmployeeFlightService, EmployeeFlightServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IClientService, ClientServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IManagerService, ManagerServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IShopService, ShopServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IShopItemService, ShopItemServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<ICartService, CartServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IReservationService, ReservationServiceProxy>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=ChooseRole}/{id?}");

app.Run();

