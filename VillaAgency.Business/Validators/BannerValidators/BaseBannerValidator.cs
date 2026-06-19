using FluentValidation;
using VillaAgency.Dto.BannerDtos;

namespace VillaAgency.Business.Validators.BannerValidators
{
    public class BaseBannerValidator<T> : AbstractValidator<T> where T : BaseBannerDto
    {
        protected BaseBannerValidator()
        {
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City information is required.")
                .MinimumLength(3).WithMessage("City name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("City name cannot exceed 50 characters.")
                .Matches(@"^[a-zA-Z\s]*$").WithMessage("City name can only contain letters and spaces.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Banner title is required.")
                .MinimumLength(3).WithMessage("Banner title must be at least 3 characters long to look good on screen.")
                .MaximumLength(150).WithMessage("Banner title cannot exceed 150 characters to prevent design breaking.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Banner image URL is required.")
                .Matches(@"^https?://.*").WithMessage("Please enter a valid URL (starting with http:// or https://).")
                .Must(url => string.IsNullOrEmpty(url) ||
                             url.EndsWith(".jpg") ||
                             url.EndsWith(".jpeg") ||
                             url.EndsWith(".png") ||
                             url.EndsWith(".webp"))
                .WithMessage("Image URL must end with a valid extension (.jpg, .jpeg, .png, or .webp).");
        }
    }
}
