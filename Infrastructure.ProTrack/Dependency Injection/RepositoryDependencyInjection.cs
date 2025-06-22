using Domain.ProTrack.Interface.RepoInterface;
using Infrastructure.ProTrack.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ProTrack.Dependency_Injection
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection InfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<IUserRepoInterface, UserRepository>();
            services.AddScoped<IEmailRepoInterface, EmailRepository>();
            services.AddScoped<IProjectRepoInterface, ProjectRepository>();
            services.AddScoped<ITaskRepositoryInterface, TaskRepository>();
            return services;
        }
    }
}
