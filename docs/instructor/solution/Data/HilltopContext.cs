using HilltopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HilltopApi.Data;

public class HilltopContext(DbContextOptions<HilltopContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasData(SeedData.Students);

        modelBuilder.Entity<Enrollment>().HasKey(e => new { e.StudentId, e.CourseId });

        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Code = "NURS 1010", Title = "Foundations of Nursing", School = "Nursing" },
            new Course { Id = 2, Code = "MEDI 2200", Title = "Human Anatomy", School = "Medicine" },
            new Course { Id = 3, Code = "BUSN 1500", Title = "Intro to Accounting", School = "Business" },
            new Course { Id = 4, Code = "MINE 3100", Title = "Mine Safety", School = "Mining" });

        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment { StudentId = 1, CourseId = 1 },
            new Enrollment { StudentId = 7, CourseId = 1 },
            new Enrollment { StudentId = 4, CourseId = 2 },
            new Enrollment { StudentId = 6, CourseId = 2 },
            new Enrollment { StudentId = 10, CourseId = 2 },
            new Enrollment { StudentId = 4, CourseId = 3 },
            new Enrollment { StudentId = 5, CourseId = 3 },
            new Enrollment { StudentId = 8, CourseId = 3 },
            new Enrollment { StudentId = 2, CourseId = 4 });
    }
}
