using AutoMapper;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.DTO
{
    public class MappingProfile : Profile
    {

        public MappingProfile() 
        {
            CreateMap<Employee,EmployeeDTO>().ReverseMap();
            CreateMap<Job, JobDTO>().ReverseMap();
            CreateMap<JobHistory, JobHistoryDTO>().ReverseMap();
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Location, LocationDTO>().ReverseMap();
        }


    }
}
