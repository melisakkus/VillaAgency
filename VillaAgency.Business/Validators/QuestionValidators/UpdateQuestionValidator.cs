using FluentValidation;
using VillaAgency.Dto.QuestionDtos;

namespace VillaAgency.Business.Validators.QuestionValidators
{
    public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionDto>
    {
        public UpdateQuestionValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id cannot be empty.");

            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("Question text cannot be empty.")
                .MinimumLength(5).WithMessage("Question must be at least 5 characters long.")
                .MaximumLength(500).WithMessage("Question cannot exceed 500 characters.");

            RuleFor(x => x.Answer)
                .NotEmpty().WithMessage("Answer cannot be empty.")
                .MinimumLength(2).WithMessage("Answer must be at least 2 characters long.")
                .MaximumLength(2000).WithMessage("Answer cannot exceed 2000 characters.");
        }    
    }
}
