using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface ILocationRepository
    {
        Task AddLocation(LocationDTO location);
        Task UpdateLocation(decimal locationId, LocationDTO location);
        Task<List<LocationDTO>> GetAllLocations();
        Task<LocationDTO> GetLocationById(decimal id);
    }
}
