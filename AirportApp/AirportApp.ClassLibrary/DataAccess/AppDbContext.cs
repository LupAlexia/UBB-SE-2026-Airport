using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.EntityFrameworkCore;

namespace AirportApp.ClassLibrary.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Airport> Airports { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Gate> Gates { get; set; }
    public DbSet<Runway> Runways { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeFlight> EmployeeFlights { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<ShopItem> ShopItems { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Sender> Senders { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<BotMessage> BotMessages { get; set; }
    public DbSet<FAQEntry> Faqs { get; set; }
    public DbSet<FAQNode> FaqNodes { get; set; }
    public DbSet<FAQOption> FaqOptions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ComplaintTicket> Tickets { get; set; }
    public DbSet<ComplaintTicketCategory> TicketCategories { get; set; }
    public DbSet<ComplaintTicketSubcategory> TicketSubcategories { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MembershipAddonDiscount> MembershipAddonDiscounts { get; set; }
    public DbSet<AddOn> AddOns { get; set; }
    public DbSet<FlightTicket> FlightTickets { get; set; }


    public AppDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sender>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<User>("User")
            .HasValue<Administrator>("Administrator");

        modelBuilder.Entity<Flight>(flightBuilder =>
        {
            flightBuilder.HasOne(flight => flight.Gate)
                .WithMany()
                .HasForeignKey("GateId")
                .OnDelete(DeleteBehavior.Restrict);

            flightBuilder.HasOne(flight => flight.Route)
                .WithMany()
                .HasForeignKey("RouteId")
                .OnDelete(DeleteBehavior.Restrict);

            flightBuilder.HasOne(flight => flight.Runway)
                .WithMany()
                .HasForeignKey("RunwayId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ComplaintTicket>(ticketBuilder =>
        {
            ticketBuilder.HasOne(ticket => ticket.Category)
                .WithMany()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Restrict);

            ticketBuilder.HasOne(ticket => ticket.Subcategory)
                .WithMany()
                .HasForeignKey("SubcategoryId")
                .OnDelete(DeleteBehavior.Restrict);

            ticketBuilder.HasOne(ticket => ticket.Creator)
                .WithMany()
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Message>(messageBuilder =>
        {
            messageBuilder.HasOne(message => message.Sender)
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Restrict);

            messageBuilder.HasOne(message => message.Chat)
                .WithMany(chat => chat.Messages)
                .HasForeignKey("ChatId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BotMessage>(botMessageBuilder =>
        {
            botMessageBuilder.HasOne(botMessage => botMessage.Sender)
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Restrict);

            botMessageBuilder.HasOne(botMessage => botMessage.Chat)
                .WithMany()
                .HasForeignKey("ChatId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmployeeFlight>()
            .HasKey("EmployeeId", "FlightId");

        modelBuilder.Entity<MembershipAddonDiscount>()
            .HasKey("MembershipId", "AddOnId");

        modelBuilder.Entity<FlightTicket>()
            .HasMany(flightTicket => flightTicket.SelectedAddOns)
            .WithMany(addOn => addOn.Tickets)
            .UsingEntity(joinTable => joinTable.ToTable("FlightTicket_AddOns"));

        modelBuilder.Entity<FAQNode>(nodeBuilder =>
        {
            nodeBuilder.ToTable("FAQNodes");
            nodeBuilder.HasKey(node => node.NodeId);
            nodeBuilder.HasMany(node => node.Options)
                .WithOne()
                .HasForeignKey("ParentNodeId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FAQOption>(optionBuilder =>
        {
            optionBuilder.ToTable("FAQOptions");
            optionBuilder.HasKey(option => option.OptionId);
            optionBuilder.HasOne(option => option.NextOption)
                .WithMany()
                .HasForeignKey("NextNodeId")
                .IsRequired(false);
        });

        modelBuilder.Entity<Gate>()
            .HasOne(gate => gate.Airport)
            .WithMany(airport => airport.Gates)
            .HasForeignKey("AirportId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>(routeBuilder =>
        {
            routeBuilder.Property(route => route.DepartureTime);
            routeBuilder.Property(route => route.ArrivalTime);
            routeBuilder.Property(route => route.StartDate);
            routeBuilder.Property(route => route.EndDate);
        });

        modelBuilder.Entity<Company>()
            .HasOne(company => company.Manager)
            .WithMany()
            .HasForeignKey("ManagerId")
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        SeedProjectData(modelBuilder);
    }

    private void SeedProjectData(ModelBuilder modelBuilder)
    {
        SeedSystemIdentities(modelBuilder);
        SeedAviationLogistics(modelBuilder);
        SeedCommercialOperations(modelBuilder);
        SeedSupportInfrastructure(modelBuilder);
        SeedSocialAndTransactions(modelBuilder);
    }

    private void SeedSystemIdentities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sender>().HasData(
            new { Id = -1, FullName = "Carlos", EmailAddress = "customer-support@cloudspritzers.com", Discriminator = "Bot" },
            new { Id = -2, FullName = "System Admin", EmailAddress = "admin@airport.com", Discriminator = "Administrator" }
        );

        modelBuilder.Entity<User>().HasData(
        new User(101, "Alice Traveler", "alice@travel.com"),
        new User(102, "Bob Voyager", "bob@voyage.com"),
        new User(103, "Mia Passenger", "mia@example.com")
    );

        modelBuilder.Entity<Manager>().HasData(
            new Manager(1, "Marcel Ionescu", "marcel@airport.com", "0745000001"),
            new Manager(2, "Elena Vasilescu", "elena@airport.com", "0745000002"),
            new Manager(3, "Victor Radu", "victor@airport.com", "0745000003")
        );

        modelBuilder.Entity<Employee>().HasData(
            new { Id = 1, Name = "Andrei Popescu", Role = EmployeeRoleEnum.Pilot, Birthday = new DateOnly(1985, 5, 12), Salary = 12000, HiringDate = new DateOnly(2020, 1, 1) },
            new { Id = 2, Name = "Maria Ionescu", Role = EmployeeRoleEnum.FlightAttendant, Birthday = new DateOnly(1995, 9, 20), Salary = 7000, HiringDate = new DateOnly(2022, 6, 15) },
            new { Id = 3, Name = "Vlad Georgescu", Role = EmployeeRoleEnum.CoPilot, Birthday = new DateOnly(1988, 11, 3), Salary = 10000, HiringDate = new DateOnly(2021, 3, 10) },
            new { Id = 4, Name = "Elena Dumitrescu", Role = EmployeeRoleEnum.FlightAttendant, Birthday = new DateOnly(1997, 2, 14), Salary = 6800, HiringDate = new DateOnly(2023, 4, 5) },
            new { Id = 5, Name = "Cristian Matei", Role = EmployeeRoleEnum.Pilot, Birthday = new DateOnly(1982, 8, 30), Salary = 13500, HiringDate = new DateOnly(2015, 11, 1) },
            new { Id = 6, Name = "Anca Stoica", Role = EmployeeRoleEnum.FlightDispatcher, Birthday = new DateOnly(1990, 12, 25), Salary = 8500, HiringDate = new DateOnly(2019, 2, 20) },
            new { Id = 7, Name = "Mihai Enache", Role = EmployeeRoleEnum.CoPilot, Birthday = new DateOnly(1992, 4, 15), Salary = 9500, HiringDate = new DateOnly(2022, 8, 1) },
            new { Id = 8, Name = "Sonia Marinescu", Role = EmployeeRoleEnum.FlightAttendant, Birthday = new DateOnly(1994, 7, 10), Salary = 7200, HiringDate = new DateOnly(2021, 5, 12) }
        );

        modelBuilder.Entity<Customer>().HasData(
            new { Id = 101, Email = "customer1@gmail.com", Username = "traveler_one", PasswordHash = "HASH123", MembershipId = 1 },
            new { Id = 102, Email = "customer2@yahoo.com", Username = "sky_high", PasswordHash = "HASH456", MembershipId = 2 },
            new { Id = 103, Email = "customer3@outlook.com", Username = "globe_trotter", PasswordHash = "HASH789", MembershipId = 3 },
            new { Id = 104, Email = "customer4@gmail.com", Username = "business_flyer", PasswordHash = "HASH000", MembershipId = 4 },
            new { Id = 105, Email = "customer5@yahoo.com", Username = "vacation_mode", PasswordHash = "HASH111", MembershipId = 1 }
        );

        modelBuilder.Entity<Membership>().HasData(
            new Membership(1, "Standard", 0f),
            new Membership(2, "Silver", 5f),
            new Membership(3, "Gold", 15f),
            new Membership(4, "Platinum", 25f)
        );
    }

    private void SeedAviationLogistics(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>().HasData(
            new Airport(1, "LHR", "London", "London Heathrow"),
            new Airport(2, "JFK", "New York", "John F. Kennedy"),
            new Airport(3, "CLJ", "Cluj-Napoca", "Cluj International"),
            new Airport(4, "CDG", "Paris", "Charles de Gaulle"),
            new Airport(5, "DXB", "Dubai", "Dubai International")
        );

        modelBuilder.Entity<Company>().HasData(
            new { Id = 1, Name = "British Airways", ManagerId = 1 },
            new { Id = 2, Name = "Delta Airlines", ManagerId = 2 },
            new { Id = 3, Name = "Wizz Air", ManagerId = 3 },
            new { Id = 4, Name = "Lufthansa", ManagerId = 2 },
            new { Id = 5, Name = "Emirates", ManagerId = 1 }
        );

        modelBuilder.Entity<Gate>().HasData(
            new { Id = 1, GateName = "A1", AirportId = 1 }, new { Id = 2, GateName = "A2", AirportId = 1 },
            new { Id = 3, GateName = "B1", AirportId = 2 }, new { Id = 4, GateName = "B2", AirportId = 2 },
            new { Id = 5, GateName = "C1", AirportId = 3 }, new { Id = 6, GateName = "C2", AirportId = 3 }
        );

        modelBuilder.Entity<Runway>().HasData(
            new { Id = 1, Name = "R-09L", HandleTime = 15 },
            new { Id = 2, Name = "R-27R", HandleTime = 12 },
            new { Id = 3, Name = "R-18", HandleTime = 20 }
        );

        modelBuilder.Entity<Route>().HasData(
            new { Id = 1, RouteType = "DEP", CompanyId = 1, AirportId = 1, RecurrenceInterval = 1, StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 12, 31), DepartureTime = new TimeOnly(8, 0), ArrivalTime = new TimeOnly(11, 0), Capacity = 180 },
            new { Id = 2, RouteType = "ARR", CompanyId = 2, AirportId = 2, RecurrenceInterval = 1, StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 12, 31), DepartureTime = new TimeOnly(14, 0), ArrivalTime = new TimeOnly(18, 30), Capacity = 220 },
            new { Id = 3, RouteType = "DEP", CompanyId = 3, AirportId = 3, RecurrenceInterval = 2, StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 12, 31), DepartureTime = new TimeOnly(6, 30), ArrivalTime = new TimeOnly(9, 45), Capacity = 150 }
        );

        modelBuilder.Entity<Flight>().HasData(
            new { Id = 1, Date = new DateTime(2026, 5, 20, 8, 0, 0), FlightNumber = "BA101", RouteId = 1, RunwayId = 1, GateId = 1 },
            new { Id = 2, Date = new DateTime(2026, 5, 21, 14, 0, 0), FlightNumber = "DL455", RouteId = 2, RunwayId = 2, GateId = 3 },
            new { Id = 3, Date = new DateTime(2026, 5, 22, 6, 30, 0), FlightNumber = "W6 3301", RouteId = 3, RunwayId = 3, GateId = 5 }
        );
    }

    private void SeedCommercialOperations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shop>().HasData(
            new { Id = 1, Name = "Duty Free Global", Type = "Retail", ManagerId = 1 },
            new { Id = 2, Name = "Starbucks", Type = "Coffee", ManagerId = 2 },
            new { Id = 3, Name = "Book World", Type = "Books", ManagerId = 3 }
        );

        modelBuilder.Entity<ShopItem>().HasData(
            new { Id = 1, Name = "Perfume", Description = "Luxury scent", Price = 85.0f, Quantity = 50, Photo = "perfume.jpg", ShopId = 1 },
            new { Id = 2, Name = "Whiskey", Description = "Single Malt", Price = 45.0f, Quantity = 30, Photo = "whiskey.jpg", ShopId = 1 },
            new { Id = 3, Name = "Caffe Latte", Description = "Grande size", Price = 5.5f, Quantity = 200, Photo = "coffee.jpg", ShopId = 2 },
            new { Id = 4, Name = "Travel Pillow", Description = "Memory foam", Price = 25.0f, Quantity = 100, Photo = "pillow.jpg", ShopId = 3 }
        );

        modelBuilder.Entity<AddOn>().HasData(
            new AddOn(1, "Extra 20kg Bag", 40f),
            new AddOn(2, "Priority Boarding", 15f),
            new AddOn(3, "In-flight Meal", 12f),
            new AddOn(4, "Fast Track Security", 10f),
            new AddOn(5, "Lounge Access", 35f)
        );
    }

    private void SeedSupportInfrastructure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FAQNode>().HasData(
            new { NodeId = 1, QuestionText = "How can I help you?", IsFinalAnswer = false },
            new { NodeId = 2, QuestionText = "What's wrong with your baggage?", IsFinalAnswer = false },
            new { NodeId = 3, QuestionText = "Contact the Lost & Found desk at Level 1.", IsFinalAnswer = true },
            new { NodeId = 4, QuestionText = "You can change seats in the My Booking app.", IsFinalAnswer = true },
            new { NodeId = 5, QuestionText = "Please go to the baggage services desk or file a lost baggage report at the arrivals hall.", IsFinalAnswer = true },
            new { NodeId = 6, QuestionText = "Take photos and report the damage at the baggage service desk before leaving the airport.", IsFinalAnswer = true },
            new { NodeId = 7, QuestionText = "Use your reference number to track the bag and contact baggage services if it does not arrive.", IsFinalAnswer = true }
        );

        modelBuilder.Entity<FAQOption>().HasData(
            new { OptionId = 1, Label = "Baggage Issues", ParentNodeId = 1, NextNodeId = 2, BotMessageId = (int?)null },
            new { OptionId = 2, Label = "Seat Selection", ParentNodeId = 1, NextNodeId = 4, BotMessageId = (int?)null },
            new { OptionId = 3, Label = "Lost baggage", ParentNodeId = 2, NextNodeId = 5, BotMessageId = (int?)null },
            new { OptionId = 4, Label = "Damaged baggage", ParentNodeId = 2, NextNodeId = 6, BotMessageId = (int?)null },
            new { OptionId = 5, Label = "Delayed baggage", ParentNodeId = 2, NextNodeId = 7, BotMessageId = (int?)null }
        );

        modelBuilder.Entity<ComplaintTicketCategory>().HasData(
            new ComplaintTicketCategory(1, "Technical Support", ComplaintTicketUrgencyLevelEnum.MEDIUM),
            new ComplaintTicketCategory(2, "Billing", ComplaintTicketUrgencyLevelEnum.HIGH),
            new ComplaintTicketCategory(3, "General Feedback", ComplaintTicketUrgencyLevelEnum.LOW)
        );
    }

    private void SeedSocialAndTransactions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>().HasData(
            new { Id = 1, UserId = 101, Status = ChatStatus.Active },
            new { Id = 2, UserId = 102, Status = ChatStatus.Closed }
        );

        modelBuilder.Entity<Review>().HasData(
            new { Id = 1, Message = "Great airport, very clean!", DutyFreeRating = 5, FlightExperienceRating = 4, StaffFriendlinessRating = 5, CleanlinessRating = 5, UserId = 101 },
            new { Id = 2, Message = "Security took too long.", DutyFreeRating = 3, FlightExperienceRating = 2, StaffFriendlinessRating = 3, CleanlinessRating = 4, UserId = 102 }
        );
    }

}