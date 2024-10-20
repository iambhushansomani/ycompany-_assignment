using YCompanyClaimsApi.Models;
using System.Collections.Generic;

namespace YCompanyClaimsApi.Services
{
public interface IDocumentService
{
    Task<Document> UploadDocumentAsync(IFormFile file, int claimId);
    Task<Document> GetDocumentAsync(string uuid);
    Task<Document> UploadInvoiceAsync(IFormFile file, int claimId);
    Task<Document> GetInvoiceAsync(int claimId);
    // Task<Document> GetDocumentById(string uuid);
}
}
