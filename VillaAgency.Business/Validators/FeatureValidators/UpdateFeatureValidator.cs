using FluentValidation;
using MongoDB.Bson;
using VillaAgency.Dto.FeatureDtos;

namespace VillaAgency.Business.Validators.FeatureValidators
{
    public class UpdateFeatureValidator : AbstractValidator<UpdateFeatureDto>
    {
        public UpdateFeatureValidator()
        {
            RuleFor(x => x.Id)
                            .NotEmpty()
                            .Must(id => ObjectId.TryParse(id?.ToString(), out _)) 
                            .WithMessage("Invalid feature id.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(BeAValidUrl).WithMessage("Image URL must be a valid URL.");

            RuleFor(x => x.FAQs)
                .NotNull().WithMessage("FAQs list cannot be null.")
                .Must(x => x.Count > 0).WithMessage("At least one FAQ item is required.")
                .Must(x => x.Count <= 20).WithMessage("FAQs cannot contain more than 20 items.");

            RuleForEach(x => x.FAQs)
                .SetValidator(new FAQItemValidator());

            RuleFor(x => x.FeatureDetails)
                .NotNull().WithMessage("FeatureDetails list cannot be null.")
                .Must(x => x.Count > 0).WithMessage("At least one Feature Detail is required.")
                .Must(x => x.Count <= 10).WithMessage("FeatureDetails cannot contain more than 10 items.");

            RuleForEach(x => x.FeatureDetails)
                .SetValidator(new FeatureDetailValidator());
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}