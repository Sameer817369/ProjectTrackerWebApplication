using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.ProTrack.Data.SeedRoles
{
    public class SeedRolesService
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string>
            {
                "Admin",
                "Employee",
            };
            foreach(var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    IdentityRole identityRole = new IdentityRole(role)
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };
                    await roleManager.CreateAsync(identityRole);
                }
            }
        }
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            string adminEmail = "admin@gmail.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    // Custom fields
                    FirstName = "System",
                    LastName = "Administrator",
                    PhoneNumber = "9765422698",
                    Age = 30,
                    City = "Kathmandu",
                    Street = "NayaBazar"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
        }
    }
}
