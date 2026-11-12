# Reference solutions

Try the challenges first. Copilot's code will differ from this. Only `npm run check` matters.

## Challenge 1: EF Core

The full files are in [`solution/`](solution): `Models/Course.cs`, `Models/Enrollment.cs`, `Data/HilltopContext.cs` and `Program.cs`. Then:

```bash
dotnet tool restore
dotnet ef migrations add AddCourses --project api/HilltopApi
```

Key points: the composite key `HasKey(e => new { e.StudentId, e.CourseId })`, `HasData` for both tables, `db.Database.Migrate()` instead of `EnsureCreated()`, and a 404 when the student doesn't exist (not just an empty list).

Common mistakes: forgetting to switch to `Migrate()` (the startup test fails), naming the migration differently, and changing the model **after** creating the migration. EF Core 9+ then refuses to migrate because of pending model changes. Fix it by adding another migration, or by removing and re-adding the one you made.

## Challenge 2: security

1. **SQL injection:** `FromSqlRaw($"... LIKE '%{name}%'")` pastes user input into SQL. `' OR '1'='1` returns every student. Fix it with `FromSql($"... LIKE '%' || {name} || '%'")` (EF turns the interpolation into a parameter) or with LINQ: `Where(s => EF.Functions.Like(s.FirstName, $"%{name}%"))`. EF Core's analyzer warns about the original as **EF1002**.
2. **Data leak:** the endpoints return the `Student` entity, including `email`, `homeAddress`, `gpa` and `dateOfBirth`. Fix it by projecting to a DTO: `record StudentDto(int Id, string FirstName, string LastName, string? School)`.

## Expected answers

| Check | Answer |
|---|---|
| `/api/students/4/courses` | `["BUSN 1500","MEDI 2200"]` |
| `/api/students/3/courses` | `[]` |
| `/api/students/999/courses` | 404 |
| `search?name=Ann` | 2 students |
| `search?name=' OR '1'='1` | 0 students |
