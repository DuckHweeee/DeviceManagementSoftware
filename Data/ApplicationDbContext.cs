using Microsoft.EntityFrameworkCore;
using DeviceManagementSoftware.Models;

namespace DeviceManagementSoftware.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DeviceCategory> DeviceCategories { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Device entity
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasConversion<int>();
                
                entity.HasOne(d => d.Category)
                      .WithMany(c => c.Devices)
                      .HasForeignKey(d => d.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure UserDevice relationship
            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.HasOne(ud => ud.User)
                      .WithMany(u => u.UserDevices)
                      .HasForeignKey(ud => ud.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ud => ud.Device)
                      .WithMany(d => d.UserDevices)
                      .HasForeignKey(ud => ud.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Device Categories
            modelBuilder.Entity<DeviceCategory>().HasData(
                new DeviceCategory { Id = 1, Name = "Computer", Description = "Desktop computers and laptops", DateCreated = DateTime.Now },
                new DeviceCategory { Id = 2, Name = "Printer", Description = "Printing devices", DateCreated = DateTime.Now },
                new DeviceCategory { Id = 3, Name = "Phone", Description = "Mobile phones and desk phones", DateCreated = DateTime.Now },
                new DeviceCategory { Id = 4, Name = "Monitor", Description = "Display monitors", DateCreated = DateTime.Now },
                new DeviceCategory { Id = 5, Name = "Network Equipment", Description = "Routers, switches, and network devices", DateCreated = DateTime.Now }
            );

            // Seed Sample Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "John Smith", Email = "john.smith@company.com", PhoneNumber = "123-456-7890", Department = "IT", Position = "System Administrator", DateCreated = DateTime.Now },
                new User { Id = 2, FullName = "Sarah Johnson", Email = "sarah.johnson@company.com", PhoneNumber = "123-456-7891", Department = "HR", Position = "HR Manager", DateCreated = DateTime.Now },
                new User { Id = 3, FullName = "Mike Brown", Email = "mike.brown@company.com", PhoneNumber = "123-456-7892", Department = "Finance", Position = "Financial Analyst", DateCreated = DateTime.Now }
            );
        }
    }
}
