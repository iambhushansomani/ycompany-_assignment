using YCompanyClaimsApi.Models;
using YCompanyClaimsApi.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;  // Add this line


namespace YCompanyClaimsApi.Services
{
    public class WorkshopService : IWorkshopService
    {
        private readonly ApplicationDbContext _context;

        public WorkshopService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Workshop> GetAllWorkshops()
        {
            return _context.Workshops.ToList();
        }

          public async Task<IEnumerable<Workshop>> GetWorkshopsByPincodeAsync(string pincode)
    {
        return await _context.Workshops
            .Where(w => w.Pincode == pincode)
            .ToListAsync();
    }
    }
}