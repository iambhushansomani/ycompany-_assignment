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
            if (context.Users.Any())
            {
                return;   // Database has been seeded
        }


            // // Look for any existing claims
            // if (context.Claims.Any())
            // {
            //     return;   // DB has been seeded
            // }

            // Add users (unchanged)
            var users = new User[]
            {
                new User { Username = "customer1", PasswordHash = "password", Role = "Customer" },
                new User { Username = "customer2", PasswordHash = "password", Role = "Customer" },
                new User { Username = "manager1", PasswordHash = "password", Role = "ClaimsManager" },
                new User { Username = "surveyor1", PasswordHash = "password", Role = "Surveyor" },
                new User { Username = "adjustor1", PasswordHash = "password", Role = "Adjustor" },
                new User { Username = "workshop1", PasswordHash = "password", Role = "Workshop" ,WorkshopId = 1},
                new User { Username = "workshop2", PasswordHash = "password", Role = "Workshop" ,WorkshopId = 2}

            };
            context.Users.AddRange(users);
            context.SaveChanges();

        }
    }
}