using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Entities;
using Helpers;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Services
{
  public class PhotoService : IPhotoService
  {
    private readonly Cloudinary _cloudinary;
    private readonly IUserRepository _userRepository;

    public PhotoService(IOptions<CloudinarySettings> config, IUserRepository userRepository)
    {
        var acc = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
		_userRepository = userRepository;
    }

    public async Task<Photo> AddPhotoAsync(ImageUploadResult result, AppUser user)
    {
		var photo = new Photo
		{
			Url = result.SecureUrl.AbsoluteUri,
			PublicId = result.PublicId
		};

		if(user.Photos.Count == 0)
		{
			photo.IsMain = true;
		}

		user.Photos.Add(photo);

		return await _userRepository.SaveAllAsync() ? photo : null;
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

    public async Task<bool> SetMainPhotoAsync(string username, int photoId)
    {
      var user = await _userRepository.GetUserByUsernameAsync(username);

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo.IsMain) return true;

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
      if (currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;

      if (await _userRepository.SaveAllAsync()) return true;

      return false;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
      var deleteParams = new DeletionParams(publicId);

      var result = await _cloudinary.DestroyAsync(deleteParams);

      return result;
    }
  }
}