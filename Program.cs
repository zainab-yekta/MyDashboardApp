using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDashboardApp.Data;
using MyDashboardApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Private settings such as the admin account, ignored by git
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or empty.");
}

builder.Services.AddDbContext<MyDashboardAppContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity with roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = false; // No email service in this demo
})
.AddEntityFrameworkStores<MyDashboardAppContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Add SignalR service
builder.Services.AddSignalR();

// Daily reset of the sample data, turned on for the public demo only
if (builder.Configuration.GetValue<bool>("DemoData:ResetDaily"))
{
    builder.Services.AddHostedService<DemoResetService>();
}

// Check the antiforgery token on every POST
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

// Add Razor Pages for the Identity UI
builder.Services.AddRazorPages();

var app = builder.Build();

// Apply migrations and seed roles, demo users and sample sales
using (var scope = app.Services.CreateScope())
{
    try
    {
        await SeedData.InitializeAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while seeding the database.");
    }
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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
