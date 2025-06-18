using Application.ProTrack.Service;
using Domain.ProTrack.Interface;
using Domain.ProTrack.Interface.RepoInterface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProTrack.Dependency_Injection
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection  ApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<IUserServiceInterface, UserService>();
            services.AddScoped<ITokenGeneratorServiceInterface, TokenGeneratiorService>();
            services.AddScoped<IAuthServiceInterface, AuthService>();
            services.AddScoped<ICustomeEmailServiceInterface, CustomeEmailService>();
            services.AddScoped<IEmailServiceInterface, EmailSenderService>();
            services.AddScoped<IProjectServiceInterface, ProjectService>();

            return services;
        }
    }
}
