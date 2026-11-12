using HilltopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HilltopApi.Data;

public class HilltopContext(DbContextOptions<HilltopContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasData(SeedData.Students);
    }
}
