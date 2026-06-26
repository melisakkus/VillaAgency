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
                .NotEmpty().WithMessage("Id information is required.")
                .Must(id => ObjectId.TryParse(id, out _))
                .WithMessage("Invalid product id format (Must be a valid ObjectId).");
        }
    }
}
