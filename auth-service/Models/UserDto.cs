namespace YCompanyClaimsApi.Models
{
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    // public string Email { get; set; }
    public string Role { get; set; }
    public int? WorkshopId { get; set; }
}
}