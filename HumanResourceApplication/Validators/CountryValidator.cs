using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
namespace HumanResourceApplication.Validators
{

    public class CountryValidator : AbstractValidator<CountryDTO>
    {
        public CountryValidator()
        {
            // CountryId Validation
            RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country ID is required.").Length(2).WithMessage("Country ID must be exactly two characters.")
            .Matches("^[A-Z]{2}$").WithMessage("Country ID must consist of exactly two uppercase letters.");

            // CountryName Validation
            RuleFor(x => x.CountryName).NotEmpty().WithMessage("Country Name is required.")
            .Must(countryName => char.IsUpper(countryName[0])).WithMessage("Country Name must start with an uppercase letter.");


        }

        
    }
}
