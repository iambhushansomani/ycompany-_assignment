using YCompanyClaimsApi.Models;
using System.Collections.Generic;

namespace YCompanyClaimsApi.Services
{
    public interface IWorkshopService
    {
        IEnumerable<Workshop> GetAllWorkshops();
            Task<IEnumerable<Workshop>> GetWorkshopsByPincodeAsync(string pincode);

    }
}