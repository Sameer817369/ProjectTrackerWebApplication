using Application.ProTrack.Service;
using Domain.ProTrack.Interface;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<ITaskServiceInterface, TaskService>();

            return services;
        }
    }
}
