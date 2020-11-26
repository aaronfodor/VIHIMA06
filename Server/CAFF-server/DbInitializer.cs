using CAFF_server.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server
{
    public static class DbInitializer
    {
        public static async Task Initialize(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            if(!roleManager.Roles.Any())
            {
                var roles = new IdentityRole[]
                {
                    new IdentityRole{ Name = Role.ADMIN},
                    new IdentityRole{ Name = Role.API_USER},
                };
                foreach (IdentityRole r in roles)
                {
                    await roleManager.CreateAsync(r);
                }
            }

            if (!userManager.Users.Any())
            {
                var user = new User { UserName = "user" };
                await userManager.CreateAsync(user, "default");
                await userManager.AddToRoleAsync(user, Role.API_USER);

                var admin = new Administrator { UserName = "admin" };
                await userManager.CreateAsync(admin, "default");
                await userManager.AddToRoleAsync(user, Role.API_USER);
                await userManager.AddToRoleAsync(admin, Role.ADMIN);
            }
        }
    }
}
