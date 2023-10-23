using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {   
        }

        //3 roles SuperAdmin, Admin, User (Model create krne ke liye class ye wali use hoti hai)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Seed Roles (SuperAdmin, Admin, User)

            var adminRoleId = "a8d42092-c0a3-4b17-bfb3-7d087abb8436";
            var superAdminRoleId = "515ba0c4-eb7c-44e0-acf8-5ed8d1a9f561";
            var userRoleId = "b69b1b83-e711-494f-a51a-b1fd6891b0dd";

            //3 roles hain isliye unko List mai krdia hai
            var roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SuperAdmin",
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "User",
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId
                }
            };

            //When it runs the asp net core will insert the roles inside the database
            builder.Entity<IdentityRole>().HasData(roles);

            //Seed SuperAdminUser (Roles mai user kese ayenge) Single user create kra hai

            var superAdminId = "458696f0-cbf0-4b7a-98ce-5565dc1bcdda";

            var superAdminUser = new IdentityUser()
            {
                UserName = "superadmin@bloggie.com",
                Email = "superadmin@bloggie.com",
                NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
                NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
                Id = superAdminId
            };

            //Creating a password for the admin user (Cryptography).
            //Created a Salted and hashed Password.

            superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdminUser, "Superadmin@123");

            //When it runs the asp net core will insert the roles inside the database
            builder.Entity<IdentityUser>().HasData(superAdminUser);

            //Add all the roles to SuperAdminUser. Ab jitne bhi roles hain na woh saray superadminuser ko dedein
            //Kyun ke superadmin sab kuch krskta hai perform baki ke paas itna access nhi rhega

            // 3 roles are given to created user with the help of class IdentityUserRole helps us to give
            // Role to the user

            var superAdminRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>()
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminUser.Id
                },
                new IdentityUserRole<string> 
                {
                    RoleId = adminRoleId,
                    UserId = superAdminUser.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = superAdminUser.Id
                },
            };

            //When it runs the asp net core will insert the roles inside the database
            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
        }
    }
}
