using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Authorization;
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


        [Authorize(Roles = "Admin, HR Team, Employee")]
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


        [Authorize(Roles = "Admin, HR Team")]
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


        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin, HR Team")]
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

        [Authorize(Roles = "Admin, HR Team")]
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


    }
    
}
