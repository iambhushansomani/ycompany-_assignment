using Microsoft.AspNetCore.Mvc;
using YCompanyClaimsApi.Services;
using YCompanyClaimsApi.Models;
using System.Collections.Generic;

namespace YCompanyClaimsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkshopController : ControllerBase
    {
        private readonly IWorkshopService _workshopService;

        public WorkshopController(IWorkshopService workshopService)
        {
            _workshopService = workshopService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Workshop>> GetWorkshops()
        {
            var workshops = _workshopService.GetAllWorkshops();
            return Ok(workshops);
        }

         [HttpGet("by-pincode/{pincode}")]
        // [Authorize] // Adjust the authorization as needed
        public async Task<ActionResult<IEnumerable<Workshop>>> GetWorkshopsByPincode(string pincode)
        {
            var workshops = await _workshopService.GetWorkshopsByPincodeAsync(pincode);
            if (workshops == null || !workshops.Any())
            {
                return NotFound($"No workshops found for pincode: {pincode}");
            }
            return Ok(workshops);
        }
    }
}