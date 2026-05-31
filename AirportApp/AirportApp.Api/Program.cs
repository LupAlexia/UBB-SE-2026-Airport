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
