using AirportApp.ClassLibrary;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder applicationBuilder = WebApplication.CreateBuilder(args);

bool isTestingEnvironment = applicationBuilder.Environment.IsEnvironment("Testing") ||
                            AppDomain.CurrentDomain.GetAssemblies().Any(a =>
                                a.FullName != null &&
                                (a.FullName.Contains("AirportApp.Tests") ||
                                 a.FullName.Contains("Microsoft.AspNetCore.Mvc.Testing")));

if (isTestingEnvironment)
{
    applicationBuilder.Services.AddDbContext<AppDbContext>(dbContextOptions =>
        dbContextOptions.UseInMemoryDatabase("InMemoryAirportApiTestDb"));
}
else
{
    applicationBuilder.Services.AddDbContext<AppDbContext>(dbContextOptions =>
        dbContextOptions.UseSqlServer(
            applicationBuilder.Configuration.GetConnectionString("DefaultConnection")));
}

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

applicationBuilder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen();

WebApplication application = applicationBuilder.Build();

if (!isTestingEnvironment)
{
    using (IServiceScope scope = application.Services.CreateScope())
    {
        AppDbContext databaseContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await databaseContext.Database.MigrateAsync();

    if (!await databaseContext.TicketSubcategories.AnyAsync())
    {
        ComplaintTicketCategory technicalSupport = await databaseContext.TicketCategories.FirstAsync(category => category.Id == 1);
        ComplaintTicketCategory billing = await databaseContext.TicketCategories.FirstAsync(category => category.Id == 2);
        ComplaintTicketCategory generalFeedback = await databaseContext.TicketCategories.FirstAsync(category => category.Id == 3);

        databaseContext.TicketSubcategories.AddRange(
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Login or account access",
                SubcategoryExternalReferenceId = 1,
                ParentCategory = technicalSupport
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Booking or reservation issue",
                SubcategoryExternalReferenceId = 2,
                ParentCategory = technicalSupport
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Website or app problem",
                SubcategoryExternalReferenceId = 3,
                ParentCategory = technicalSupport
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Refund request",
                SubcategoryExternalReferenceId = 4,
                ParentCategory = billing
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Incorrect charge",
                SubcategoryExternalReferenceId = 5,
                ParentCategory = billing
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Invoice or payment receipt",
                SubcategoryExternalReferenceId = 6,
                ParentCategory = billing
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Complaint",
                SubcategoryExternalReferenceId = 7,
                ParentCategory = generalFeedback
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Suggestion",
                SubcategoryExternalReferenceId = 8,
                ParentCategory = generalFeedback
            },
            new ComplaintTicketSubcategory
            {
                SubcategoryName = "Praise",
                SubcategoryExternalReferenceId = 9,
                ParentCategory = generalFeedback
            }
        );

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.Faqs.AnyAsync())
    {
        databaseContext.Faqs.AddRange(
            new FAQEntry
            {
                Question = "How do I check in online?",
                Answer = "Open your booking, select Check-in, and follow the on-screen steps to confirm your passenger details and boarding pass.",
                Category = FAQCategoryEnum.CheckIn,
                ViewCount = 0,
                HelpfulVotesCount = 0,
                NotHelpfulVotesCount = 0
            },
            new FAQEntry
            {
                Question = "Where can I park at the airport?",
                Answer = "Use the short-stay or long-stay car parks marked on the airport map. The parking page shows the current options and rates.",
                Category = FAQCategoryEnum.Parking,
                ViewCount = 0,
                HelpfulVotesCount = 0,
                NotHelpfulVotesCount = 0
            },
            new FAQEntry
            {
                Question = "What items are allowed in cabin baggage?",
                Answer = "Liquids must follow the 100 ml rule and sharp or restricted items are not allowed in the cabin. Check the baggage page for the full list.",
                Category = FAQCategoryEnum.Baggage,
                ViewCount = 0,
                HelpfulVotesCount = 0,
                NotHelpfulVotesCount = 0
            },
            new FAQEntry
            {
                Question = "How do I change or cancel a ticket?",
                Answer = "Go to My Bookings, open the ticket, and choose the change or cancel option if it is available for your fare type.",
                Category = FAQCategoryEnum.Tickets,
                ViewCount = 0,
                HelpfulVotesCount = 0,
                NotHelpfulVotesCount = 0
            },
            new FAQEntry
            {
                Question = "Where can I find airport facilities?",
                Answer = "Facilities such as restrooms, lounges, charging points, and accessibility services are listed in the airport facilities section.",
                Category = FAQCategoryEnum.Facilities,
                ViewCount = 0,
                HelpfulVotesCount = 0,
                NotHelpfulVotesCount = 0
            }
        );

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.Flights.AnyAsync())
    {
        databaseContext.Flights.AddRange(
            new Flight
            {
                Date = new DateTime(2026, 5, 20, 8, 0, 0),
                FlightNumber = "BA101",
                Route = new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 1 },
                Runway = new Runway { Id = 1 },
                Gate = new Gate { Id = 1 }
            },
            new Flight
            {
                Date = new DateTime(2026, 5, 21, 14, 0, 0),
                FlightNumber = "DL455",
                Route = new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 2 },
                Runway = new Runway { Id = 2 },
                Gate = new Gate { Id = 3 }
            },
            new Flight
            {
                Date = new DateTime(2026, 5, 22, 6, 30, 0),
                FlightNumber = "W6 3301",
                Route = new AirportApp.ClassLibrary.Entity.Domain.Route { Id = 3 },
                Runway = new Runway { Id = 3 },
                Gate = new Gate { Id = 5 }
            }
        );

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.Flights.AnyAsync(flight => flight.FlightNumber == "BA103" && flight.Date == new DateTime(2026, 6, 3, 8, 0, 0)))
    {
        AirportApp.ClassLibrary.Entity.Domain.Route londonRoute = new() { Id = 1 };
        Runway runway = new() { Id = 1 };
        Gate gate = new() { Id = 1 };

        databaseContext.Entry(londonRoute).State = EntityState.Unchanged;
        databaseContext.Entry(runway).State = EntityState.Unchanged;
        databaseContext.Entry(gate).State = EntityState.Unchanged;

        databaseContext.Flights.Add(new Flight
        {
            Date = new DateTime(2026, 6, 3, 8, 0, 0),
            FlightNumber = "BA103",
            Route = londonRoute,
            Runway = runway,
            Gate = gate
        });

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.Flights.AnyAsync(flight => flight.FlightNumber == "BA201" && flight.Date == new DateTime(2026, 7, 4, 8, 0, 0)))
    {
        AirportApp.ClassLibrary.Entity.Domain.Route londonRoute = new() { Id = 1 };
        Runway runway = new() { Id = 1 };
        Gate gate = new() { Id = 1 };

        databaseContext.Entry(londonRoute).State = EntityState.Unchanged;
        databaseContext.Entry(runway).State = EntityState.Unchanged;
        databaseContext.Entry(gate).State = EntityState.Unchanged;

        databaseContext.Flights.Add(new Flight
        {
            Date = new DateTime(2026, 7, 4, 8, 0, 0),
            FlightNumber = "BA201",
            Route = londonRoute,
            Runway = runway,
            Gate = gate
        });

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.Flights.AnyAsync(flight => flight.FlightNumber == "DL456" && flight.Date == new DateTime(2026, 7, 11, 14, 0, 0)))
    {
        AirportApp.ClassLibrary.Entity.Domain.Route newYorkRoute = new() { Id = 2 };
        Runway runway = new() { Id = 2 };
        Gate gate = new() { Id = 3 };

        databaseContext.Entry(newYorkRoute).State = EntityState.Unchanged;
        databaseContext.Entry(runway).State = EntityState.Unchanged;
        databaseContext.Entry(gate).State = EntityState.Unchanged;

        databaseContext.Flights.Add(new Flight
        {
            Date = new DateTime(2026, 7, 11, 14, 0, 0),
            FlightNumber = "DL456",
            Route = newYorkRoute,
            Runway = runway,
            Gate = gate
        });

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.Flights.AnyAsync(flight => flight.FlightNumber == "W6 3302" && flight.Date == new DateTime(2026, 7, 18, 6, 30, 0)))
    {
        AirportApp.ClassLibrary.Entity.Domain.Route clujRoute = new() { Id = 3 };
        Runway runway = new() { Id = 3 };
        Gate gate = new() { Id = 5 };

        databaseContext.Entry(clujRoute).State = EntityState.Unchanged;
        databaseContext.Entry(runway).State = EntityState.Unchanged;
        databaseContext.Entry(gate).State = EntityState.Unchanged;

        databaseContext.Flights.Add(new Flight
        {
            Date = new DateTime(2026, 7, 18, 6, 30, 0),
            FlightNumber = "W6 3302",
            Route = clujRoute,
            Runway = runway,
            Gate = gate
        });

        await databaseContext.SaveChangesAsync();
    }

    if (!await databaseContext.FaqNodes.AnyAsync(node => node.NodeId == 5))
    {
        FAQNode lostBaggageNode = new()
        {
            QuestionText = "Please go to the baggage services desk or file a lost baggage report at the arrivals hall.",
            IsFinalAnswer = true
        };

        FAQNode damagedBaggageNode = new()
        {
            QuestionText = "Take photos and report the damage at the baggage service desk before leaving the airport.",
            IsFinalAnswer = true
        };

        FAQNode delayedBaggageNode = new()
        {
            QuestionText = "Use your reference number to track the bag and contact baggage services if it does not arrive.",
            IsFinalAnswer = true
        };

        databaseContext.FaqNodes.AddRange(lostBaggageNode, damagedBaggageNode, delayedBaggageNode);
        await databaseContext.SaveChangesAsync();

        FAQNode baggageQuestionNode = await databaseContext.FaqNodes.FirstAsync(node => node.NodeId == 2);

        baggageQuestionNode.Options.Add(new FAQOption
        {
            Label = "Lost baggage",
            NextOption = lostBaggageNode
        });

        baggageQuestionNode.Options.Add(new FAQOption
        {
            Label = "Damaged baggage",
            NextOption = damagedBaggageNode
        });

        baggageQuestionNode.Options.Add(new FAQOption
        {
            Label = "Delayed baggage",
            NextOption = delayedBaggageNode
        });

        await databaseContext.SaveChangesAsync();
    }
}
}

application.UseSwagger();
application.UseSwaggerUI(swaggerUiOptions =>
{
    swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Airport API v1");
    swaggerUiOptions.RoutePrefix = string.Empty;
});

application.UseCors("AirportCorsPolicy");
application.UseHttpsRedirection();
application.UseAuthorization();
application.MapControllers();

if (!isTestingEnvironment)
{
    application.Lifetime.ApplicationStarted.Register(() =>
    {
        string swaggerUrl = "http://localhost:5043";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(swaggerUrl) { UseShellExecute = true });
    });
}

application.Run();

public partial class Program { }
