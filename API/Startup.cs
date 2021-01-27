
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Extensions;
using Filters;
using System.Text.Json;
using System.Linq;
using Serilog;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);
            services.AddCors();
            services
                .AddControllers(config => {
                    config.Filters.Add(new ExceptionFilter());
                })
                .AddJsonOptions(option => {
                    option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            services.AddIdentityServices(_config);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(X => X.AllowAnyHeader().AllowAnyMethod().WithOrigins(_config
                .GetSection("Origins")
                .GetChildren()
                .Select(x => x.Value)
                .ToArray()
            ));

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
