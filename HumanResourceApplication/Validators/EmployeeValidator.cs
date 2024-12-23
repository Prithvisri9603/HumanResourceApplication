using FluentValidation;
using HumanResourceApplication.DTO;
using Microsoft.Identity.Client;

namespace HumanResourceApplication.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeDTO>
    {
        private readonly List<string> validJobIds = new()
        {
            "AC_ACCOUNT", "AC_MGR", "AD_ASST", "AD_PRES", "AD_VP",
            "FI_ACCOUNT", "FI_MGR", "HR_REP", "IT_PROG", "MK_MAN",
            "MK_REP", "PR_REP", "PU_CLERK", "PU_MAN", "SA_MAN",
            "SA_REP", "SH_CLERK", "ST_CLERK", "ST_MAN"
        };

            private readonly List<decimal> validDepartmentIds = new()
            {
                10, 20, 30, 40, 50, 60, 70, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170, 180, 190, 200,
                210, 220, 230, 240, 250, 260, 270
            };

            public EmployeeValidator()
            {
            
                RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Please provide an Employee ID.").InclusiveBetween(100, 999).WithMessage("Employee ID must be a 3-digit number between 100 and 999.");
                RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please provide a First Name.").Matches(@"^[a-zA-Z]+$").WithMessage("First Name should only contain letters.").Matches(@"^[A-Z]").WithMessage("First Name should start with a capital letter.");
                RuleFor(x => x.LastName).NotEmpty().WithMessage("Please provide a Last Name.").Matches(@"^[a-zA-Z]+$").WithMessage("Last Name should only contain letters.").Matches(@"^[A-Z]").WithMessage("Last Name should start with a capital letter.");
                RuleFor(x => x.Email).NotEmpty().WithMessage("Please provide an Email.").EmailAddress().WithMessage("Please provide a valid Email address.");
                RuleFor(x => x.JobId).NotEmpty().WithMessage("Please provide a Job ID.").Must(jobId => validJobIds.Contains(jobId)).WithMessage("Invalid Job ID. Please choose from the valid Job IDs: " + string.Join(", ", validJobIds));
                RuleFor(x => x.CommissionPct).GreaterThanOrEqualTo(0).WithMessage("Commission percentage cannot be negative.").LessThanOrEqualTo(1).WithMessage("Commission percentage must be between 0 and 1.");
                RuleFor(x => x.DepartmentId).Must(departmentId => departmentId == null || validDepartmentIds.Contains((decimal)departmentId)).WithMessage("Invalid Department ID. Please choose from the valid Department IDs: " + string.Join(", ", validDepartmentIds));
            }
    }

}

