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

        #region Get all locations

        /// <summary>
        /// Retrieves all available locations.
        /// </summary>
        /// <returns>
        /// Returns an HTTP 200 OK response with a list of locations retrieved from the repository.
        /// </returns>

        [HttpGet("GetAllLocations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationRepository.GetAllLocations();
            return Ok(locations);
        }
        #endregion

        #region Get loctaion by id

        /// <summary>
        /// Retrieves a specific location by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the location to retrieve.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response with the location details if found.
        /// Returns an HTTP 404 Not Found response if the location with the specified ID is not found.
        /// Returns an HTTP 400 Bad Request response if an error occurs during the operation.
        /// </returns>

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
        #endregion

        #region Add location

        /// <summary>
        /// Adds a new location to the system.
        /// </summary>
        /// <param name="locationDto">The location data to be added.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response with a success message if the location is added successfully.
        /// Returns an HTTP 400 Bad Request response if validation fails or an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint is restricted to users with the "Admin" role.
        /// </remarks>

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
        #endregion

        #region Update location

        /// <summary>
        /// Updates an existing location by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the location to be updated.</param>
        /// <param name="locationDto">The updated location data.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response with a success message if the update is successful.
        /// Returns an HTTP 404 Not Found response if the location does not exist.
        /// Returns an HTTP 400 Bad Request response if validation fails or an error occurs during the operation.
        /// </returns>

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
        #endregion
    }
}
