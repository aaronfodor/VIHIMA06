using AutoMapper;
using concertticket_webapp_appserver.DTOs;
using concertticket_webapp_appserver.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace concertticket_webapp_appserver
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
