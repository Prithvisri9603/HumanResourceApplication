using AutoMapper;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.DTO
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
            CreateMap<Location, LocationDTO>().ReverseMap();
        }
    }
}
