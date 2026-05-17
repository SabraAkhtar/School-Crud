using mera_school.Models;
using Microsoft.EntityFrameworkCore;

namespace mera_school.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for the School Management System.
    /// </summary>
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student → Attendance (one-to-many)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Teacher → Attendance (one-to-many)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Teacher)
                .WithMany(t => t.Attendances)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Teacher → Subject (one-to-many)
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1, FullName = "Ali Hassan", Gender = "Male", Age = 14,
                    Email = "ali@school.com", PhoneNumber = "03001234567",
                    Address = "123 Main Street, Peshawar", ClassName = "Grade 8",
                    AdmissionDate = new DateTime(2023, 4, 1)
                },
                new Student
                {
                    Id = 2, FullName = "Sara Khan", Gender = "Female", Age = 13,
                    Email = "sara@school.com", PhoneNumber = "03009876543",
                    Address = "45 Garden Road, Peshawar", ClassName = "Grade 7",
                    AdmissionDate = new DateTime(2023, 4, 1)
                }
            );

            modelBuilder.Entity<Teacher>().HasData(
                new Teacher
                {
                    Id = 1, FullName = "Mr. Peshawa", Subject = "Mathematics",
                    Qualification = "M.Sc Mathematics", Email = "peshawa@school.com",
                    PhoneNumber = "03111234567", Salary = 45000,
                    JoinDate = new DateTime(2020, 1, 15)
                },
                new Teacher
                {
                    Id = 2, FullName = "Ms. Nadia", Subject = "English",
                    Qualification = "M.A English Literature", Email = "nadia@school.com",
                    PhoneNumber = "03119876543", Salary = 42000,
                    JoinDate = new DateTime(2021, 3, 10)
                }
            );
        }
    }
}
