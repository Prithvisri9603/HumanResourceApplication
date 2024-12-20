using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HumanResourceApplication.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IValidator<RegionDTO> _regionValidator;
        private readonly IConfiguration _configuration;

        public RegionController(IRegionRepository regionRepository, IValidator<RegionDTO> regionValidator, IConfiguration configuration)
        {
            _regionRepository = regionRepository;
            _regionValidator = regionValidator;
            _configuration = configuration;
        }
        /// <summary>
        /// Adding new region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost("AddRegion")]
        public async Task<IActionResult> AddNewRegion(RegionDTO region)
        {
            var validationResult = _regionValidator.Validate(region);
            if (!validationResult.IsValid)
            {
                return BadRequest("Validation failed");
            }
            try
            {
                if (region == null)
                {
                    return BadRequest();
                }

                await _regionRepository.AddNewRegion(region);
                return Ok("Record created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateRegion(decimal regionId, RegionDTO regiondto)
        {

            try
            {
                var validationResult = _regionValidator.Validate(regiondto);

                // If validation fails, return BadRequest with the validation errors
                if (!validationResult.IsValid)
                {
                    return BadRequest("Validation failed");
                }
                await _regionRepository.UpdateRegion(regionId, regiondto);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("GetAllRegion")]

        public async Task<IActionResult> ListAllRegion()
        {
            try
            {
                List<RegionDTO> regionlist = await _regionRepository.ListAllRegion();
                return Ok(regionlist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{region_Id}/id")]
        public async Task<IActionResult> GetRegionById(decimal region_Id)
        {
            try
            {
                var regionid = await _regionRepository.GetRegionById(region_Id);
                if (regionid == null)
                {
                    return NotFound("Id does not exsits");
                }
                return Ok(regionid);

            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpDelete("{region_id}/Delete")]
        public async Task<IActionResult> DeleteById(decimal region_id)
        {
            try
            {
                var region = await _regionRepository.GetRegionById(region_id);

                // If the region doesn't exist, return NotFound response
                if (region == null)
                {
                    return NotFound(new { message = "Region not found" });
                }

                // If the region exists, delete it
                await _regionRepository.DeleteRegionById(region_id);

                // Return success response
                return Ok(new { message = "Region deleted successfully" });
            }
        
                 catch (Exception ex)
                {
        
                return Ok(ex.Message);
                }
    }
           
        }



   }

