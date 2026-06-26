using FluentValidation;
using MongoDB.Bson;
using VillaAgency.Dto.ContactDtos;

namespace VillaAgency.Business.Validators.ContactValidators
{
    public class UpdateContactValidator : AbstractValidator<UpdateContactDto>
    {
        public UpdateContactValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.")
                .Must(id => ObjectId.TryParse(id, out _))
                .WithMessage("Invalid Id format. It must be a valid 24-character MongoDB ObjectId.");

            RuleFor(x => x.MapUrl)
                .NotEmpty().WithMessage("Map URL is required.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Map URL must be a valid absolute URL.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format. Please use international format (e.g., +1234567890).");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.");
        }
    }
}