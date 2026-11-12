// Challenges 1 and 2: make these tests pass WITHOUT editing this file.
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HilltopApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace HilltopApi.Tests;

// Starts the API in memory against its own throwaway SQLite file.
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public string DbPath { get; } =
        Path.Combine(Path.GetTempPath(), $"hilltop-test-{Guid.NewGuid():N}.db");

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__Hilltop", $"Data Source={DbPath};Pooling=False");
        return base.CreateHost(builder);
    }
}

internal static class Sql
{
    public static long Scalar(string dbPath, string sql)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath};Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return (long)command.ExecuteScalar()!;
    }
}

public class Challenge1Tests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact, Trait("Challenge", "1")]
    public void Migration_AddCourses_creates_and_seeds_courses_and_enrollments()
    {
        var path = Path.Combine(Path.GetTempPath(), $"hilltop-migrate-{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<HilltopContext>()
            .UseSqlite($"Data Source={path};Pooling=False")
            .Options;

        using (var db = new HilltopContext(options))
        {
            Assert.Contains(db.Database.GetMigrations(), m => m.EndsWith("_AddCourses"));
            db.Database.Migrate();
        }

        Assert.Equal(10L, Sql.Scalar(path, "SELECT COUNT(*) FROM Students"));
        Assert.Equal(4L, Sql.Scalar(path, "SELECT COUNT(*) FROM Courses"));
        Assert.Equal(9L, Sql.Scalar(path, "SELECT COUNT(*) FROM Enrollments"));
    }

    [Fact, Trait("Challenge", "1")]
    public async Task App_applies_migrations_on_startup()
    {
        var client = factory.CreateClient();
        (await client.GetAsync("/api/students")).EnsureSuccessStatusCode();

        Assert.Equal(1L, Sql.Scalar(factory.DbPath,
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = '__EFMigrationsHistory'"));
    }

    [Fact, Trait("Challenge", "1")]
    public async Task Student_courses_endpoint_returns_course_codes_sorted()
    {
        var client = factory.CreateClient();

        Assert.Equal(new[] { "BUSN 1500", "MEDI 2200" },
            await client.GetFromJsonAsync<string[]>("/api/students/4/courses"));
        Assert.Equal(new[] { "NURS 1010" },
            await client.GetFromJsonAsync<string[]>("/api/students/7/courses"));
        Assert.Empty((await client.GetFromJsonAsync<string[]>("/api/students/3/courses"))!);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.GetAsync("/api/students/999/courses")).StatusCode);
    }
}

public class Challenge2Tests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly string[] PublicFields = { "firstName", "id", "lastName", "school" };

    private async Task<JsonElement> GetJson(string url)
    {
        var response = await factory.CreateClient().GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static void AssertOnlyPublicFields(JsonElement student) =>
        Assert.Equal(PublicFields, student.EnumerateObject().Select(p => p.Name).Order().ToArray());

    [Fact, Trait("Challenge", "2")]
    public async Task Search_still_finds_matching_students()
    {
        var results = await GetJson("/api/students/search?name=Ann");
        Assert.Equal(2, results.GetArrayLength());
    }

    [Fact, Trait("Challenge", "2")]
    public async Task Search_is_not_vulnerable_to_sql_injection()
    {
        var results = await GetJson("/api/students/search?name=" + Uri.EscapeDataString("' OR '1'='1"));
        Assert.Equal(0, results.GetArrayLength());
    }

    [Theory, Trait("Challenge", "2")]
    [InlineData("/api/students")]
    [InlineData("/api/students/search?name=Ann")]
    public async Task Student_lists_expose_only_public_fields(string url)
    {
        foreach (var student in (await GetJson(url)).EnumerateArray())
            AssertOnlyPublicFields(student);
    }

    [Fact, Trait("Challenge", "2")]
    public async Task Single_student_exposes_only_public_fields()
    {
        AssertOnlyPublicFields(await GetJson("/api/students/1"));
    }
}
