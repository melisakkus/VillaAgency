using Mapster;
using Microsoft.AspNetCore.Identity;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.AuthDtos;
using VillaAgency.Entity.Identity;

namespace VillaAgency.Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AuthManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto, string role)
        {
            if (dto.Password != dto.ConfirmedPassword)
            {
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
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new AppRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }


        public async Task<SignInResult> LoginAsync(LoginDto dto)
        {
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
                return SignInResult.Failed;

            if (!user.IsActive)
                return SignInResult.NotAllowed;

            return await _signInManager.PasswordSignInAsync(
                   user,
                   dto.Password,
                   dto.RememberMe,
                   lockoutOnFailure: true);

        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


    }
}
