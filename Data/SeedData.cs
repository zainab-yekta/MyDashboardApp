using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyDashboardApp.Models;

namespace MyDashboardApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<MyDashboardAppContext>();
            var config = services.GetRequiredService<IConfiguration>();

            // Create the database and apply migrations
            await context.Database.MigrateAsync();

            await SeedRoles(services);

            // Demo accounts come from configuration, nothing is hardcoded
            await SeedUser(services, config["DemoAccounts:AdminEmail"], config["DemoAccounts:AdminPassword"], "Admin");
            await SeedUser(services, config["DemoAccounts:UserEmail"], config["DemoAccounts:UserPassword"], "User");

            await SeedSales(context);
        }

        private static async Task SeedRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUser(IServiceProvider services, string? email, string? password, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return;
            }

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // UserName must match the email, the login page signs in by email
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                await userManager.CreateAsync(user, password);
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }

        // Remove everything added by visitors and put the sample year back
        public static async Task ResetSalesAsync(MyDashboardAppContext context)
        {
            await context.SalesData.ExecuteDeleteAsync();
            await SeedSales(context);
        }

        private static async Task SeedSales(MyDashboardAppContext context)
        {
            if (await context.SalesData.AnyAsync())
            {
                return;
            }

            // One year of sample data so the chart is not empty on first run
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var sales = new[] { 4200, 3900, 5100, 4800, 5600, 6100, 5800, 6400, 7000, 6700, 7600, 8300 };

            for (int i = 0; i < months.Length; i++)
            {
                context.SalesData.Add(new SalesData { Month = months[i], Sales = sales[i] });
            }

            await context.SaveChangesAsync();
        }
    }
}
