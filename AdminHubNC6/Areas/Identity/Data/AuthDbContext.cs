using AdminHubNC6.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminHubNC6.Data;

public class AuthDbContext : IdentityDbContext<AdminHubUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {        
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        /* [START] Seed Admin Role and admin user for system initial stage*/
        string ADMIN_ID = "02189cf0-9412-4cfe-afbf-59f706d72cf6";
        string ROLE_ID =  "341743f0-asd2-72de-afbf-89kckmk72cf6";

        //seed admin role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = "Admin",
            NormalizedName = "ADMIN",
            Id = ROLE_ID,
            ConcurrencyStamp = ROLE_ID
        });

        //create user
        var appUser = new AdminHubUser
        {
            Id = ADMIN_ID,
            Email = "admin@admin.com",
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Admin",
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM"
        };

        //set user password
        PasswordHasher<AdminHubUser> ph = new PasswordHasher<AdminHubUser>();
        appUser.PasswordHash = ph.HashPassword(appUser, "p@ssw0rd");

        //seed user
        builder.Entity<AdminHubUser>().HasData(appUser);

        //set user role to admin
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ROLE_ID,
            UserId = ADMIN_ID
        });

        /* [END] Seed Admin Role and admin user for system initial stage*/

        base.OnModelCreating(builder);
    }
}
