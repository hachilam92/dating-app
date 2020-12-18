using AutoMapper;
using DTOs;
using Entities;

namespace Helpers
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>();
            CreateMap<Photo, PhotoDTO>();
        }
    }
}