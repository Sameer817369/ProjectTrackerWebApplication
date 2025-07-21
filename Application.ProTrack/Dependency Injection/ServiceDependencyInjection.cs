using Application.ProTrack.Service;
using Application.ProTrack.Service.Interface;
using Microsoft.AspNetCore.Http;
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
            //email triggering services
            services.AddTransient<ICustomeEmailServiceInterface, CustomeEmailService>();
            services.AddTransient<IEmailServiceInterface, EmailSenderService>();
            services.AddScoped<IEmailDispatcherServiceInterface, EmailDispatcherService>();
            services.AddScoped<IProjectEmailNotificationHelperInterface, ProjectEmailNotificationHelperService>();
            //project service
            services.AddScoped<IProjectServiceInterface, ProjectService>();
            services.AddScoped<IProjectHelperService, ProjectHelperService>();
            //task service
            services.AddScoped<ITaskServiceInterface, TaskService>();
            //comment service
            services.AddScoped<ICommentServiceInterface, CommentService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IHangeFrieJobsServiceInterface, HangeFireJobService>();
            return services;
        }
    }
}
