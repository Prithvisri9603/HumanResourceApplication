using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using HumanResourceApplication.Validators;
using Microsoft.AspNetCore.Authorization;
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
        //private readonly IConfiguration _configuration;

        public RegionController(IRegionRepository regionRepository, IValidator<RegionDTO> regionValidator)
        {
            _regionRepository = regionRepository;
            _regionValidator = regionValidator;
            //_configuration = configuration;
        }
        #region AddNewRegion
        [Authorize(Roles = "Admin")]
        [HttpPost("AddRegion")]
        public async Task<IActionResult> AddNewRegion(RegionDTO region)
        {
            var validationResult = await _regionValidator.ValidateAsync(region);
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
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred." });
            }
        }

        //public async Task<IActionResult> AddNewRegion(RegionDTO region)
        //{
        //    try
        //    {
        //        var validationResult = await _regionValidator.ValidateAsync(region);
        //        if (!validationResult.IsValid)
        //        {
        //            return BadRequest(validationResult.Errors);
        //        }
        //        await _regionRepository.AddNewRegion(region);
        //        return Ok("Record created successfully");
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest(new { Message = "An error occurred." });
        //    }
        //}

        #endregion

        #region UpdateRegion
        [Authorize(Roles = "Admin, HR Team")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateRegion(decimal regionId, RegionDTO regiondto)
        {

            try
            {
                var validationResult =await  _regionValidator.ValidateAsync(regiondto);

                // If validation fails, return BadRequest with the validation errors
                if (!validationResult.IsValid)
                {
                    return BadRequest("Validation failed");
                }
                await _regionRepository.UpdateRegion(regionId, regiondto);
                return Ok("Record Modified Successfully");
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred." });

            }
        }
        #endregion

        #region ListAllRegion
        [Authorize(Roles = "Admin, HR Team, Employee")]
        [HttpGet("GetAllRegion")]
        //did changes here
        public async Task<IActionResult> ListAllRegion()
        {
            try
            {
                List<RegionDTO> regionlist = await _regionRepository.ListAllRegion();

                if (regionlist == null || !regionlist.Any())
                {
                    return NotFound(new { Message = "No regions found." });
                }

                return Ok(regionlist);
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred." });
            }
        }

        #endregion

        #region GetRegionById
        [Authorize(Roles = "Admin, HR Team")]
        [HttpGet("{region_Id}/id")]
        public async Task<IActionResult> GetRegionById(decimal region_Id)
        {
            try
            {
                var regionid = await _regionRepository.GetRegionById(region_Id);
                if (regionid == null)
                {
                    return NotFound(new { message = "Region not found." });
                }
                return Ok(regionid);

            }
            catch(Exception)
            {
                return BadRequest("An error occurred.");
            }
        }
        #endregion

        #region DeleteById
        [Authorize(Roles = "Admin, HR Team")]
        [HttpDelete("{region_id}/Delete")]
        public async Task<IActionResult> DeleteById(decimal region_id)
        {
            try
            {
                //var region = await _regionRepository.GetRegionById(region_id);

                // If the region doesn't exist, return NotFound response
                //if (region == null)
                //{
                //    return NotFound(new { message = "Region not found" });
                //}

                // If the region exists, delete it
                await _regionRepository.DeleteRegionById(region_id);

                // Return success response
                //return Ok(new { message = "Region deleted successfully" });
                return Ok("Region deleted successfully");
            }
        
             catch (Exception)
                {
        
                return BadRequest("Error deleting");
              }
             }

        
    }
    #endregion



}

