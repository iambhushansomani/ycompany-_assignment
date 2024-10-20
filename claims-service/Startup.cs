// using Microsoft.EntityFrameworkCore;
// using YCompanyClaimsApi.Data;
// using YCompanyClaimsApi.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;

// public class Startup
// {
//     public IConfiguration Configuration { get; }

//     public Startup(IConfiguration configuration)
//     {
//         Configuration = configuration;
//     }

//     public void ConfigureServices(IServiceCollection services)
//     {
//         services.AddDbContext<ApplicationDbContext>(options =>
//             options.UseInMemoryDatabase("YCompanyClaimsDb"));
//         services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//             .AddJwtBearer(options =>
//             {
//                 options.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuer = true,
//                     ValidateAudience = true,
//                     ValidateLifetime = true,
//                     ValidateIssuerSigningKey = true,
//                     ValidIssuer = Configuration["Jwt:Issuer"],
//                     ValidAudience = Configuration["Jwt:Audience"],
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
//                 };
//             });

//         services.AddAuthorization(options =>
//         {
//             options.AddPolicy("ClaimsManager", policy => policy.RequireRole("ClaimsManager"));
//         });

//         services.AddScoped<IClaimService, ClaimService>();
//         services.AddScoped<IDocumentService, DocumentService>();
//         services.AddScoped<IUserService, UserService>();

//         services.AddControllers();
//     }

//     public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//     {
//         if (env.IsDevelopment())
//         {
//             app.UseDeveloperExceptionPage();
//         }

//         app.UseHttpsRedirection();
//         app.UseRouting();
//         app.UseAuthentication();
//         app.UseAuthorization();

//         app.UseEndpoints(endpoints =>
//         {
//             endpoints.MapControllers();
//         });
//     }
// }