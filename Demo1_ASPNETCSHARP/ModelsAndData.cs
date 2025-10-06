using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TaskTrackerPro
{
    public enum TaskStatus
    {
        New = 0,
        InProgress = 1,
        Blocked = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(120)]
        public string Name { get; set; } = default!;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = default!;

        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }

    public class Project
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(160)]
        public string Name { get; set; } = default!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public Guid? OwnerId { get; set; }
        public User? Owner { get; set; }

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(160)]
        public string Title { get; set; } = default!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateOnly? DueDate { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.New;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required]
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }

        public Guid? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public static class DemoData
    {
        public static async Task SeedAsync(AppDbContext ctx)
        {
            if (await ctx.Users.AnyAsync()) return;

            var alice = new User { Name = "Alice Ahmed", Email = "alice@example.com" };
            var ben = new User { Name = "Ben Brown", Email = "ben@example.com" };
            var chloe = new User { Name = "Chloe Chen", Email = "chloe@example.com" };

            var proj1 = new Project
            {
                Name = "Customer Portal",
                Description = "Build a secure customer portal MVP.",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-21)),
                Owner = alice
            };
            var proj2 = new Project
            {
                Name = "Mobile App Rewrite",
                Description = "Rewrite legacy Xamarin app with MAUI.",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-35)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
                Owner = ben
            };

            var tasks = new List<TaskItem>
            {
                new TaskItem {
                    Title = "Design auth flow",
                    Description = "Draft user journeys and states",
                    Project = proj1,
                    Priority = TaskPriority.High,
                    Status = TaskStatus.InProgress,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
                    AssignedTo = alice
                },
                new TaskItem {
                    Title = "Implement login API",
                    Project = proj1,
                    Priority = TaskPriority.Critical,
                    Status = TaskStatus.Blocked,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
                    AssignedTo = ben
                },
                new TaskItem {
                    Title = "Telemetry baseline",
                    Project = proj1,
                    Priority = TaskPriority.Medium,
                    Status = TaskStatus.Completed,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-2)),
                    AssignedTo = chloe
                },
                new TaskItem {
                    Title = "Create MAUI shell",
                    Project = proj2,
                    Priority = TaskPriority.Medium,
                    Status = TaskStatus.New,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10)),
                    AssignedTo = chloe
                },
                new TaskItem {
                    Title = "Migrate settings store",
                    Project = proj2,
                    Priority = TaskPriority.High,
                    Status = TaskStatus.InProgress,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1)), // overdue unless completed
                    AssignedTo = ben
                }
            };

            await ctx.Users.AddRangeAsync(alice, ben, chloe);
            await ctx.Projects.AddRangeAsync(proj1, proj2);
            await ctx.Tasks.AddRangeAsync(tasks);
            await ctx.SaveChangesAsync();
        }
    }
}
