using YCompanyClaimsApi.Models;
using System.Collections.Generic;

namespace YCompanyClaimsApi.Services
{
public interface IUserService
{
    Task<User> AuthenticateAsync(string username, string password);
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);

}
}