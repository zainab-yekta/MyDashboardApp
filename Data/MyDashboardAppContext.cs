using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyDashboardApp.Models;

namespace MyDashboardApp.Data;

public class MyDashboardAppContext : IdentityDbContext<IdentityUser>
{
    public MyDashboardAppContext(DbContextOptions<MyDashboardAppContext> options)
        : base(options)
    {
    }

    public DbSet<SalesData> SalesData { get; set; }
}
