namespace YCompanyClaimsApi.Models
{
public class Assessment
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int SurveyorId { get; set; }
    public string Report { get; set; }
    public DateTime SubmissionDate { get; set; }
    public Claim Claim { get; set; }
    public User Surveyor { get; set; }
}
}
