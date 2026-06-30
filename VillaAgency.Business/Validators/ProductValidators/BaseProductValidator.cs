using FluentValidation;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.Business.Validators.ProductValidators
{
    public class BaseProductValidator<T> : AbstractValidator<T> where T : BaseProductDto
    {
        public BaseProductValidator()
        {
            RuleFor(x => x.ImageUrl)
              .NotEmpty().WithMessage("ImageUrl is required.")
              .Matches(@"^https?://.*").WithMessage("Please enter a valid URL.")
              .Must(url => string.IsNullOrEmpty(url) || url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg"))
              .WithMessage("Image URL must end with .jpg, .jpeg or .png");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .IsInEnum().WithMessage("Please select a valid status.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters long.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.BedroomCount)
                .InclusiveBetween(0, 20).WithMessage("Bedroom count must be between 0 and 20.")
                .When(x => x.BedroomCount.HasValue);

            RuleFor(x => x.BathroomCount)
                .GreaterThanOrEqualTo(0).WithMessage("Bathroom count cannot be negative.")
                .When(x => x.BathroomCount.HasValue);

            RuleFor(x => x.Area)
                .GreaterThanOrEqualTo(10).WithMessage("Area must be at least 10 square meters.")
                .When(x => x.Area.HasValue);

            RuleFor(x => x.Floor)
                .InclusiveBetween(-2, 150).WithMessage("Floor must be between -2 and 150.")
                .When(x => x.Floor.HasValue);

            RuleFor(x => x.ParkingCount)
                .GreaterThanOrEqualTo(0).WithMessage("Parking count cannot be negative.")
                .When(x => x.ParkingCount.HasValue);

            RuleFor(x => x.Status)
                .NotNull()
                .WithMessage("Status is required.");
        }
    }
}
