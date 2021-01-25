using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using DTOs;
using Entities;
using Helpers;
using Microsoft.AspNetCore.Http;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<PagedList<MemberDTO>> GetUsers(string username, UserParams userParams);
        Task<MemberDTO> GetUser(string username);
        Task<AppUser> GetUserByUsername(string username);
        Task<bool> UpdateUser(MemberUpdateDTO memberUpdateDTO, string username);
        Task<Photo> AddPhotoAsync(string username, IFormFile file);
        Task<bool> SetMainPhotoAsync(string username, int photoId);
        Task<bool> DeletePhotoAsync(string username, int photoId);
    }
}