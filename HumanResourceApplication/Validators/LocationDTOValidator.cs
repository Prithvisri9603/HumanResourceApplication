using System.Net;
using FluentValidation;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Validators
{
    public class LocationDTOValidator:AbstractValidator<LocationDTO>
    {

        public LocationDTOValidator()
        {
                // StreetAddress Custom Validation
                RuleFor(x => x.StreetAddress).Custom((streetAddress, context) =>
                    {
                        // Check if StreetAddress is empty
                        if (string.IsNullOrEmpty(streetAddress))
                        {
                            context.AddFailure("Street Address is required.");
                        }
                        // Check if StreetAddress more than 255 characters
                        else if (streetAddress.Length > 255)
                        {
                            context.AddFailure("Street Address cannot exceed 255 characters.");
                        }
                    });

                // PostalCode Custom Validation
                RuleFor(x => x.PostalCode).Custom((postalCode, context) =>
                    {
                        // Check if PostalCode exceeds 20 characters
                        if (postalCode.Length > 20)
                        {
                            context.AddFailure("Postal Code cannot exceed 20 characters.");
                        }
                    });

                // City Custom Validation
                RuleFor(x => x.City).Custom((city, context) =>
                    {
                        // Check if City is empty
                        if (string.IsNullOrEmpty(city))
                        {
                            context.AddFailure("City is required.");
                        }
                        // Check if City exceeds 100 characters
                        else if (city.Length > 100)
                        {
                            context.AddFailure("City cannot exceed 100 characters.");
                        }
                    });

                // StateProvince Custom Validation
                RuleFor(x => x.StateProvince).Custom((stateProvince, context) =>
                    {
                        // Check if StateProvince exceeds 100 characters
                        if (stateProvince.Length > 100)
                        {
                            context.AddFailure("State/Province cannot exceed 100 characters.");
                        }
                    });

                // CountryId Custom Validation
                RuleFor(x => x.CountryId).Custom((countryId, context) =>
                    {
                        // Check if CountryId is empty
                        if (string.IsNullOrEmpty(countryId))
                        {
                            context.AddFailure("Country ID is required.");
                        }
                        // Check if CountryId is not exactly 2 characters
                        else if (countryId.Length != 2)
                        {
                            context.AddFailure("Country ID must be exactly 2 characters.");
                        }
                        // Check if CountryId contains only uppercase letters
                        else if (!System.Text.RegularExpressions.Regex.IsMatch(countryId, "^[A-Z]{2}$"))
                        {
                            context.AddFailure("Country ID must be two uppercase letters.");
                        }
                    });
        }
    }

    
}
    

