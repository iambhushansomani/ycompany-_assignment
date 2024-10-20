using YCompanyClaimsApi.Models;
using Microsoft.AspNetCore.Mvc;
using YCompanyClaimsApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;  // Add this line

namespace YCompanyClaimsApi.Controllers

{


[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;
    private readonly ILogger<ClaimsController> _logger;
    private readonly IDocumentService _documentService;
    public ClaimsController(IClaimService claimService, IDocumentService documentService, ILogger<ClaimsController> logger)
    {
        _claimService = claimService;
        _documentService = documentService;
        _logger = logger;
    }

    [HttpGet("all")]
    // [Authorize(Policy = "ClaimsManager")]
    public async Task<IActionResult> GetAllClaims()
    {
        try
        {
            var claims = await _claimService.GetAllClaimsAsync();
            return Ok(claims);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while retrieving claims.");
        }
    }

        [HttpGet("{id}")]
    public async Task<ActionResult<YCompanyClaimsApi.Models.Claim>> GetClaim(int id)
    {
        _logger.LogInformation($"Attempting to retrieve claim with ID: {id}");
        var claim = await _claimService.GetClaimByIdAsync(id);

        if (claim == null)
        {
            _logger.LogWarning($"Claim with ID {id} not found");
            return NotFound($"Claim with ID {id} not found");
        }

        _logger.LogInformation($"Successfully retrieved claim with ID: {id}");
        return Ok(claim);
    }

[HttpPost]
// [Authorize]
public async Task<IActionResult> CreateClaim([FromForm] YCompanyClaimsApi.Models.ClaimDto claimDto,String userId)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    // if (string.IsNullOrEmpty(userId))
    // {
    //     return Unauthorized();
    // }

    try
    {
        var claim = await _claimService.CreateClaimAsync(claimDto, userId);
        return CreatedAtAction(nameof(GetClaim), new { id = claim.ClaimId }, claim);
    }
    catch (Exception ex)
    {
        // Log the exception
        _logger.LogError(ex, "Error occurred while creating claim: {Message}", ex.Message);

        return StatusCode(500, "An error occurred while creating the claim.");
    }
}

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetClaimsForUser(string userId)
    {
        var claims = await _claimService.GetClaimsForUserAsync(userId);
        return Ok(claims);
    }

    [HttpPut("{id}/workshop")]
    public async Task<IActionResult> SelectWorkshop(int id, [FromBody] string workshopId)
    {
        try
        {
            var updatedClaim = await _claimService.SelectWorkshopAsync(id, workshopId);
            return Ok(updatedClaim);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}/status")]
    // [Authorize] // Add appropriate authorization
    public async Task<IActionResult> UpdateClaimStatus(int id, [FromBody] string status)
    {
        try
        {
            var updatedClaim = await _claimService.UpdateClaimStatusAsync(id, status);
            return Ok(updatedClaim);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("report")]
    // [Authorize(Policy = "ClaimsManager")]
    public async Task<IActionResult> GenerateReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var report = await _claimService.GenerateReportAsync(startDate, endDate);
        return File(report, "text/csv", "claims_report.csv");
    }

    [HttpPut("{id}/assign-adjustor")]
    public async Task<IActionResult> AssignAdjustor(int id, [FromBody] int adjustorId)
    {
        try
        {
            var updatedClaim = await _claimService.AssignAdjustorAsync(id, adjustorId);
            return Ok(updatedClaim);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}/adjudicate")]
    public async Task<IActionResult> AdjudicateClaim(int id, [FromBody] AdjudicationDto dto)
    {
        try
        {
            var updatedClaim = await _claimService.AdjudicateClaimAsync(id, dto.AdjustorNotes, dto.NewStatus);
            return Ok(updatedClaim);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpPost("{id}/process-payment")]
    // [Authorize(Roles = "Customer")] // Adjust as needed
    public async Task<IActionResult> ProcessPayment(int id)
    {
        try
        {
            var updatedClaim = await _claimService.ProcessPaymentAsync(id);
            return Ok(updatedClaim);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}/assign-surveyor")]
// [Authorize(Roles = "ClaimsManager")] // Adjust as needed
public async Task<IActionResult> AssignSurveyor(int id, [FromBody] int surveyorId)
{
    try
    {
        var updatedClaim = await _claimService.AssignSurveyorAsync(id, surveyorId);
        return Ok(updatedClaim);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
}

    [HttpPost("{id}/upload-invoice")]
    // [Authorize(Roles = "Workshop")] // Adjust as needed
    public async Task<IActionResult> UploadInvoice(int id, IFormFile file)
    {
        try
        {
            var updatedClaim = await _claimService.AddInvoiceToClaimAsync(id, file);
            return Ok(updatedClaim);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}/invoice")]
    // [Authorize] // Adjust as needed
    public async Task<IActionResult> GetInvoice(int id)
    {
        var invoice = await _documentService.GetInvoiceAsync(id);
        if (invoice == null)
            return NotFound("Invoice not found");

        return File(invoice.Content, invoice.ContentType, invoice.FileName);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "ClaimsManager")]
    public async Task<IActionResult> UpdateClaim(int id, [FromBody] ClaimUpdateModel updateModel)
    {
        try
        {
            var updatedClaim = await _claimService.UpdateClaimAsync(id, updateModel);
            if (updatedClaim == null)
            {
                return NotFound();
            }
            return Ok(updatedClaim);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while updating the claim.");
        }
    }
}
}
