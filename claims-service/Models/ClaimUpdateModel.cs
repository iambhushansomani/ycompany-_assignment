using System;

namespace YCompanyClaimsApi.Models
{
    public class ClaimUpdateModel
    {
        public string PolicyNumber { get; set; }
        public DateTime? IncidentDate { get; set; }
        public decimal? ClaimAmount { get; set; }
        public string IncidentLocation { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? SurveyorId { get; set; }
        public int? AdjusterId { get; set; }
        public int? WorkshopId { get; set; }
        // Add any other fields that can be updated
    }
}