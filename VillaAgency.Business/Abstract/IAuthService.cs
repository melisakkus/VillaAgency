using Microsoft.AspNetCore.Identity;
using VillaAgency.Dto.AuthDtos;

namespace VillaAgency.Business.Abstract
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto, string role);
        Task<SignInResult> LoginAsync(LoginDto dto);
        Task LogoutAsync();
        Task<IdentityResult> DeleteUserAsync(string userId);
    }
}
