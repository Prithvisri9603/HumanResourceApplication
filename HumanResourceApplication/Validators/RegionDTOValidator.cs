using FluentValidation;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Validators
{
    public class RegionDTOValidator : AbstractValidator<RegionDTO>
    {
        public RegionDTOValidator()
        {
            RuleFor(x => x.RegionId).NotEmpty().WithMessage("Region Id is required");
            

        }

    }
}
