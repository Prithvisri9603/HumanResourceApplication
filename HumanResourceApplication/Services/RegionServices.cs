using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceApplication.Services
{
    public class RegionServices:IRegionRepository
    {
        private readonly HrContext _hrContext;
        private readonly IMapper _mapper;
        #region constructor injection
        public RegionServices(HrContext hrContext, IMapper mapper)
        {
            _hrContext = hrContext;
            _mapper = mapper;
        }
        #endregion

        #region AddNewRegion
        /// <summary>
        /// Input:regionId ::decimal
        ///        regionName::string
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public async Task AddNewRegion(RegionDTO region)
        {
            var addRegion = _mapper.Map<Region>(region);
            _hrContext.Regions.Add(addRegion);
            await _hrContext.SaveChangesAsync();
        }
        #endregion



        #region UpdateRegion
        /// <summary>
        /// intput:Enter the region id as decimal
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateRegion(decimal regionId,RegionDTO region)
        {
            var regionData = await _hrContext.Regions.FindAsync(regionId);
            var r = _mapper.Map<Region>(regionData);
            if (r != null)
            {
                r.RegionName = region.RegionName;
                await _hrContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Region Id is not found");
            }

        }
        #endregion


        #region ListAllRegion
        /// <summary>
        /// Getting all the data
        /// </summary>
        /// <returns></returns>
        public async  Task<List<RegionDTO>> ListAllRegion()
        {
            var regionlist = await _hrContext.Regions.ToListAsync();
            var regionDTOList = _mapper.Map<List<RegionDTO>>(regionlist);
            return regionDTOList;

        }
        #endregion


        #region GetRegionById
        /// <summary>
        /// Input:regionId::decimal
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public async Task<Region> GetRegionById(decimal regionId)
        {
            var regionData = await _hrContext.Regions.FirstOrDefaultAsync(a => a.RegionId == regionId);
            if(regionData == null)
            {
                return null;
            }
            return regionData;
        }

        #endregion



        #region DeleteRegionById
        /// <summary>
        /// Input:Enter the regionId in decimal to delete
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public async Task DeleteRegionById(decimal regionId)
        {
            var regionToDelete = await _hrContext.Regions.FindAsync(regionId);
            if(regionToDelete != null)
            {
                _hrContext.Regions.Remove(regionToDelete);
                await _hrContext.SaveChangesAsync();
            }
            
        }
        #endregion

    }
}
