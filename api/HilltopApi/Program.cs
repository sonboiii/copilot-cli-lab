using HilltopApi.Data;
using HilltopApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("Hilltop") ?? "Data Source=hilltop.db";
builder.Services.AddDbContext<HilltopContext>(options => options.UseSqlite(connection));

var app = builder.Build();

// Create the database and seed it on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HilltopContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "Hilltop University API. Try /api/students");

app.MapGet("/api/students", async (HilltopContext db) =>
    await db.Students.OrderBy(s => s.Id).ToListAsync());

app.MapGet("/api/students/{id:int}", async (int id, HilltopContext db) =>
    await db.Students.FindAsync(id) is Student student
        ? Results.Ok(student)
        : Results.NotFound());

app.MapGet("/api/students/search", async (string name, HilltopContext db) =>
    await db.Students
        .FromSqlRaw($"SELECT * FROM Students WHERE FirstName LIKE '%{name}%'")
        .ToListAsync());

app.Run();

// Lets the test project start this app in memory.
public partial class Program { }
