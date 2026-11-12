using HilltopApi.Data;
using HilltopApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("Hilltop") ?? "Data Source=hilltop.db";
builder.Services.AddDbContext<HilltopContext>(options => options.UseSqlite(connection));

var app = builder.Build();

// Challenge 1: apply migrations instead of EnsureCreated.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HilltopContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Hilltop University API. Try /api/students");

// Challenge 2: return a DTO, never the entity (no email, address, GPA or date of birth).
app.MapGet("/api/students", async (HilltopContext db) =>
    await db.Students.OrderBy(s => s.Id)
        .Select(s => new StudentDto(s.Id, s.FirstName, s.LastName, s.School))
        .ToListAsync());

app.MapGet("/api/students/{id:int}", async (int id, HilltopContext db) =>
    await db.Students.Where(s => s.Id == id)
        .Select(s => new StudentDto(s.Id, s.FirstName, s.LastName, s.School))
        .FirstOrDefaultAsync() is StudentDto student
        ? Results.Ok(student)
        : Results.NotFound());

// Challenge 2: parameterized query instead of string concatenation.
app.MapGet("/api/students/search", async (string name, HilltopContext db) =>
    await db.Students
        .FromSql($"SELECT * FROM Students WHERE FirstName LIKE '%' || {name} || '%'")
        .Select(s => new StudentDto(s.Id, s.FirstName, s.LastName, s.School))
        .ToListAsync());

// Challenge 1: a student's course codes, A to Z.
app.MapGet("/api/students/{id:int}/courses", async (int id, HilltopContext db) =>
{
    if (!await db.Students.AnyAsync(s => s.Id == id)) return Results.NotFound();

    var codes = await db.Enrollments
        .Where(e => e.StudentId == id)
        .Select(e => e.Course.Code)
        .OrderBy(code => code)
        .ToListAsync();
    return Results.Ok(codes);
});

app.Run();

record StudentDto(int Id, string FirstName, string LastName, string? School);

// Lets the test project start this app in memory.
public partial class Program { }
