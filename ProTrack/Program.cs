using Application.ProTrack.Service;
using Domain.ProTrack.Interface;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Hangfire;
using Infrastructure.ProTrack.Data;
using Infrastructure.ProTrack.Data.SeedRoles;
using Infrastructure.ProTrack.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;


using DotNetEnv;
using Application.ProTrack.Dependency_Injection;
using Infrastructure.ProTrack.Dependency_Injection;

var builder = WebApplication.CreateBuilder(args);
Env.Load(); // Loads from .env automatically
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

Log.Information("Getting the motors running...");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Web Api",
        Version = "v1"
    });

    option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
             new OpenApiSecurityScheme
             {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id =  "Bearer"
                }
             },[]
        }
    });
});
var dbConnection = Environment.GetEnvironmentVariable("DBCONNECTIONS");
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(dbConnection));

builder.Services.AddIdentity<AppUser, IdentityRole>(option =>
{
    option.SignIn.RequireConfirmedEmail = true;
    option.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//Repositories
builder.Services.InfrastructureDI();
//Services
builder.Services.ApplicationDI();

builder.Services.AddHangfire(options => options.UseSqlServerStorage(dbConnection));
builder.Services.AddHangfireServer();

var app = builder.Build();
using(var scope= app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await SeedRolesService.SeedRoleAsync(roleManager);
    await SeedRolesService.SeedAdminAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/jobs");
app.Run();
