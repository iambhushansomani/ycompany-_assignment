using Microsoft.EntityFrameworkCore;
using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Services;
using YCompanyClaimsApi.Controllers;
using Microsoft.AspNetCore.Cors;
using System.Text.Json.Serialization;
using Npgsql;


Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:5000");
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
    serverOptions.ListenAnyIP(5002);
});

// Disable HTTPS redirection
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 0;
});
// builder.WebHost.UseUrls("http://*:5000");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
});
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
builder.Services.AddScoped<IDocumentService, DocumentService>();

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

app.UsePathBase("/documents");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/documents/swagger/v1/swagger.json", "YCompany API V1"));
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
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Log the connection string (be careful with this in production!)
        
        // Check if the database exists, if not create it
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
                    command.CommandText = $"CREATE DATABASE {databaseName}";
                    command.ExecuteNonQuery();
                }
            }
        }
        
        // Now create the Documents table if it doesn't exist
        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS ""Documents"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Uuid"" TEXT NOT NULL,
                ""FileName"" TEXT NOT NULL,
                ""ContentType"" TEXT NOT NULL,
                ""Content"" BYTEA NOT NULL,
                ""ClaimId"" INTEGER NOT NULL,
                ""DocumentType"" TEXT NOT NULL
            )
        ");
        
        // Attempt to apply migrations
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
