using YCompanyClaimsApi.Models;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Claim> Claims { get; set; }
    public DbSet<Document> Documents { get; set; }
    // public DbSet<Workshop> Workshops { get; set; }
    // public DbSet<User> Users { get; set; }
    public DbSet<Assessment> Assessments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Claim>()
                .HasOne(c => c.Assessment)
                .WithOne(a => a.Claim)
                .HasForeignKey<Assessment>(a => a.ClaimId);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId);
        }
    }



        // Seed some initial data



        

        // modelBuilder.Entity<Document>()
        //     .Property(d => d.Uuid)
        //     .HasMaxLength(36);  // UUID is 36 characters

        // Add more seed data as needed
