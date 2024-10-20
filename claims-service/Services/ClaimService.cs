using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
namespace YCompanyClaimsApi.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ClaimService> _logger;
        private readonly IDocumentService _documentService;

        public ClaimService(ApplicationDbContext context, IHttpClientFactory clientFactory, ILogger<ClaimService> logger, IDocumentService documentService)
        {
            _context = context;
            _clientFactory = clientFactory;
            _logger = logger;
            _documentService = documentService;
        }

        public async Task<Claim> GetClaimByIdAsync(int claimId)
        {
            _logger.LogInformation($"Attempting to retrieve claim with ID: {claimId} from database");
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim == null)
            {
                _logger.LogWarning($"No claim found in database with ID: {claimId}");
            }
            else
            {
                _logger.LogInformation($"Successfully retrieved claim with ID: {claimId} from database");
            }

            return claim;
        }

        public async Task<Claim> CreateClaimAsync(ClaimDto claimDto, string userId)
        {
            try
            {
                if (!int.TryParse(userId, out int userIdInt))
                {
                    throw new ArgumentException("Invalid user ID format", nameof(userId));
                }

                var claim = new Claim
                {
                    Description = claimDto.Description,
                    IncidentDate = claimDto.IncidentDate,
                    PolicyNumber = claimDto.PolicyNumber,
                    Status = "Pending",
                    UserId = userIdInt,
                    ClaimAmount = claimDto.ClaimAmount,
                    IncidentLocation = claimDto.IncidentLocation,
                    DocumentUuid = ""
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync(); // Save to generate ClaimId

                if (claimDto.Document != null)
                {
                    try
                    {
                        var documentUuid = await AttachDocumentToClaim(claim.ClaimId, claimDto.Document);
                        if (!string.IsNullOrEmpty(documentUuid))
                        {
                            claim.DocumentUuid = documentUuid;
                            await _context.SaveChangesAsync(); // Save again to update DocumentUuid
                            _logger.LogInformation("Document attached successfully. UUID: {UUID}", documentUuid);
                        }
                        else
                        {
                            _logger.LogWarning("Document attachment returned null or empty UUID for claim {ClaimId}", claim.ClaimId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error attaching document to claim {ClaimId}: {Message}", claim.ClaimId, ex.Message);
                        // We're not throwing here, so the claim will still be created even if document attachment fails
                    }
                }
                else
                {
                    _logger.LogInformation("No document provided for claim {ClaimId}", claim.ClaimId);
                }

                return claim;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating claim: {Message}", ex.Message);
                throw; // Re-throw the exception to be handled by the controller
            }
        }

        private async Task<string> AttachDocumentToClaim(int claimId, IFormFile document)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var content = new MultipartFormDataContent();

                using (var memoryStream = new MemoryStream())
                {
                    await document.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var fileContent = new ByteArrayContent(memoryStream.ToArray());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(document.ContentType);

                    content.Add(fileContent, "file", document.FileName);

                    _logger.LogInformation("Sending document to API for claim {ClaimId}", claimId);
                    var response = await client.PostAsync($"http://document-service:5002/api/Documents?claimId={claimId}", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("Received response: {Response}", responseContent);
                        var documentResponse = JsonSerializer.Deserialize<DocumentResponse>(responseContent);
                        return documentResponse?.Uuid.Uuid ?? string.Empty;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to attach document. Status code: {StatusCode}", response.StatusCode);
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AttachDocumentToClaim for claim {ClaimId}: {Message}", claimId, ex.Message);
                return string.Empty;
            }
        }

        public async Task<Claim> GetClaimAsync(int id)
        {
            return await _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
        }

        public async Task<Claim> UpdateClaimAsync(Claim claim)
        {
            _context.Claims.Update(claim);
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<List<Claim>> GetClaimsForUserAsync(string userId)
        {
            if (int.TryParse(userId, out int parsedUserId))
            {
                return await _context.Claims
                    .Where(c => c.UserId == parsedUserId)
                    .ToListAsync();
            }
            else
            {
                throw new ArgumentException("Invalid userId. Must be a valid integer.");
            }
        }

        public async Task<List<Claim>> GetAllClaimsAsync()
        {
            return await _context.Claims.ToListAsync();
        }

        public async Task<Claim> SelectWorkshopAsync(int claimId, string workshopId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
                throw new ArgumentException("Claim not found");

            claim.SelectedWorkshopId = int.Parse(workshopId);
            claim.Status = "Workshop Selected";
            await _context.SaveChangesAsync();
            return claim;
        }

            public async Task<Claim> AssignSurveyorAsync(int claimId, int surveyorId)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim == null)
            throw new ArgumentException("Claim not found");

        var surveyor = await _context.Users.FindAsync(surveyorId);
        if (surveyor == null || surveyor.Role != "Surveyor")
            throw new ArgumentException("Invalid surveyor");

        claim.SurveyorId = surveyorId;
        claim.Status = "Surveyor Assigned";
        await _context.SaveChangesAsync();
        return claim;
    }

        public async Task<Claim> UpdateClaimStatusAsync(int claimId, string status)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
                throw new ArgumentException("Claim not found");

            if (!IsValidStatus(status))
                throw new ArgumentException("Invalid status");

            claim.Status = status;
            await _context.SaveChangesAsync();
            return claim;
        }

        private bool IsValidStatus(string status)
        {
            return new[] {
                "Initiated",
                "Surveyor Assigned",
                "Surveyor Approved",
                "Surveyor Rejected",
                "Adjustor Assigned",
                "Adjustor Approved",
                "Adjustor Rejected",
                "Work Started",
                "Work Completed - Payment Pending",
                "Payment Completed",
                "Completed & Delivered"
            }.Contains(status);
        }

        public async Task<byte[]> GenerateReportAsync(DateTime startDate, DateTime endDate)
        {
            var claims = await _context.Claims
                .Where(c => c.IncidentDate >= startDate && c.IncidentDate <= endDate)
                .ToListAsync();

            // Generate a report (e.g., CSV) based on the claims
            // This is a simplified example; you'd want to use a proper reporting library in a real application
            var report = new System.Text.StringBuilder();
            report.AppendLine("Id,UserId,Status,IncidentDate,SelectedWorkshopId");
            foreach (var claim in claims)
            {
                report.AppendLine($"{claim.ClaimId},{claim.UserId},{claim.Status},{claim.IncidentDate},{claim.SelectedWorkshopId}");
            }

            return System.Text.Encoding.UTF8.GetBytes(report.ToString());
        }

        public class DocumentResponse
        {
            public DocumentInfo Uuid { get; set; }
        }

        public class DocumentInfo
        {
            public int Id { get; set; }
            public string Uuid { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public string Content { get; set; }
            public int ClaimId { get; set; }
            public Claim claim { get; set; }
        }

        public IEnumerable<Claim> GetClaimsByWorkshop(string workshopId)
        {
            if (int.TryParse(workshopId, out int parsedWorkshopId))
            {
                return _context.Claims.Where(c => c.SelectedWorkshopId == parsedWorkshopId).ToList();
            }
            else
            {
                // Handle the case where workshopId is not a valid integer
                throw new ArgumentException("Invalid workshopId. Must be a valid integer.");
            }
        }

        public Claim GetClaimById(int id)
        {
            // Assuming _context is your database context
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            
            if (claim != null)
            {
                return claim;
            }
            else
            {
                // If no claim is found, you have a few options:
                // 1. Return null (not recommended if the return type is non-nullable)
                // return null;

                // 2. Throw an exception
                throw new KeyNotFoundException($"Claim with id {id} not found");

                // 3. Return a default Claim object
                // return new Claim();
            }
        }

         public async Task<Claim> AddInvoiceToClaimAsync(int claimId, IFormFile invoiceFile)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim == null)
            throw new ArgumentException("Claim not found");

        var invoice = await _documentService.UploadInvoiceAsync(invoiceFile, claimId);
        claim.InvoiceDocumentUuid = invoice.Uuid;
        claim.Status = "Work Completed - Payment Pending";
        await _context.SaveChangesAsync();
        return claim;
    }

    public async Task<Claim> ProcessPaymentAsync(int claimId)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim == null)
            throw new ArgumentException("Claim not found");

        claim.IsPaymentComplete = true;
        claim.PaymentDate = DateTime.UtcNow;
        claim.Status = "Payment Completed";
        await _context.SaveChangesAsync();
        return claim;
    }

        public async Task<Claim> AssignAdjustorAsync(int claimId, int adjustorId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
                throw new ArgumentException("Claim not found");

            claim.AdjustorId = adjustorId;
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<Claim> AdjudicateClaimAsync(int claimId, string adjustorNotes, string newStatus)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
                throw new ArgumentException("Claim not found");

            claim.AdjustorNotes = adjustorNotes;
            claim.Status = newStatus;
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<Claim> UpdateClaimAsync(int claimId, ClaimUpdateModel updateModel)
        {
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim == null)
            {
                return null;
            }

            // Update only the non-null properties
            if (updateModel.PolicyNumber != null)
                claim.PolicyNumber = updateModel.PolicyNumber;
            if (updateModel.IncidentDate.HasValue)
                claim.IncidentDate = updateModel.IncidentDate.Value;
            if (updateModel.ClaimAmount.HasValue)
                claim.ClaimAmount = updateModel.ClaimAmount.Value;
            if (updateModel.IncidentLocation != null)
                claim.IncidentLocation = updateModel.IncidentLocation;
            if (updateModel.Status != null)
                claim.Status = updateModel.Status;
            if (updateModel.Description != null)
                claim.Description = updateModel.Description;
            if (updateModel.SurveyorId.HasValue)
                claim.SurveyorId = updateModel.SurveyorId.Value;
            if (updateModel.AdjusterId.HasValue)
                claim.AdjustorId = updateModel.AdjusterId.Value;
            if (updateModel.WorkshopId.HasValue)
                claim.SelectedWorkshopId = updateModel.WorkshopId.Value;

            // Add any other fields that can be updated

            await _context.SaveChangesAsync();

            return claim;
        }
    }
}