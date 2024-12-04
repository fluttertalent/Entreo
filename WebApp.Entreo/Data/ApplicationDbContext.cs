using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApp.Entreo.Models;
using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitLog> HabitLogs { get; set; }
        public DbSet<HabitReminder> HabitReminders { get; set; }
        public DbSet<HabitResource> HabitResources { get; set; }
        public DbSet<HabitStreak> HabitStreaks { get; set; }
        public DbSet<AccountabilityPartner> AccountabilityPartners { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        // Notification-related DbSets
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationSchedule> NotificationSchedules { get; set; }
        public DbSet<NotificationDelivery> NotificationDeliveries { get; set; }
        public DbSet<UserNotificationPreference> UserNotificationPreferences { get; set; }
        public DbSet<NotificationCategoryPreference> NotificationCategoryPreferences { get; set; }
        public DbSet<PushNotificationToken> PushNotificationTokens { get; set; }

        public DbSet<Translation> Translations { get; set; }
        public DbSet<PostalCode> PostalCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Habit relationship
            modelBuilder.Entity<Habit>()
                .HasOne(h => h.User)
                .WithMany(u => u.Habits)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // User - HabitLog relationship
            modelBuilder.Entity<HabitLog>()
                .HasOne(l => l.User)
                .WithMany(u => u.HabitLogs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // User - HabitReminder relationship
            modelBuilder.Entity<HabitReminder>()
                .HasOne(r => r.User)
                .WithMany(u => u.HabitReminders)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // User - UserNotificationPreference relationship
            modelBuilder.Entity<UserNotificationPreference>()
                .HasOne(p => p.User)
                .WithMany(u => u.NotificationPreferences)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Habit - HabitLog relationship
            modelBuilder.Entity<HabitLog>()
                .HasOne(l => l.Habit)
                .WithMany()
                .HasForeignKey(l => l.HabitId)
                .OnDelete(DeleteBehavior.NoAction);

            // HabitReminder - NotificationTemplate relationship
            modelBuilder.Entity<HabitReminder>()
                .HasOne(r => r.NotificationTemplate)
                .WithMany()
                .HasForeignKey(r => r.NotificationTemplateId)
                .OnDelete(DeleteBehavior.NoAction);

            // HabitCompletion relationships
            modelBuilder.Entity<HabitCompletion>()
                .HasOne(c => c.Habit)
                .WithMany()
                .HasForeignKey(c => c.HabitId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HabitReminder>()
                .HasIndex(r => new { r.HabitId, r.UserId })
                .IsUnique();

            // Configure many-to-many relationship between Habit and Tag
            modelBuilder.Entity<Habit>()
                .HasMany(h => h.Tags)
                .WithMany(t => t.Habits)
                .UsingEntity(j => j.ToTable("HabitTags"));
        }

        public static void InitializeDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (!AllMigrationsApplied(context))
            {
                context.Database.Migrate();
            }
        }

        public static bool AllMigrationsApplied(DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}