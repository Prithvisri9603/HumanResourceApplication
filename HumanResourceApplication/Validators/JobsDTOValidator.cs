using FluentValidation;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Validators
{

    public class JobsDTOValidator : AbstractValidator<JobDTO>
    {
        public JobsDTOValidator()
        {
            RuleFor(x => x.JobTitle)
                .NotEmpty().WithMessage("Job Title is required")
                .MaximumLength(100).WithMessage("Job Title must not exceed 100 characters");

            RuleFor(x => x.MinSalary)
                .GreaterThanOrEqualTo(1000).WithMessage("Minimum Salary must be greater than or equal to 1000");

            RuleFor(x => x.MaxSalary)
                .GreaterThan(x => x.MinSalary).WithMessage("Maximum Salary must be greater than Minimum Salary");

        }
    }
}
