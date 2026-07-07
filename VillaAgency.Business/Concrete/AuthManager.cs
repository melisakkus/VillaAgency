using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.AuthDtos;
using VillaAgency.Entity.Identity;

namespace VillaAgency.Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthManager> _logger;

        public AuthManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AuthManager> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto, string role)
        {
            if (dto.Password != dto.ConfirmedPassword)
            {
                _logger.LogWarning("Registration failed: Password mismatch for user {UserName}", dto.UserName);
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "PasswordMismatch",
                        Description = "Password and confirmation password do not match."
                    });
            }
            var user = dto.Adapt<AppUser>();
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User registered successfully: {UserName}", user.UserName);
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Identity Error during registration: {Code} - {Description}", error.Code, error.Description);
                }
            }
            return result;
        }


        public async Task<SignInResult> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for: {UserIdentifier}", dto.UserNameOrEmail);

            if (string.IsNullOrWhiteSpace(dto.UserNameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return SignInResult.Failed;
            }

            AppUser? user;

            if (dto.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(dto.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(dto.UserNameOrEmail);
            }

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found ({UserIdentifier})", dto.UserNameOrEmail);
                return SignInResult.Failed;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: Account inactive ({UserIdentifier})", dto.UserNameOrEmail);
                return SignInResult.NotAllowed;
            }

            var result = await _signInManager.PasswordSignInAsync(
                                                   user,
                                                   dto.Password,
                                                   dto.RememberMe,
                                                   lockoutOnFailure: true);

            if (result.Succeeded)
                _logger.LogInformation("Login successful: {UserIdentifier}", dto.UserNameOrEmail);
            else
                _logger.LogWarning("Login failed: Invalid password for {UserIdentifier}", dto.UserNameOrEmail);

            return result;

        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully.");
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Delete failed: User not found with ID {UserId}", userId);
                return IdentityResult.Failed(new IdentityError { Code = "NotFound", Description = "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                _logger.LogInformation("User deleted successfully: {UserName}", user.UserName);
            else
                _logger.LogError("Delete failed for user {UserName}: {Errors}", user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));

            return result;
        }
    }
}
