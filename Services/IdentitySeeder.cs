using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Services
{
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration configuration,
            ILogger logger)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
            var db = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            await db.Database.MigrateAsync();

            foreach (var role in new[] { AdminRole, UserRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    EnsureSucceeded(result, $"Không thể khởi tạo vai trò {role}");
                }
            }

            var adminEmail = configuration["Identity:BootstrapAdmin:Email"];
            var adminPassword = configuration["Identity:BootstrapAdmin:Password"];
            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning(
                    "Chưa khởi tạo tài khoản quản trị mặc định. " +
                    "Cấu hình Identity:BootstrapAdmin:Email và Password bằng biến môi trường.");
                return;
            }

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(admin, adminPassword);
                EnsureSucceeded(createResult, "Không thể khởi tạo tài khoản quản trị");
            }
            else if (!admin.EmailConfirmed)
            {
                admin.EmailConfirmed = true;
                var updateResult = await userManager.UpdateAsync(admin);
                EnsureSucceeded(updateResult, "Không thể xác nhận email tài khoản quản trị");
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                var roleResult = await userManager.AddToRoleAsync(admin, AdminRole);
                EnsureSucceeded(roleResult, "Không thể gán vai trò quản trị");
            }
        }

        private static void EnsureSucceeded(
            IdentityResult result,
            string operation)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"{operation}: {errors}");
            }
        }
    }
}
