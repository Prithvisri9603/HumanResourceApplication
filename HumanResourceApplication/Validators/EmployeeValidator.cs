using FluentValidation;
using HumanResourceApplication.DTO;
using Microsoft.Identity.Client;

namespace HumanResourceApplication.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeDTO>
    {
        public EmployeeValidator() 
        {
            RuleFor(x=>x.EmployeeId).NotEmpty().WithMessage("Enter a valid Employee ID").Must(x=>x>=100 && x<=999).WithMessage("Employee ID is 3-digit number only");
            RuleFor(x => x.FirstName).Matches(@"^[a-zA-Z]+$").WithMessage("First Name Should only contain letters").Matches(@"^[A-Z]").WithMessage("Name Should Start With Capital Letter");
            RuleFor(x => x.LastName).Matches(@"^[a-zA-Z]+$").WithMessage("Last Name Should only contain letters").Matches(@"^[A-Z]").WithMessage("Name Should Start With Capital Letter");
            RuleFor(x => x.JobId).NotEmpty().WithMessage("Enter a valid Job ID");
            RuleFor(x => x.CommissionPct).LessThanOrEqualTo(1).WithMessage("The Commission should be less than 1");
        }
    }
}
