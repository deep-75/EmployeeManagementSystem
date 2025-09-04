using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementSystem.Data.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, AppDbContext context)
        {
            // Add roles
            string[] roles = { "Admin", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default Admin user
            string adminEmail = "admin@ems.com";
            string adminPassword = "Admin@12345";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // ✅ Create default Employee user
            string empEmail = "employee@ems.com";
            string empPassword = "Employee@12345";

            var empUser = await userManager.FindByEmailAsync(empEmail);
            if (empUser == null)
            {
                var newEmployee = new AppUser
                {
                    UserName = empEmail,
                    Email = empEmail,
                    FullName = "Default Employee",
                    EmailConfirmed = true
                };

                // ✅ After creating or finding the demo employee
                var employeeUser = await userManager.FindByEmailAsync(empEmail);
                if (employeeUser == null)
                {
                    var Employee = new AppUser
                    {
                        UserName = empEmail,
                        Email = empEmail,
                        FullName = "Demo Employee",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newEmployee, empPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newEmployee, "Employee");
                        employeeUser = newEmployee; // assign created user
                    }
                }

               

            }
        }
    }
}
