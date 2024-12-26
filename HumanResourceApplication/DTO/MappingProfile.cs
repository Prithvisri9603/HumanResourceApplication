using AutoMapper;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
<<<<<<< Updated upstream
            CreateMap<Employee,EmployeeDTO>().ReverseMap();
=======
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
            CreateMap<Job, JobDTO>().ReverseMap();
            CreateMap<Job, SpDTO>().ReverseMap();
            CreateMap<JobHistory, JobHistoryDTO>().ReverseMap();
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Location, LocationDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Region, RegionDTO>().ReverseMap();

>>>>>>> Stashed changes
        }
    }
}
