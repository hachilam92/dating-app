using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Services.Interface;
using System.Threading.Tasks;

namespace Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(ICloudinaryService cloudinaryService)
        {
            _cloudinary = cloudinaryService.CreateCloudinary();
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}