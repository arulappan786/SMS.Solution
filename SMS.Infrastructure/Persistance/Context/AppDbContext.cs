using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.Attendance;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Finance;
using SMS.Domain.Entities.Grading;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Entities.Scheduling;

namespace SMS.Infrastructure.Persistance.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) 
        : IdentityDbContext<AppUser, IdentityRole, string>(options)
    {
        // Academic
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Class> Classs { get; set; }
        public DbSet<ClassSubject> ClassSubjects { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        // Attendance
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<DisciplinaryAction> DisciplinaryActions { get; set; }

        // Core
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }

        // Finance
        public DbSet<FeeInvoice> FeeInvoices { get; set; }
        public DbSet<FeeType> FeeTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // Grading
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Grade> Grades { get; set; }

        // Scheduling
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<HolidayOrEvent> HolidayOrEvents { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().OwnsOne(s => s.HomeAddress, address =>
            {
                address.Property(a => a.Street).HasColumnName("Home_Street");
                address.Property(a => a.City).HasColumnName("Home_City");
                address.Property(a => a.State).HasColumnName("Home_State");
                address.Property(a => a.ZipCode).HasColumnName("Home_ZipCode");
                address.Property(a => a.Country).HasColumnName("Home_Country");

            });

            modelBuilder.Entity<Student>().OwnsOne(s => s.FullName, fullname =>
            {
                fullname.Property(fn => fn.FirstName).HasColumnName("FullName_FirstName");
                fullname.Property(fn => fn.LastName).HasColumnName("FullName_LastName");
            });

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<StudentParent>()
                .HasIndex(sp => new { sp.StudentId, sp.ParentId })
                .IsUnique();

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId);

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId);

            modelBuilder.Entity<Assignment>()
                .Property(a => a.WeightPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Grade>()
                .Property(g => g.MarksObtained)
                .HasPrecision(8, 4);
        }
    }
}