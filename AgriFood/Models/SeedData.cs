using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AgriFood.Constants;

namespace AgriFood.Models
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            string farmer_role = Roles.FarmerRole;
            string customer_role = Roles.CustomerRole;

            await CheckRoleManager(roleManager, farmer_role);
            await CheckRoleManager(roleManager, customer_role);

            if (await userManager.FindByNameAsync("farmer1@user.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "farmer1@user.com",
                    Email = "farmer1@user.com",
                    PhoneNumber = "6902341234",
                    FarmName = "Yeşilova Çifliği",
                    Description = "Yeşilova Çiftliği'nde organik meyve ve sebzeler bulunmaktadır",
                    Latitude = 37.4889554838666,
                    Longitude = 29.5550897785416
                };

                await CreateUser(userManager, context, user, farmer_role);
            }

            if (await userManager.FindByNameAsync("farmer2@user.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "farmer2@user.com",
                    Email = "farmer2@user.com",
                    PhoneNumber = "7788951456",
                    FarmName = "Sarı Koyun Çiftliği",
                    Description = "Sarı Koyun Çiftliği'nde günlük süt ve peynir ürünleri üretilmektedir",
                    Latitude = 40.8306330595443,
                    Longitude = 41.0832875469796
                };

                await CreateUser(userManager, context, user, farmer_role);
            }

            if (await userManager.FindByNameAsync("farmer3@user.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "farmer3@user.com",
                    Email = "farmer3@user.com",
                    PhoneNumber = "5657242367",
                    FarmName = "Harman Çiftliği",
                    Description = "Çiftliğimizin ürünlerinin lezzeti dünyaca ünlüdür",
                    Latitude = 38.8788229999926,
                    Longitude = 35.7022687560811
                };

                await CreateUser(userManager, context, user, farmer_role);
            }

            if (await userManager.FindByNameAsync("customer@user.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "customer@user.com",
                    Email = "customer@user.com",
                    PhoneNumber = "6572136821"
                };

                await CreateUser(userManager, context, user, customer_role);
            }

            context.SaveChanges();
        }

        private static async Task CheckRoleManager(RoleManager<IdentityRole>  roleManager, string role)
        {
            if (await roleManager.FindByNameAsync(role) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task CreateUser(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ApplicationUser user, string role)
        {
            string password = "12*-qweR";

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await userManager.AddPasswordAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
