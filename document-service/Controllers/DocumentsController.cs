using YCompanyClaimsApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace YCompanyClaimsApi.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument(IFormFile file, [FromQuery] int claimId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

         try
        {
            var documentUuid = await _documentService.UploadDocumentAsync(file, claimId);
            return Ok(new { Uuid = documentUuid });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while uploading the document.");
        }
    }

    [HttpGet("{uuid}")]
    public async Task<IActionResult> GetDocument(String uuid)
    {
        var document = await _documentService.GetDocumentAsync(uuid);
        if (document == null)
            return NotFound();

        return File(document.Content, document.ContentType, document.FileName);
    }
}
}