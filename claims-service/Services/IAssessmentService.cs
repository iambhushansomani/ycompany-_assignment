using YCompanyClaimsApi.Models;
using System.Collections.Generic;

namespace YCompanyClaimsApi.Services
{
public interface IAssessmentService
{
    Task<Assessment> SubmitAssessmentAsync(int claimId, int surveyorId, string report, string status);
    Task<Assessment> GetAssessmentAsync(int assessmentId);
}
}