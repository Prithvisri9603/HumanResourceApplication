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

        #region Get all locations

        /// <summary>
        /// Retrieves all locations from the database and maps them to a list of DTO objects.
        /// </summary>
        /// <returns>
        /// A list of <see cref="LocationDTO"/> objects representing all locations.
        /// </returns>

        public async Task<List<LocationDTO>> GetAllLocations()
        {
            var locationList = await _context.Locations.ToListAsync();
            var locationDTOList=_mapper.Map<List<LocationDTO>>(locationList);
            return locationDTOList;
        }
        #endregion

        #region Get location by id

        /// <summary>
        /// Retrieves a specific location by its unique identifier from the database and maps it to a DTO object.
        /// </summary>
        /// <param name="id">The unique identifier of the location to retrieve.</param>
        /// <returns>
        /// A <see cref="LocationDTO"/> object representing the location if found; otherwise, null if no location exists with the specified ID.
        /// </returns>

        public async Task<LocationDTO?> GetLocationById(decimal id)
        {
            var location = await _context.Locations.FindAsync(id);
            if(location==null)
            {
                return null;
            }
            return _mapper.Map<LocationDTO>(location);
        }
        #endregion

        #region Add location

        /// <summary>
        /// Asynchronously adds a new location to the database.
        /// Maps the provided LocationDTO to a Location entity and saves it to the Locations table.
        /// </summary>
        /// <param name="locationDto">The LocationDTO containing the data to be added.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>

        public async Task AddLocation(LocationDTO locationDto)
        {
            var LocationData = _mapper.Map<Location>(locationDto);
            _context.Locations.Add(LocationData);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Update location

        /// <summary>
        /// Asynchronously updates an existing location in the database.
        /// Finds the location by its unique identifier and updates its values with the provided LocationDTO.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location to update.</param>
        /// <param name="locationDto">The LocationDTO containing the updated data.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>

        public async Task UpdateLocation(decimal locationId, LocationDTO locationDto)
        {
            var existing = await _context.Locations.FindAsync(locationId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(locationId);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

    }
}

