using FluentValidation;
using MongoDB.Bson;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.Business.Validators.ProductValidators
{
    public class UpdateProductValidator : BaseProductValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                            .NotEmpty()
                            .Must(id => ObjectId.TryParse(id?.ToString(), out _))
                            .WithMessage("Invalid feature id.");
        }
    }
}
