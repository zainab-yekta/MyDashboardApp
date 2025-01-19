using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MyDashboardApp.Areas.Identity.Data;

public class MyDashboardAppContext : IdentityDbContext<IdentityUser>
{
    public MyDashboardAppContext(DbContextOptions<MyDashboardAppContext> options)
        : base(options)
    {
    }

    // Add custom DbSet for the SalesData entity
    public DbSet<SalesData> SalesData { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Add additional configuration for SalesData
        builder.Entity<SalesData>(entity =>
        {
            // Example: Configure primary key or other constraints
            entity.HasKey(e => e.Id); // Assuming SalesData has a property named Id
        });

        //Use Fluent API for Identity Configuration
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.Property(t => t.LoginProvider).HasMaxLength(128).IsRequired();
            entity.Property(t => t.Name).HasMaxLength(128).IsRequired();
        });
    }
}
// Example SalesData entity
public class SalesData
{
    public int Id { get; set; } // Primary key
    public string? Month { get; set; }
    public int Sales { get; set; }
}
