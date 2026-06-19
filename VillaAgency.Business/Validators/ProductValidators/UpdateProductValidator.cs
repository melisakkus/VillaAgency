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
                .NotEmpty().WithMessage("Product ID is required for updating.")
                .NotEqual(ObjectId.Empty).WithMessage("Invalid Product ID.");
        }
    }
}
