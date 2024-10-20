namespace YCompanyClaimsApi.Models
{
public class AssessmentSubmissionDto
{
    public int ClaimId { get; set; }
    public string Report { get; set; }
    public int surveyorId { get; set; }
    public string status { get; set; }
}
}