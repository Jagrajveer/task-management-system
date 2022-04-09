using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using task_management_system.Data;

namespace task_management_system.Models
{
    public class SeedData
    {
        public async static System.Threading.Tasks.Task Initialize(IServiceProvider serviceProvider)
        {
            var _context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            List<string> roles = new List<string> {
                "Admin", "ProjectManager", "Developer", "User"
            };

            if (!_context.Roles.Any())
            {
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (!_context.Users.Any())
            {
                User AdminUser = new()
                {
                    UserName = "Admin@task-management.com",
                    Email = "Admin@task-management.com",
                    NormalizedEmail = "ADMIN@TASK-MANAGEMENT.COM",
                    NormalizedUserName = "ADMIN@TASK-MANAGEMENT.COM",
                    EmailConfirmed = true,
                    Salary = 7000,
                    PaymentDuration = User.PaymentDurationType.Monthly
                };

                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(AdminUser, "P@ssword!12");
                AdminUser.PasswordHash = hashed;

                User jagraj = new()
                {
                    UserName = "Jagraj@task-management.com",
                    Email = "Jagraj@task-management.com",
                    NormalizedEmail = "JAGRAJ@TASK-MANAGEMENT.COM",
                    NormalizedUserName = "JAGRAJ@TASK-MANAGEMENT.COM",
                    EmailConfirmed = true,
                    Salary = 6000,
                    PaymentDuration = User.PaymentDurationType.Monthly
                };

                var jagrajHashed = password.HashPassword(jagraj, "P@ssword!12");
                jagraj.PasswordHash = jagrajHashed;

                User sahil = new()
                {
                    UserName = "Sahil@task-management.com",
                    Email = "Sahil@task-management.com",
                    NormalizedEmail = "SAHIL@TASK-MANAGEMENT.COM",
                    NormalizedUserName = "SAHIL@TASK-MANAGEMENT.COM",
                    EmailConfirmed = true,
                    Salary = 4000,
                    PaymentDuration = User.PaymentDurationType.Monthly
                };

                var sahilHashed = password.HashPassword(sahil, "P@ssword!12");
                sahil.PasswordHash = sahilHashed;

                User prateek = new()
                {
                    UserName = "Prateek@task-management.com",
                    Email = "Prateek@task-management.com",
                    NormalizedEmail = "PRATEEK@TASK-MANAGEMENT.COM",
                    NormalizedUserName = "PRATEEK@TASK-MANAGEMENT.COM",
                    EmailConfirmed = true,
                    Salary = 1000,
                    PaymentDuration = User.PaymentDurationType.BiWeekly
                };

                var prateekHashed = password.HashPassword(prateek, "P@ssword!12");
                prateek.PasswordHash = prateekHashed;

                User taimoor = new()
                {
                    UserName = "Taimoor@task-management.com",
                    Email = "Taimoor@task-management.com",
                    NormalizedEmail = "TAIMOOR@TASK-MANAGEMENT.COM",
                    NormalizedUserName = "TAIMOOR@TASK-MANAGEMENT.COM",
                    EmailConfirmed = true,
                    Salary = 10000,
                    PaymentDuration = User.PaymentDurationType.Contract
                };

                var taimoorHashed = password.HashPassword(taimoor, "P@ssword!12");
                taimoor.PasswordHash = taimoorHashed;


                await _userManager.CreateAsync(AdminUser);
                await _userManager.CreateAsync(jagraj);
                await _userManager.CreateAsync(sahil);
                await _userManager.CreateAsync(prateek);
                await _userManager.CreateAsync(taimoor);

                await _userManager.AddToRoleAsync(AdminUser, "Admin");
                await _userManager.AddToRoleAsync(jagraj, "ProjectManager");
                await _userManager.AddToRoleAsync(sahil, "Developer");
                await _userManager.AddToRoleAsync(prateek, "Developer");
                await _userManager.AddToRoleAsync(taimoor, "Developer");
            }

            await _context.SaveChangesAsync();
        }
    }
}
