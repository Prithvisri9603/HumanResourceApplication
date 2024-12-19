﻿using AutoMapper;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Employee,EmployeeDTO>().ReverseMap();
        }
    }
}
