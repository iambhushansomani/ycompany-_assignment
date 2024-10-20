using YCompanyClaimsApi.Models;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

   
    public DbSet<Workshop> Workshops { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);





  modelBuilder.Entity<Workshop>(entity =>
            {
                entity.ToTable("Workshops");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name);
                entity.Property(e => e.Address);
                entity.Property(e => e.Pincode);
            });


        

        // modelBuilder.Entity<Document>()
        //     .Property(d => d.Uuid)
        //     .HasMaxLength(36);  // UUID is 36 characters

        // Add more seed data as needed
    }
}