using System.Collections.Generic;
using YCompanyClaimsApi.Models;

namespace YCompanyClaimsApi.Services
{
public interface IClaimService
{
    Task<Claim> CreateClaimAsync(ClaimDto claimDto,string user);
    Task<Claim> GetClaimByIdAsync(int claimId);
    Task<Claim> UpdateClaimAsync(Claim claim);
    Task<List<Claim>> GetClaimsForUserAsync(string userId);
    Task<List<Claim>> GetAllClaimsAsync();
    Task<Claim> SelectWorkshopAsync(int claimId, string workshopId);
    Task<Claim> UpdateClaimStatusAsync(int claimId, string status);
    Task<byte[]> GenerateReportAsync(DateTime startDate, DateTime endDate);
    Task<Claim> AdjudicateClaimAsync(int claimId, string adjustorNotes, string newStatus);
    Task<Claim> AssignAdjustorAsync(int claimId, int adjustorId);
    Task<Claim> AddInvoiceToClaimAsync(int claimId, IFormFile invoiceFile);
    // Task<Claim> AddInvoiceToClaimAsync(int claimId, string invoiceDocumentUuid);
    Task<Claim> ProcessPaymentAsync(int claimId);
    Task<Claim> AssignSurveyorAsync(int claimId, int surveyorId);
    Task<Claim> UpdateClaimAsync(int claimId, ClaimUpdateModel updateModel);



}
}