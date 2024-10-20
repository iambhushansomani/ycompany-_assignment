using YCompanyClaimsApi.Models;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

   
    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    

        // Seed some initial data
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "customer1", PasswordHash = "password", Role = "Customer" },
            new User { Id = 2, Username = "customer2", PasswordHash = "password", Role = "Customer" },
            new User { Id = 3, Username = "manager1", PasswordHash = "password", Role = "ClaimsManager" }
        );

   


        

        // modelBuilder.Entity<Document>()
        //     .Property(d => d.Uuid)
        //     .HasMaxLength(36);  // UUID is 36 characters

        // Add more seed data as needed
    }
}