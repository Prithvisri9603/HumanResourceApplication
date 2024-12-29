using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using HumanResourceApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HumanResourceApplication.Utility;
using System.ComponentModel.DataAnnotations;

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IValidator<CountryDTO> _countryValidator;
       
        

        public CountryController(ICountryRepository countryRepository, IValidator<CountryDTO> validator)
        {
            _countryRepository = countryRepository;
            _countryValidator = validator;
        }


        #region Get All Countries

        /// <summary>
        /// Retrieves a list of all countries from the database.
        /// </summary>
        /// <returns>A list of CountryDTO objects or a NotFound response if no countries are found.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("List all the Countries")]
        public async Task<ActionResult<List<CountryDTO>>> GetAllCountries()
        {
            try
            {
                var countries = await _countryRepository.GetAllCountries();
                if (countries == null || countries.Count == 0)
                {
                    return NotFound(new { message = "No countries found." });
                }

                return Ok(countries);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        #region Get Country By Id

        /// <summary>
        /// Retrieves a specific country by its ID from the database.
        /// </summary>
        /// <param name="Countryid">The ID of the country to retrieve.</param>
        /// <returns>A CountryDTO object if found, otherwise a NotFound response.</returns>

        [Authorize(Roles = "Admin,HR Team")]
        [HttpGet("Get country by id")]
       
        public async Task<ActionResult<CountryDTO>> GetCountryById(string Countryid)
        {
            try
            {
                var country = await _countryRepository.GetCountryById(Countryid);
                if (country == null)
                {
                    return NotFound(new { message = "Country not found." });
                }

                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Add Country

        /// <summary>
        /// Adds a new country to the database.
        /// </summary>
        /// <param name="country">The CountryDTO object containing the new country data.</param>
        /// <returns>A success message or a BadRequest response if validation fails.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("Add new country")]

        public async Task<IActionResult> AddCountry(CountryDTO country)
        {
            /*try
            {
                var validationResult = await _countryValidator.ValidateAsync(country);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                await _countryRepository.AddCountry(country);
                return Ok("Record added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            try
            {
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");



                // Check if the country already exists
                var existingCountry = await _countryRepository.GetCountryById(country.CountryId);
                if (existingCountry != null)
                {
                    throw new AlreadyExistsException($"Country '{country.CountryId}' already exists.");
                }

                // Validate the country data
                var validationResult = _countryValidator.Validate(country);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    throw new CustomeValidationException(errors, timeStamp);
                }

                // Add the new country to the repository
                await _countryRepository.AddCountry(country);

                // Return success response with timestamp
                return Ok(new
                {
                    //TimeStamp = timeStamp,
                    Message = "Country record created successfully"
                });
            }

            catch (Exception ex)
            {
                // Return BadRequest error response for any other exception
                return BadRequest(new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = ex.Message
                });
            }


        }

        
        #endregion

        #region Update Country

        /// <summary>
        /// Updates an existing country in the database.
        /// </summary>
        /// <param name="Countryid">The ID of the country to update.</param>
        /// <param name="country">The CountryDTO object containing the updated country data.</param>
        /// <returns>A success message or a BadRequest response if validation fails.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("Modify the country")]
        
        public async Task<IActionResult> UpdateCountry(string Countryid ,CountryDTO country)
        {
            /*try
            {
                var validationResult = await _countryValidator.ValidateAsync(country);
                if (!validationResult.IsValid)
                {

                    return BadRequest(validationResult.Errors);

                }
                await _countryRepository.UpdateCountry(Countryid, country);
                return Ok("Record updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            try
            {
                // Capture the current timestamp in UTC ISO 8601 format
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Validate the country DTO using the validator
                var validationResult = await _countryValidator.ValidateAsync(country);
                if (!validationResult.IsValid)
                {
                    // Throw a custom validation exception if validation fails
                    throw new CustomeValidationException(validationResult.Errors.Select(e => e.ErrorMessage).ToList(), timeStamp);
                }

                // Check if the country exists by its ID
                var existingCountry = await _countryRepository.GetCountryById(Countryid);
                if (existingCountry == null)
                {
                    // Throw an exception if the country is not found
                    throw new AlreadyExistsException("Country not found.");
                }

                // Proceed to update the country if it exists
                await _countryRepository.UpdateCountry(Countryid, country);

                // Return a successful response with the timestamp
                return Ok(new
                {
                    //TimeStamp = timeStamp,
                    Message = "Country record updated successfully"
                });
            }
            catch (Exception ex)
            {
                // Handle general exceptions, return a bad request with the exception message and timestamp
                return BadRequest(new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                });
            }





        }
        #endregion

        #region  Delete Country By Id

        /// <summary>
        /// Deletes a country by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the country to delete.</param>
        /// <returns>A NoContent response on success, or a BadRequest response on failure.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpDelete("Delete country by id")]
        public async Task<IActionResult> DeleteCountryById(string id)
        {
            try
            {
                await _countryRepository.DeleteCountryById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


    }

}
