using FluentValidation;
using VillaAgency.Dto.FeatureDtos;

namespace VillaAgency.Business.Validators.FeatureValidators
{
    public class FAQItemValidator : AbstractValidator<FAQItemDto>
    {
        public FAQItemValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Question is required.")
                .MaximumLength(300);

            RuleFor(x => x.Answer)
                .NotEmpty().WithMessage("Answer is required.")
                .MaximumLength(1000);
        }
    }
}
