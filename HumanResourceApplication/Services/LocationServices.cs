using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using Microsoft.EntityFrameworkCore;


namespace HumanResourceApplication.Services
{
    public class LocationServices:ILocationRepository
    {
        private readonly HrContext _context;
        private readonly IMapper _mapper;

        public LocationServices(HrContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<List<LocationDTO>> GetAllLocations()
        {
            var locationList = await _context.Locations.ToListAsync();
            var locationDTOList=_mapper.Map<List<LocationDTO>>(locationList);
            return locationDTOList;
        }
        
        public async Task<LocationDTO?> GetLocationById(decimal id)
        {
            var location = await _context.Locations.FindAsync(id);
            if(location==null)
            {
                return null;
            }
            return _mapper.Map<LocationDTO>(location);
        }
      
        public async Task AddLocation(LocationDTO locationDto)
        {
            var LocationData = _mapper.Map<Location>(locationDto);
            _context.Locations.Add(LocationData);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLocation(decimal locationId, LocationDTO locationDto)
        {
            var existing = await _context.Locations.FindAsync(locationId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(locationId);
                await _context.SaveChangesAsync();
            }
        }
      
    }
}

