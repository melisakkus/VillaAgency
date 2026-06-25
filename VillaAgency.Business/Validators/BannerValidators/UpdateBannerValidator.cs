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
                .NotEmpty()
                .Must(id => ObjectId.TryParse(id?.ToString(), out _))
                .WithMessage("Invalid feature id.");
        }
    }
}
