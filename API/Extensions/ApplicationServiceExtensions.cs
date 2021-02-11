using API.Data;
using AutoMapper;
using Data;
using Data.Repository;
using Data.Repository.Interface;
using Filters;
using Helpers;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Interface;

namespace Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikeRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesService, LikesService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}