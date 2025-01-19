using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyDashboardApp.Areas.Identity.Data;
using Microsoft.AspNetCore.SignalR;
using MyDashboardApp.Hubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyDashboardApp.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or empty.");
}

builder.Services.AddDbContext<MyDashboardAppContext>(options =>
    options.UseSqlServer(connectionString));

//Configure a Real Email Sender
//builder.Services.AddTransient<IEmailSender>(provider =>
//    new SendGridEmailSender("Your_SendGrid_API_Key"));

// Add Identity Services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{ 
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<MyDashboardAppContext>()    
.AddDefaultTokenProviders();

// Add SignalR service
builder.Services.AddSignalR();

//register the DummyEmailSender
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Controllers with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed roles into the database
async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = role,
                NormalizedName = role.ToUpper()
            });
        }
    }
}

// Seed users and assign them roles
async Task SeedUsersWithRoles(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Access credentials from appsettings.json
    var adminEmail = builder.Configuration["AdminEmail"] ?? "admin@example.com";
    var adminPassword = builder.Configuration["AdminPassword"] ?? "Admin@123";

    // Create an admin user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin@123"); // Strong password
    }

    // Assign Admin role
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Create a regular user
    var userEmail = builder.Configuration["UserEmail"] ?? "user@example.com";
    var userPassword = builder.Configuration["UserPassword"] ?? "User@123";

    var regularUser = await userManager.FindByEmailAsync(userEmail);

    if (regularUser == null)
    {
        regularUser = new IdentityUser
        {
            UserName = "user",
            Email = userEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(regularUser, "User@123"); // Strong password
    }

    // Assign User role
    if (!await userManager.IsInRoleAsync(regularUser, "User"))
    {
        await userManager.AddToRoleAsync(regularUser, "User");
    }
}

// Seed roles and users
var serviceProvider = app.Services.CreateScope().ServiceProvider;
try
{
    await SeedRoles(serviceProvider);
    await SeedUsersWithRoles(serviceProvider);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while seeding roles and users: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChartHub>("/chartHub"); // Map SignalR hub
app.MapRazorPages(); // For Identity pages
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
