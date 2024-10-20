using Microsoft.EntityFrameworkCore;
using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Services;
using YCompanyClaimsApi.Controllers;
using Microsoft.AspNetCore.Cors;
using System.Text.Json.Serialization;
using Npgsql;


Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:5000");
Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER", "0");
// var builder = WebApplication.CreateBuilder(args);
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    WebRootPath = "wwwroot"
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5003);
});

// Disable HTTPS redirection
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 0;
});
// builder.WebHost.UseUrls("http://*:5000");

// Add services to the container.
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseInMemoryDatabase("YCompanyClaimsDb"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers().AddJsonOptions(options =>
    {
          options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Register your services
// builder.Services.AddScoped<IClaimService, ClaimService>();
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IWorkshopService, WorkshopService>();
// builder.Services.AddScoped<IAssessmentService, AssessmentService>();
// builder.Services.AddScoped<INotificationService, NotificationService>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();
app.UsePathBase("/workshop");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/workshop/swagger/v1/swagger.json", "YCompany API V1"));
// app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
       endpoints.MapControllers();
});

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // logger.LogInformation($"Connection string: {context.Database.GetConnectionString()}");

        var connectionString = context.Database.GetConnectionString();
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = connectionStringBuilder.Database;
        connectionStringBuilder.Database = "YCompanyClaimsDb";  

         using (var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
                var result = command.ExecuteScalar();
                if (result == null)
                {
                    // logger.LogInformation($"Database {databaseName} does not exist. Creating...");
                    command.CommandText = $"CREATE DATABASE {databaseName}";
                    command.ExecuteNonQuery();
                    // logger.LogInformation($"Database {databaseName} created successfully.");
                }
            }
        }
        
        // Now create the Workshops table if it doesn't exist
        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ""Workshops"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Name"" TEXT NOT NULL,
                ""Address"" TEXT NOT NULL,
                ""Pincode"" TEXT NOT NULL
            )
        ");
        // logger.LogInformation("Workshops table created or already exists.");      
        // This will apply any pending migrations
        context.Database.Migrate();

        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        // var logger = services.GetRequiredService<ILogger<Program>>();
        // logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
