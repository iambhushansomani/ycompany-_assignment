using YCompanyClaimsApi.Models;
using System;
using System.Linq;
using System.IO;

namespace YCompanyClaimsApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

                    // Check if the database already has data
        if (context.Workshops.Any())
        {
            return;   // Database has been seeded
        }



           
            // Add workshops (unchanged)
            var workshops = new Workshop[]
            {
                new Workshop { Name = "Quick Fix Auto", Address = "123 Main St", Pincode = "12345" },
                new Workshop { Name = "Speedy Repairs", Address = "456 Elm St", Pincode = "67890" }
            };
            context.Workshops.AddRange(workshops);
            context.SaveChanges();

                    }
    }
}