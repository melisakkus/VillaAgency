using FluentValidation;
using MongoDB.Bson;
using VillaAgency.Dto.MessageDtos;

namespace VillaAgency.Business.Validators.MessageValdiators
{
    public class UpdateMessageValidator : AbstractValidator<UpdateMessageDto>
    {
        public UpdateMessageValidator()
        {
            RuleFor(x => x.Id)
                            .NotEmpty().WithMessage("Id information is required.")
                            .Must(id => ObjectId.TryParse(id, out _))
                            .WithMessage("Invalid message id format (Must be a valid ObjectId).");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MinimumLength(3).WithMessage("Subject must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Subject must not exceed 100 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content is required.")
                .MinimumLength(10).WithMessage("Message must be at least 10 characters.")
                .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");
        }
    }
}
