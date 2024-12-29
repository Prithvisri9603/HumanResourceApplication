using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
namespace HumanResourceApplication.Validators
{

    public class CountryValidator : AbstractValidator<CountryDTO>
    {
        public CountryValidator()
        {
            // CountryId Custom Validation
            RuleFor(x => x.CountryId).Custom((countryId, context) =>
                {
                    // Check if CountryId is empty
                    if (string.IsNullOrEmpty(countryId))
                    {
                        context.AddFailure("Country ID is required.");
                    }
                    // Check if Countryid length is exactly 2 characters
                    else if (countryId.Length != 2)
                    {
                        context.AddFailure("Country ID must be exactly two characters.");
                    }
                    // Check if Countryid matches the pattern only uppercase letters
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(countryId, "^[A-Z]{2}$"))
                    {
                        context.AddFailure("Country ID must consist of exactly two uppercase letters.");
                    }
                });

            // CountryName Custom Validation
            RuleFor(x => x.CountryName).Custom((countryName, context) =>
                {
                    // Check if CountryName is empty
                    if (string.IsNullOrEmpty(countryName))
                    {
                        context.AddFailure("Country Name is required.");
                    }
                    // Check if CountryName starts with an uppercase letter
                    else if (!char.IsUpper(countryName[0]))
                    {
                        context.AddFailure("Country Name must start with an uppercase letter.");
                    }
                });
        }
    }

}
