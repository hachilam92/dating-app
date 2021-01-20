using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Entities;
using Microsoft.AspNetCore.Http;

namespace Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
        Task<Photo> AddPhotoAsync(ImageUploadResult result, AppUser user);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}