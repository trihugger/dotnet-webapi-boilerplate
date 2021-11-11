using DN.WebApi.Application.Extensions;
using DN.WebApi.Infrastructure.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DN.WebApi.Bootstrapper
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddFluentValidation();
            services
                .AddApplication()
                .AddInfrastructure(_config);
            services.AddAntiforgery();  // see more on http://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore.6.0

                                        // add [AutoValidateAntiForgeryToken] tag to the controller and can be disabled with [IgnoreAntiforgeryToken] tag as needed
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHttpsRedirection(); // Add to automatically redirect to https. Note: this can also be done in the IIS config as well.
            }

            app.UseInfrastructure(_config);
        }
    }
}