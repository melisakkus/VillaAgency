using FluentValidation;
using VillaAgency.Dto.VideoViewDtos;

namespace VillaAgency.Business.Validators.VideoViewValidators
{
    public class CreateVideoViewValidator : AbstractValidator<CreateVideoViewDto>
    {
        public CreateVideoViewValidator()
        {
            RuleFor(x => x.VideoUrl)
                .NotEmpty().WithMessage("Video URL is required.")
                .NotNull().WithMessage("Video URL cannot be null.")
                .MaximumLength(500).WithMessage("Video URL cannot exceed 500 characters.")
                .Must(BeAValidUrl).WithMessage("Invalid video URL format.");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
