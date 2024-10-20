namespace YCompanyClaimsApi.Models
{
public class Claim
{
    public int ClaimId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string PolicyNumber { get; set; }
    public DateTime IncidentDate { get; set; }
    public int SelectedWorkshopId { get; set; }
    public decimal ClaimAmount { get; set; }
    public string IncidentLocation { get; set; }
    // public IFormFile Document { get; set; } 
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    public string DocumentUuid { get;set; } // Add this line
    public int? AssessmentId { get; set; }
    public int? AdjustorId { get; set; }
    public string? AdjustorNotes { get; set; }
    public Assessment Assessment { get; set; }
    public User Adjustor { get; set; }
    public bool IsPaymentComplete { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? InvoiceDocumentUuid { get; set; }
    public int? SurveyorId { get; set; }
}
}