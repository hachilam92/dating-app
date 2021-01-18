using CloudinaryDotNet;
using Helpers;
using Microsoft.Extensions.Options;
using Services.Interface;

namespace Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly string _cloudName;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            _cloudName = config.Value.CloudName;
            _apiKey = config.Value.ApiKey;
            _apiSecret = config.Value.ApiSecret;
        }

        public Cloudinary CreateCloudinary()
        {
            var acc = new Account
            (
                _cloudName,
                _apiKey,
                _apiSecret
            );

            return new Cloudinary(acc);
        }
    }
}