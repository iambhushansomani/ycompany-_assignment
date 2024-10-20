using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
namespace YCompanyClaimsApi.Services
{


public class AssessmentService : IAssessmentService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public AssessmentService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Assessment> SubmitAssessmentAsync(int claimId, int surveyorId, string report,string status)
    {
        var assessment = new Assessment
        {
            ClaimId = claimId,
            SurveyorId = surveyorId,
            Report = report,
            SubmissionDate = DateTime.UtcNow
        };

        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();

        // Notify adjustor
        var claim = await _context.Claims.FindAsync(claimId);
        claim.AssessmentId = assessment.Id;
        claim.Status = status;
        await _context.SaveChangesAsync();
        if (claim.AdjustorId.HasValue)
        {
            await _notificationService.NotifyAdjustorAsync(claim.AdjustorId.Value, claimId);
        }

        return assessment;
    }

    public async Task<Assessment> GetAssessmentAsync(int assessmentId)
    {
        return await _context.Assessments.FindAsync(assessmentId);
    }
}
}