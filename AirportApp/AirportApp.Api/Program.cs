using AirportApp.ClassLibrary;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
applicationBuilder.Services.AddSwaggerGen();

WebApplication application = applicationBuilder.Build();

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

application.Lifetime.ApplicationStarted.Register(() =>
{
    string swaggerUrl = "http://localhost:5043";
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(swaggerUrl) { UseShellExecute = true });
});

application.Run();
