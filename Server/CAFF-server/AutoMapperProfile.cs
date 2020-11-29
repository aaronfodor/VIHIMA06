using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>()
                .ForMember(d => d.Id, s => s.Ignore());
            CreateMap<CAFF, CAFFDTO>();
            CreateMap<Comment, CommentDTO>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.User.Name));
            CreateMap<CommentDTO, Comment>();
        }
    }
}
