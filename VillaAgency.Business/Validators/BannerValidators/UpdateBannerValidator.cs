using FluentValidation;
using MongoDB.Bson;
using VillaAgency.Dto.BannerDtos;

namespace VillaAgency.Business.Validators.BannerValidators
{
    public class UpdateBannerValidator : BaseBannerValidator<UpdateBannerDto>
    {
        public UpdateBannerValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Banner ID is required for updating.")
                .NotEqual(ObjectId.Empty).WithMessage("Invalid Product ID.");
        }
    }
}
