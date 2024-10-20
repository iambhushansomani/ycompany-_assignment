namespace YCompanyClaimsApi.Models
{
public class Document
{
    public int Id { get; set; }
    public string Uuid { get; set; }

    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
    public int ClaimId { get; set; }
    public Claim Claim { get; set; }
    public string DocumentType { get; set; }
}}