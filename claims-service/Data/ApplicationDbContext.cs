using YCompanyClaimsApi.Models;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        
        // Add this line to configure DateTime conversion
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Claim> Claims { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }

    public DbSet<Assessment> Assessments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

           modelBuilder.Entity<Claim>()
                .HasOne(c => c.Assessment)
                .WithOne(a => a.Claim)
                .HasForeignKey<Assessment>(a => a.ClaimId);

            modelBuilder.Entity<Assessment>()
                .HasOne(a => a.Claim)
                .WithOne(c => c.Assessment)
                .HasForeignKey<Claim>(c => c.AssessmentId);

   


        

        // modelBuilder.Entity<Document>()
        //     .Property(d => d.Uuid)
        //     .HasMaxLength(36);  // UUID is 36 characters

        // Add more seed data as needed
    }
}