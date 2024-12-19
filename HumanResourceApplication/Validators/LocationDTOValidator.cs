using FluentValidation;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Validators
{
    public class LocationDTOValidator:AbstractValidator<LocationDTO>
    {
        public LocationDTOValidator()
        {
            RuleFor(x => x.StreetAddress)   
                .NotEmpty().WithMessage("Street Address is required.")
                .MaximumLength(255).WithMessage("Street Address cannot exceed 255 characters.");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal Code cannot exceed 20 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.StateProvince)
                .MaximumLength(100).WithMessage("State/Province cannot exceed 100 characters.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country ID is required.")
                .Length(2).WithMessage("Country ID must be exactly 2 characters.")
                .Matches("^[A-Z]{2}$").WithMessage("Country ID must be two uppercase letters.");
        }
    }
}
    

