namespace YCompanyClaimsApi.Models
{
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public int? WorkshopId { get; set; }  // New property

}}