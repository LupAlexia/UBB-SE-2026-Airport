using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddOns",
                columns: table => new
                {
                    AddOn_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Base_Price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddOns", x => x.AddOn_Id);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Airport_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Airport_Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Airport_Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Client_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Client_Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Company_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Company_Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Employee_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Date_of_Birth = table.Column<DateOnly>(type: "date", nullable: false),
                    Hire_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Salary = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Employee_Id);
                });

            migrationBuilder.CreateTable(
                name: "FAQNodes",
                columns: table => new
                {
                    NodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFinalAnswer = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQNodes", x => x.NodeId);
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    FAQ_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Answer_Text = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    View_Count = table.Column<int>(type: "int", nullable: false),
                    Helpful_Votes = table.Column<int>(type: "int", nullable: false),
                    Not_Helpful_Votes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.FAQ_Id);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Manager_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Manager_Id);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Membership_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flight_Discount_Percentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Membership_Id);
                });

            migrationBuilder.CreateTable(
                name: "Runways",
                columns: table => new
                {
                    Runway_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Handle_Time = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runways", x => x.Runway_Id);
                });

            migrationBuilder.CreateTable(
                name: "Senders",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email_Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.Sender_Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategories",
                columns: table => new
                {
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Default_Urgency_Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategories", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "Gates",
                columns: table => new
                {
                    Gate_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gate_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AirportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gates", x => x.Gate_Id);
                    table.ForeignKey(
                        name: "FK_Gates_Airports_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airports",
                        principalColumn: "Airport_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Cart_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Cart_Id);
                    table.ForeignKey(
                        name: "FK_Carts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Client_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Route_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AirportId = table.Column<int>(type: "int", nullable: false),
                    Route_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Departure_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Arrival_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Departure_Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    Arrival_Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Recurrence_Interval = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Route_Id);
                    table.ForeignKey(
                        name: "FK_Routes_Airports_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airports",
                        principalColumn: "Airport_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Company_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Shop_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Shop_Id);
                    table.ForeignKey(
                        name: "FK_Shops_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Manager_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Customer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password_Hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MembershipId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Customer_Id);
                    table.ForeignKey(
                        name: "FK_Customers_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Membership_Id");
                });

            migrationBuilder.CreateTable(
                name: "Membership_Addon_Discounts",
                columns: table => new
                {
                    MembershipId = table.Column<int>(type: "int", nullable: false),
                    AddOnId = table.Column<int>(type: "int", nullable: false),
                    Discount_Percentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membership_Addon_Discounts", x => new { x.MembershipId, x.AddOnId });
                    table.ForeignKey(
                        name: "FK_Membership_Addon_Discounts_AddOns_AddOnId",
                        column: x => x.AddOnId,
                        principalTable: "AddOns",
                        principalColumn: "AddOn_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Membership_Addon_Discounts_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Membership_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Sender_Id);
                    table.ForeignKey(
                        name: "FK_Administrators_Senders_Sender_Id",
                        column: x => x.Sender_Id,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Sender_Id);
                    table.ForeignKey(
                        name: "FK_Users_Senders_Sender_Id",
                        column: x => x.Sender_Id,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketSubcategories",
                columns: table => new
                {
                    Subcategory_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subcategory_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    External_Reference_Id = table.Column<int>(type: "int", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSubcategories", x => x.Subcategory_Id);
                    table.ForeignKey(
                        name: "FK_TicketSubcategories_TicketCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "TicketCategories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Reservation_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationCartId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Reservation_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Reservation_Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Carts_ReservationCartId",
                        column: x => x.ReservationCartId,
                        principalTable: "Carts",
                        principalColumn: "Cart_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Flight_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    GateId = table.Column<int>(type: "int", nullable: false),
                    Departure_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Flight_Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RunwayId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Flight_Id);
                    table.ForeignKey(
                        name: "FK_Flights_Gates_GateId",
                        column: x => x.GateId,
                        principalTable: "Gates",
                        principalColumn: "Gate_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flights_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Route_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flights_Runways_RunwayId",
                        column: x => x.RunwayId,
                        principalTable: "Runways",
                        principalColumn: "Runway_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    ShopItem_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.ShopItem_Id);
                    table.ForeignKey(
                        name: "FK_ShopItems_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Shop_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Chat_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Chat_Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Chat_Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Duty_Free_Rating = table.Column<int>(type: "int", nullable: false),
                    Flight_Experience_Rating = table.Column<int>(type: "int", nullable: false),
                    Staff_Friendliness_Rating = table.Column<int>(type: "int", nullable: false),
                    Cleanliness_Rating = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Ticket_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Creation_Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Urgency_Level = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Ticket_Id);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TicketCategories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketSubcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "TicketSubcategories",
                        principalColumn: "Subcategory_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFlights",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFlights", x => new { x.EmployeeId, x.FlightId });
                    table.ForeignKey(
                        name: "FK_EmployeeFlights_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeFlights_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Flight_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightTickets",
                columns: table => new
                {
                    Ticket_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    Seat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Passenger_First_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Passenger_Last_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Passenger_Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Passenger_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTickets", x => x.Ticket_Id);
                    table.ForeignKey(
                        name: "FK_FlightTickets_Customers_UserId",
                        column: x => x.UserId,
                        principalTable: "Customers",
                        principalColumn: "Customer_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTickets_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Flight_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItem_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItem_Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Cart_Id");
                    table.ForeignKey(
                        name: "FK_CartItems_ShopItems_ShopItemId",
                        column: x => x.ShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "ShopItem_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BotMessages",
                columns: table => new
                {
                    Message_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message_Text = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotMessages", x => x.Message_Id);
                    table.ForeignKey(
                        name: "FK_BotMessages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Chat_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotMessages_Senders_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Message_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message_Text = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Message_Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Chat_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Senders_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightTicket_AddOns",
                columns: table => new
                {
                    SelectedAddOnsId = table.Column<int>(type: "int", nullable: false),
                    TicketsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTicket_AddOns", x => new { x.SelectedAddOnsId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_FlightTicket_AddOns_AddOns_SelectedAddOnsId",
                        column: x => x.SelectedAddOnsId,
                        principalTable: "AddOns",
                        principalColumn: "AddOn_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTicket_AddOns_FlightTickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "FlightTickets",
                        principalColumn: "Ticket_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQOptions",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextNodeId = table.Column<int>(type: "int", nullable: true),
                    BotMessageId = table.Column<int>(type: "int", nullable: true),
                    ParentNodeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQOptions", x => x.OptionId);
                    table.ForeignKey(
                        name: "FK_FAQOptions_BotMessages_BotMessageId",
                        column: x => x.BotMessageId,
                        principalTable: "BotMessages",
                        principalColumn: "Message_Id");
                    table.ForeignKey(
                        name: "FK_FAQOptions_FAQNodes_NextNodeId",
                        column: x => x.NextNodeId,
                        principalTable: "FAQNodes",
                        principalColumn: "NodeId");
                    table.ForeignKey(
                        name: "FK_FAQOptions_FAQNodes_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "FAQNodes",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AddOns",
                columns: new[] { "AddOn_Id", "Base_Price", "Name" },
                values: new object[,]
                {
                    { 1, 40f, "Extra 20kg Bag" },
                    { 2, 15f, "Priority Boarding" },
                    { 3, 12f, "In-flight Meal" },
                    { 4, 10f, "Fast Track Security" },
                    { 5, 35f, "Lounge Access" }
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Airport_Id", "Airport_Code", "City", "Name" },
                values: new object[,]
                {
                    { 1, "LHR", "London", "London Heathrow" },
                    { 2, "JFK", "New York", "John F. Kennedy" },
                    { 3, "CLJ", "Cluj-Napoca", "Cluj International" },
                    { 4, "CDG", "Paris", "Charles de Gaulle" },
                    { 5, "DXB", "Dubai", "Dubai International" }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Company_Id", "Name" },
                values: new object[,]
                {
                    { 1, "British Airways" },
                    { 2, "Delta Airlines" },
                    { 3, "Wizz Air" },
                    { 4, "Lufthansa" },
                    { 5, "Emirates" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Employee_Id", "Date_of_Birth", "Hire_Date", "Name", "Role", "Salary" },
                values: new object[,]
                {
                    { 1, new DateOnly(1985, 5, 12), new DateOnly(2020, 1, 1), "Andrei Popescu", 1, 12000 },
                    { 2, new DateOnly(1995, 9, 20), new DateOnly(2022, 6, 15), "Maria Ionescu", 3, 7000 },
                    { 3, new DateOnly(1988, 11, 3), new DateOnly(2021, 3, 10), "Vlad Georgescu", 2, 10000 },
                    { 4, new DateOnly(1997, 2, 14), new DateOnly(2023, 4, 5), "Elena Dumitrescu", 3, 6800 },
                    { 5, new DateOnly(1982, 8, 30), new DateOnly(2015, 11, 1), "Cristian Matei", 1, 13500 },
                    { 6, new DateOnly(1990, 12, 25), new DateOnly(2019, 2, 20), "Anca Stoica", 4, 8500 },
                    { 7, new DateOnly(1992, 4, 15), new DateOnly(2022, 8, 1), "Mihai Enache", 2, 9500 },
                    { 8, new DateOnly(1994, 7, 10), new DateOnly(2021, 5, 12), "Sonia Marinescu", 3, 7200 }
                });

            migrationBuilder.InsertData(
                table: "FAQNodes",
                columns: new[] { "NodeId", "IsFinalAnswer", "QuestionText" },
                values: new object[,]
                {
                    { 1, false, "How can I help you?" },
                    { 2, false, "What's wrong with your baggage?" },
                    { 3, true, "Contact the Lost & Found desk at Level 1." },
                    { 4, true, "You can change seats in the My Booking app." }
                });

            migrationBuilder.InsertData(
                table: "FAQOptions",
                columns: new[] { "OptionId", "BotMessageId", "Label", "NextNodeId", "ParentNodeId" },
                values: new object[,]
                {
                    { 1, null, "Baggage Issues", null, null },
                    { 2, null, "Seat Selection", null, null },
                    { 3, null, "Lost Item", null, null }
                });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Manager_Id", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "marcel@airport.com", "Marcel Ionescu", "0745000001" },
                    { 2, "elena@airport.com", "Elena Vasilescu", "0745000002" },
                    { 3, "victor@airport.com", "Victor Radu", "0745000003" }
                });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Membership_Id", "Flight_Discount_Percentage", "Name" },
                values: new object[,]
                {
                    { 1, 0f, "Standard" },
                    { 2, 5f, "Silver" },
                    { 3, 15f, "Gold" },
                    { 4, 25f, "Platinum" }
                });

            migrationBuilder.InsertData(
                table: "Runways",
                columns: new[] { "Runway_Id", "Handle_Time", "Name" },
                values: new object[,]
                {
                    { 1, 15, "R-09L" },
                    { 2, 12, "R-27R" },
                    { 3, 20, "R-18" }
                });

            migrationBuilder.InsertData(
                table: "Senders",
                columns: new[] { "Sender_Id", "Discriminator", "Email_Address", "Full_Name" },
                values: new object[,]
                {
                    { -2, "Administrator", "admin@airport.com", "System Admin" },
                    { -1, "Bot", "customer-support@cloudspritzers.com", "Carlos" },
                    { 101, "User", "alice@travel.com", "Alice Traveler" },
                    { 102, "User", "bob@voyage.com", "Bob Voyager" },
                    { 103, "User", "mia@example.com", "Mia Passenger" }
                });

            migrationBuilder.InsertData(
                table: "TicketCategories",
                columns: new[] { "Category_Id", "Category_Name", "Default_Urgency_Level" },
                values: new object[,]
                {
                    { 1, "Technical Support", 1 },
                    { 2, "Billing", 2 },
                    { 3, "General Feedback", 0 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Customer_Id", "Email", "MembershipId", "Password_Hash", "Phone", "Username" },
                values: new object[,]
                {
                    { 101, "customer1@gmail.com", 1, "HASH123", null, "traveler_one" },
                    { 102, "customer2@yahoo.com", 2, "HASH456", null, "sky_high" },
                    { 103, "customer3@outlook.com", 3, "HASH789", null, "globe_trotter" },
                    { 104, "customer4@gmail.com", 4, "HASH000", null, "business_flyer" },
                    { 105, "customer5@yahoo.com", 1, "HASH111", null, "vacation_mode" }
                });

            migrationBuilder.InsertData(
                table: "Gates",
                columns: new[] { "Gate_Id", "AirportId", "Gate_Name" },
                values: new object[,]
                {
                    { 1, 1, "A1" },
                    { 2, 1, "A2" },
                    { 3, 2, "B1" },
                    { 4, 2, "B2" },
                    { 5, 3, "C1" },
                    { 6, 3, "C2" }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Route_Id", "AirportId", "Arrival_Time", "Capacity", "CompanyId", "Departure_Time", "Arrival_Date", "Recurrence_Interval", "Route_Type", "Departure_Date" },
                values: new object[,]
                {
                    { 1, 1, new TimeOnly(11, 0, 0), 180, 1, new TimeOnly(8, 0, 0), new DateOnly(2026, 12, 31), 1, "DEP", new DateOnly(2026, 1, 1) },
                    { 2, 2, new TimeOnly(18, 30, 0), 220, 2, new TimeOnly(14, 0, 0), new DateOnly(2026, 12, 31), 1, "ARR", new DateOnly(2026, 1, 1) },
                    { 3, 3, new TimeOnly(9, 45, 0), 150, 3, new TimeOnly(6, 30, 0), new DateOnly(2026, 12, 31), 2, "DEP", new DateOnly(2026, 1, 1) }
                });

            migrationBuilder.InsertData(
                table: "Shops",
                columns: new[] { "Shop_Id", "ManagerId", "Name", "Type" },
                values: new object[,]
                {
                    { 1, 1, "Duty Free Global", "Retail" },
                    { 2, 2, "Starbucks", "Coffee" },
                    { 3, 3, "Book World", "Books" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                column: "Sender_Id",
                values: new object[]
                {
                    101,
                    102,
                    103
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Chat_Id", "Chat_Status", "UserId" },
                values: new object[,]
                {
                    { 1, 0, 101 },
                    { 2, 1, 102 }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Flight_Id", "Departure_Date", "Flight_Number", "GateId", "RouteId", "RunwayId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 20, 8, 0, 0, 0, DateTimeKind.Unspecified), "BA101", 1, 1, 1 },
                    { 2, new DateTime(2026, 5, 21, 14, 0, 0, 0, DateTimeKind.Unspecified), "DL455", 3, 2, 2 },
                    { 3, new DateTime(2026, 5, 22, 6, 30, 0, 0, DateTimeKind.Unspecified), "W6 3301", 5, 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Review_Id", "Cleanliness_Rating", "Duty_Free_Rating", "Flight_Experience_Rating", "Message", "Staff_Friendliness_Rating", "UserId" },
                values: new object[,]
                {
                    { 1, 5, 5, 4, "Great airport, very clean!", 5, 101 },
                    { 2, 4, 3, 2, "Security took too long.", 3, 102 }
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "ShopItem_Id", "Description", "Name", "Photo", "Price", "Quantity", "ShopId" },
                values: new object[,]
                {
                    { 1, "Luxury scent", "Perfume", "perfume.jpg", 85f, 50, 1 },
                    { 2, "Single Malt", "Whiskey", "whiskey.jpg", 45f, 30, 1 },
                    { 3, "Grande size", "Caffe Latte", "coffee.jpg", 5.5f, 200, 2 },
                    { 4, "Memory foam", "Travel Pillow", "pillow.jpg", 25f, 100, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotMessages_ChatId",
                table: "BotMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_BotMessages_SenderId",
                table: "BotMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ShopItemId",
                table: "CartItems",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ClientId",
                table: "Carts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_MembershipId",
                table: "Customers",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFlights_FlightId",
                table: "EmployeeFlights",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQOptions_BotMessageId",
                table: "FAQOptions",
                column: "BotMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQOptions_NextNodeId",
                table: "FAQOptions",
                column: "NextNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQOptions_ParentNodeId",
                table: "FAQOptions",
                column: "ParentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_GateId",
                table: "Flights",
                column: "GateId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RunwayId",
                table: "Flights",
                column: "RunwayId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTicket_AddOns_TicketsId",
                table: "FlightTicket_AddOns",
                column: "TicketsId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTickets_FlightId",
                table: "FlightTickets",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTickets_UserId",
                table: "FlightTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Gates_AirportId",
                table: "Gates",
                column: "AirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Addon_Discounts_AddOnId",
                table: "Membership_Addon_Discounts",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationCartId",
                table: "Reservations",
                column: "ReservationCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_AirportId",
                table: "Routes",
                column: "AirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_CompanyId",
                table: "Routes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_ShopId",
                table: "ShopItems",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_ManagerId",
                table: "Shops",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SubcategoryId",
                table: "Tickets",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSubcategories_ParentCategoryId",
                table: "TicketSubcategories",
                column: "ParentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "EmployeeFlights");

            migrationBuilder.DropTable(
                name: "FAQOptions");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "FlightTicket_AddOns");

            migrationBuilder.DropTable(
                name: "Membership_Addon_Discounts");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "ShopItems");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "BotMessages");

            migrationBuilder.DropTable(
                name: "FAQNodes");

            migrationBuilder.DropTable(
                name: "FlightTickets");

            migrationBuilder.DropTable(
                name: "AddOns");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "TicketSubcategories");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "TicketCategories");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "Gates");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Runways");

            migrationBuilder.DropTable(
                name: "Senders");

            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
