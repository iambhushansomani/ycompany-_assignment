using YCompanyClaimsApi.Models;
using YCompanyClaimsApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace YCompanyClaimsApi.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class AssessmentsController : ControllerBase
{
    private readonly IAssessmentService _assessmentService;

    public AssessmentsController(IAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    [HttpPost]
    // [Authorize(Roles = "Surveyor")]
    public async Task<IActionResult> SubmitAssessment([FromBody] AssessmentSubmissionDto dto)
    {
        // var surveyorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var assessment = await _assessmentService.SubmitAssessmentAsync(dto.ClaimId, dto.surveyorId, dto.Report, dto.status);
        return CreatedAtAction(nameof(GetAssessment), new { id = assessment.Id }, assessment);
    }

    [HttpGet("{id}")]
    // [Authorize(Roles = "Surveyor,Adjustor,ClaimsManager")]
    public async Task<IActionResult> GetAssessment(int id)
    {
        var assessment = await _assessmentService.GetAssessmentAsync(id);
        if (assessment == null)
            return NotFound();
        return Ok(assessment);
    }
}
}
