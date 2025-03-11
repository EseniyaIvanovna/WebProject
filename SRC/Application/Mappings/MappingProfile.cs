using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Message, MessageDto>().ReverseMap();
            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<Reaction, ReactionDto>().ReverseMap();
            CreateMap<Interaction, InteractionDto>().ReverseMap();
            

        }
    }
}
