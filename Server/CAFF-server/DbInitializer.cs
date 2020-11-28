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
                    new IdentityRole{ Name = "Admin"},
                    new IdentityRole{ Name = "User"},
                };
                foreach (IdentityRole r in roles)
                {
                    await roleManager.CreateAsync(r);
                }
            }

            if (!userManager.Users.Any())
            {
                var user = new User { UserName = "user", Email = "user@webshop.ws", Name = "user" };
                await userManager.CreateAsync(user, "default");
                await userManager.AddToRoleAsync(user, "User");

                var admin = new Administrator { UserName = "admin", Email="admin@webshop.ws", Name="admin" };
                await userManager.CreateAsync(admin, "default");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
