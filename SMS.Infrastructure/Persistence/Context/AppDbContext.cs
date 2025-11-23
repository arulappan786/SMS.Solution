using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMS.Application.Services.Interfaces.Context;
using SMS.Domain.Entities.Attendance;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Grading;
using SMS.Domain.Entities.Identity;

namespace SMS.Infrastructure.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // --- DbSet Properties (Tables) ---
        // These fulfill the contract defined in IAppDbContext
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }

        // IdentityDbContext already provides a DbSet for AppUser, 
        // but we explicitly define the property to satisfy the IAppDbContext contract.
        //public DbSet<AppUser> AppUsers => base.Users;

        // --- Unit of Work Implementation (Explicit Interface) ---

        // The Application layer calls this method via the IAppDbContext interface.
        async Task<int> IAppDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            // Add any common logging or auditing logic here before saving
            return await base.SaveChangesAsync(cancellationToken);
        }

        // --- Model Configuration (As previously discussed) ---

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: Call the base method first for Identity tables setup.
            base.OnModelCreating(modelBuilder);

            // 💡 FIX 1: Configure Address as an Owned Entity for Student
            modelBuilder.Entity<Student>().OwnsOne(s => s.HomeAddress, address =>
            {
                // Optional: Rename columns in the Student table to avoid collisions (e.g., if Student also had an OfficeAddress)
                // address.Property(a => a.Street).HasColumnName("HomeStreet"); 
            });

            // 💡 FIX 1: Configure Address as an Owned Entity for Student
            modelBuilder.Entity<Student>().OwnsOne(s => s.FullName, fullname =>
            {
                // Optional: Rename columns in the Student table to avoid collisions (e.g., if Student also had an OfficeAddress)
                // address.Property(a => a.Street).HasColumnName("HomeStreet"); 
            });

            // 1. **EXPLICIT FIX for Grade (The one mentioned in the error)**
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // <-- Explicitly restricted

            // 2. **EXPLICIT FIX for StudentParent (The likely second path)**
            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // <-- Explicitly restricted

            // 3. Ensure the Parent side is also restricted
            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. **EXPLICIT FIX for Attendance** (A probable third path)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);


            // 5. Global Fallback (Leave this in as a safety net)
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Your custom entity configuration goes here...
            // Configure composite uniqueness (optional but recommended for a join table)
            modelBuilder.Entity<StudentParent>()
                .HasIndex(sp => new { sp.StudentId, sp.ParentId })
                .IsUnique();

            // Configuration for the link from Student to StudentParent:
            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student) // StudentParent has ONE Student
                .WithMany(s => s.StudentParents) // Student has MANY StudentParents
                .HasForeignKey(sp => sp.StudentId);

            // Configuration for the link from Parent to StudentParent:
            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent) // StudentParent has ONE Parent
                .WithMany(p => p.StudentParents) // Parent also needs the reciprocal collection!
                .HasForeignKey(sp => sp.ParentId);


            // --- Fix for WeightPercentage on Assignment ---
            modelBuilder.Entity<Assignment>()
                // HasPrecision(total_digits, after_decimal_point)
                .Property(a => a.WeightPercentage)
                .HasPrecision(5, 2); // Example: Allows values up to 999.99

            // --- Fix for MarksObtained on Grade ---
            modelBuilder.Entity<Grade>()
                .Property(g => g.MarksObtained)
                .HasPrecision(8, 4); // Example: Allows values up to 9999.9999
        }
    }
}