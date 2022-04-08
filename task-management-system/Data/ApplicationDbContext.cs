using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using task_management_system.Models;

namespace task_management_system.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<task_management_system.Models.Task> Tasks { get; set; }

        public DbSet<Notification > Notifications { get; set; }

        public DbSet<ProjectDeveloper> ProjectDevelopers { get; set; }

        public DbSet<Note> Notes { get; set; }

    }
}