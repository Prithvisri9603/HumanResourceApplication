using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.Services
{
    public interface IRegionRepository
    {
        //Add new Region
        Task AddNewRegion(RegionDTO region);

        //Modify the region

        Task UpdateRegion(decimal regionId, RegionDTO region);
        //List Add all region

        Task<List<RegionDTO>> ListAllRegion();

        //Get region by id
        Task<RegionDTO> GetRegionById(decimal regionId);

        //delete region by id
        Task DeleteRegionById(decimal regionId);
    }
}
