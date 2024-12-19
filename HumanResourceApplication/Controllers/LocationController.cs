using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IValidator<LocationDTO> _validator;

        public LocationController(ILocationRepository locationRepository,IValidator<LocationDTO> validator)
        {
            _locationRepository = locationRepository;
            _validator = validator;
        }
        [HttpGet("GetAllLocations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationRepository.GetAllLocations();
            return Ok(locations);
        }
        [HttpGet]
        public async Task<IActionResult> GetLocationById(decimal id)
        {
            try
            {
                LocationDTO location = await _locationRepository.GetLocationById(id);
                if (location == null)
                {
                    return NotFound($"Location with Id= {id} not found");
                }
                return Ok(location);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]

        public async Task<IActionResult> AddLocation(LocationDTO locationDto)
        {
            try
            {
                var validationResult = _validator.Validate(locationDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                await _locationRepository.AddLocation(locationDto);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("id")]
        public async Task<IActionResult> UpdateLocation(int id, LocationDTO locationDto)
        {
            try
            {
                var validationResult = _validator.Validate(locationDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                var existingLocations = await _locationRepository.GetAllLocations();
                if (existingLocations == null)
                {
                    return NotFound("Location not found.");
                }

                await _locationRepository.UpdateLocation(id, locationDto);
                return Ok("Record Modified Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
