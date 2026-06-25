using FluentValidation;
using VillaAgency.Dto.FeatureDtos;

namespace VillaAgency.Business.Validators.FeatureValidators
{
    public class FeatureDetailValidator : AbstractValidator<FeatureDetailDto>
    {
        public FeatureDetailValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Feature detail title is required.")
                .MaximumLength(150);

            RuleFor(x => x.SubTitle)
                .NotEmpty().WithMessage("Feature detail subtitle is required.")
                .MaximumLength(300);

            RuleFor(x => x.Icon)
                .NotEmpty().WithMessage("Icon URL is required.");
        }
    }
}
