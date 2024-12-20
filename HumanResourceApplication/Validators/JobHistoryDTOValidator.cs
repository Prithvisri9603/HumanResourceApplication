using FluentValidation;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Validators
{
    public class JobHistoryDTOValidator : AbstractValidator<JobHistoryDTO>
    {
        public JobHistoryDTOValidator()
        {
            RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Employee ID is required.")
            .Matches(@"^[A-Z]{2}_.*$").WithMessage("Employee ID must be in right format");


            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Employee Start Date is Mandatory");

            //RuleFor(x => x.EndDate).NotEmpty().WithMessage("End Date is required to maintain history");

            RuleFor(x => x.DepartmentId).Must(departmentId => departmentId % 10 == 0).NotEmpty().
                WithMessage("Department ID is required and must be a multiple of 10");
        }

    }
}
