using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Models;
using Microsoft.EntityFrameworkCore;
namespace YCompanyClaimsApi.Services
{
    public class DocumentService : IDocumentService
    {
    private readonly ApplicationDbContext _context;

    public DocumentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document> UploadDocumentAsync(IFormFile file, int claimId)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var document = new Document
        {
            Uuid = Guid.NewGuid().ToString(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Content = memoryStream.ToArray(),
            ClaimId = claimId,
            DocumentType = "General" // Add this line to differentiate from invoices

        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document> GetDocumentAsync(string uuid)
    {
            return await _context.Documents.FirstOrDefaultAsync(d => d.Uuid == uuid);
    }

        public async Task<Document> UploadInvoiceAsync(IFormFile file, int claimId)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var invoice = new Document
            {
                Uuid = Guid.NewGuid().ToString(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Content = memoryStream.ToArray(),
                ClaimId = claimId,
                DocumentType = "Invoice" // Add this line to identify as an invoice
            };

            _context.Documents.Add(invoice);

            // Update the claim with the invoice document UUID
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim != null)
            {
                claim.InvoiceDocumentUuid = invoice.Uuid;
                claim.Status = "Work Completed - Payment Pending";
            }

            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Document> GetInvoiceAsync(int claimId)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.ClaimId == claimId && d.DocumentType == "Invoice");
        }
    }

// public Task<Document> GetDocumentById(string uuid)
// {
//     var document = _context.Documents.FirstOrDefault(d => d.Uuid == uuid);
//     if (document == null)
//     {
//         throw new KeyNotFoundException($"Document with uuid {uuid} not found");
//     }
//     return document;
// }
    }