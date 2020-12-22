using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using DTOs;
using Entities;
using Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;

namespace Helpers
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
                .ForMember(
                    dest => dest.PhotoUrl,
                    opt => opt.MapFrom(
                        src => src.Photos.FirstOrDefault(
                            x => x.IsMain
                        ).Url))
                .ForMember(
                    dest => dest.Age,
                    OPT => OPT.MapFrom(
                        src => src.DateOfBirth.CalculateAge()
                    ));

            CreateMap<Photo, PhotoDTO>();

            CreateMap<MemberUpdateDTO, AppUser>();
        }
    }
}