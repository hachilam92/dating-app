using CloudinaryDotNet;

namespace Services.Interface
{
    public interface ICloudinaryService
    {
        Cloudinary CreateCloudinary();
    }
}