using AirportApp.ClassLibrary;
using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;

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
