using FluentValidation;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Validators
{
    public class DepartmentDTOValidator : AbstractValidator<DepartmentDTO>
    {
        public DepartmentDTOValidator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("Department Id is required");
            RuleFor(x => x.ManagerId).NotNull();
            RuleFor(x => x.LocationId).NotNull();
            
        }
    }
}
