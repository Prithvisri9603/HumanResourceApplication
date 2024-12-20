using AutoMapper;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.DTO
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Region, RegionDTO>().ReverseMap();
            //CreateMap<RegionDTO, Region > ();//Mapping from region to regionDTO
            //Mapping from region to regionDTO
            CreateMap<Department, DepartmentDTO>().ReverseMap();

        }
    }
}
