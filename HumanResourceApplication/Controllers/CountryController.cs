using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
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
        [HttpGet("id")]
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

        [HttpPost]
        public async Task<IActionResult> AddCountry(CountryDTO country)
        {
            try
            {
                var validationResult =await _countryValidator.ValidateAsync(country);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                await _countryRepository.AddCountry( country);
                return Ok("Record added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
        [HttpPut]
        public async Task<IActionResult> UpdateCountry(string Countryid ,CountryDTO country)
        {
            try
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
            }
        }
        #endregion

        #region  Delete Country By Id

        /// <summary>
        /// Deletes a country by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the country to delete.</param>
        /// <returns>A NoContent response on success, or a BadRequest response on failure.</returns>
       
        [HttpDelete("id")]
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
