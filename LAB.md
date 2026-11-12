# Lab: Copilot CLI Challenges

**COMP 4976 · Son Bui · Time: 20 minutes · macOS, Linux, Windows (Git Bash)**

You've used Copilot inside an editor. In this lab the agent lives in your **shell**: it runs `dotnet ef`, `dotnet build` and the tests, reads the errors, and fixes them, while **you** approve every step. You'll work on a Hilltop University API (ASP.NET Core, EF Core, SQLite) built on the week 9 student data.

Each step gives you a **prompt to paste** into Copilot and tells you what to expect. Your job is to **read what Copilot proposes before you approve it**.

| | Challenge | Time |
|---|---|---|
| 0 | Setup | 4 min |
| 1 | EF Core: add courses and enrollments with a migration and an endpoint | 10 min |
| 2 | Security review: find and fix two vulnerabilities in the API | 6 min |

> [!WARNING]
> **Rules**
> 1. When Copilot asks permission, choose **Yes**, not "Yes, and approve for the rest of the session". Read the diff or command first.
> 2. Don't edit `api/HilltopApi.Tests/ApiTests.cs`. The checker detects edits.
> 3. Run `npm run check` at any time to see your progress.

## Setup (4 min)

You need **GitHub Copilot** (Copilot Free includes the CLI; turn it on at <https://github.com/settings/copilot>), the **.NET 10 SDK**, **Node.js 22+** and **Git**. On Windows, use **Git Bash**.

1. Fork **github.com/sonboiii/copilot-cli-lab**, then clone **your fork**:

   ```bash
   git clone https://github.com/YOUR-USERNAME/copilot-cli-lab.git
   cd copilot-cli-lab
   dotnet tool restore      # installs dotnet-ef for this repo
   npm run check            # builds the API; expect 0/2
   ```

2. Install Copilot CLI:

   ```bash
   npm install -g @github/copilot     # or run: gh copilot
   ```

3. Start it **inside the project folder**, choose **Yes, proceed** to trust the folder, and type `/login` if asked:

   ```bash
   copilot
   ```

## Challenge 1: EF Core with migrations (10 min)

`api/HilltopApi` is a minimal API whose only entity is `Student`. You'll add courses and enrollments the way a real team would: with a migration.

**Step 1: Understand the project (one-shot).** Quit Copilot (Ctrl+C), then run this in the terminal:

```bash
copilot -p "Explain how api/HilltopApi creates its database and what the Student entity contains. Don't change anything."
```

**Expected:** it mentions SQLite, `EnsureCreated()` in `Program.cs`, and the seeded students.

**Step 2: Add the entities.** Start `copilot` and paste:

```text
In api/HilltopApi/Models, add a Course entity (Id, Code, Title, School) and an Enrollment entity (StudentId, CourseId) with navigation properties. Give Enrollment a composite key of StudentId and CourseId. Add DbSets named Courses and Enrollments to HilltopContext.
```

Approve the new files and the context change once you've read them.

**Step 3: Seed the data.** In the same session, paste:

```text
In HilltopContext, seed with HasData. Courses: 1 "NURS 1010" "Foundations of Nursing" Nursing; 2 "MEDI 2200" "Human Anatomy" Medicine; 3 "BUSN 1500" "Intro to Accounting" Business; 4 "MINE 3100" "Mine Safety" Mining. Enrollments (StudentId-CourseId): 1-1, 7-1, 4-2, 6-2, 10-2, 4-3, 5-3, 8-3, 2-4.
```

**Step 4: Create the migration and use it.** Paste:

```text
Create an EF Core migration named AddCourses for the api/HilltopApi project using dotnet ef. Then change Program.cs to apply migrations on startup with Database.Migrate() instead of EnsureCreated().
```

**Expected:** Copilot asks to run `dotnet ef migrations add AddCourses ...`. Approve it, and a `Migrations` folder appears.

**Step 5: Add the endpoint and let Copilot test it.** Paste:

```text
Add GET /api/students/{id}/courses to Program.cs. Return the student's course codes sorted A to Z, or 404 if the student doesn't exist. Then run: dotnet test api/HilltopApi.Tests --filter Challenge=1 and fix anything that fails.
```

**Expected:** Copilot runs the tests, reads any failures, and fixes them. This is the **agent loop**. It may take a couple of rounds.

**Step 6: Check and commit.** Quit Copilot, then:

```bash
npm run check                    # PASS  Challenge 1
git add -A && git commit -m "Challenge 1: courses and enrollments"
```

> [!NOTE]
> **Troubleshooting:** if the app reports "table Students already exists", delete `api/HilltopApi/hilltop.db`. It was made by the old `EnsureCreated()`.

## Challenge 2: Security review (6 min)

The API looks fine at a glance, but it has **two security bugs**. You'll have Copilot find them **before** it changes anything.

**Step 1: Review only (one-shot).** In the terminal:

```bash
copilot -p "Review api/HilltopApi/Program.cs for security vulnerabilities. List each issue with its line and severity. Don't edit anything."
```

**Expected:** Copilot finds (1) **SQL injection** in the `/api/students/search` endpoint, which builds SQL from user input with `FromSqlRaw`, and (2) a **data leak**: the endpoints return the whole `Student`, including email, home address, GPA and date of birth.

**Step 2: Fix the SQL injection.** Start `copilot` and paste:

```text
Fix the SQL injection in the /api/students/search endpoint by using a parameterized query instead of string interpolation. It must still match partial first names.
```

**Step 3: Stop the data leak.** Paste:

```text
Add a StudentDto record with only Id, FirstName, LastName and School. Return it from every student endpoint instead of the Student entity.
```

**Step 4: Prove it.** Paste:

```text
Run: dotnet test api/HilltopApi.Tests --filter Challenge=2 and fix anything that fails.
```

**Step 5: Check, commit and push.** Quit Copilot, then:

```bash
npm run check                    # 2/2 challenges complete
git add -A && git commit -m "Challenge 2: security fixes"
git push
```

## Troubleshooting

| Problem | Fix |
|---|---|
| `copilot: command not found` | Reopen the terminal. Still failing? Use `gh copilot` instead. |
| "You don't have access to Copilot" | Turn on Copilot Free at github.com/settings/copilot, then `/login` again. |
| `dotnet ef: command not found` | Run `dotnet tool restore` in the repo folder. |
| The check says "the API doesn't build" | Paste into Copilot: "dotnet build api/HilltopApi.slnx fails. Fix it." |
| "table Students already exists" | Delete `api/HilltopApi/hilltop.db`, then run the check again. |
| A test still fails after Copilot's fix | Paste the failing test's name and error into Copilot and ask it to fix that one. |
| The test file was edited | `git checkout -- api/HilltopApi.Tests/` |
| Copilot changed something you didn't want | `git diff` to see it, `git restore <file>` to undo it. |
| Copilot writes PowerShell (Windows) | Run the lab in **Git Bash**, or say "use bash, not PowerShell". |
| Out of requests | Pair up with a neighbour. |

---

**References:** [About GitHub Copilot CLI](https://docs.github.com/en/copilot/concepts/agents/about-copilot-cli) · [GitHub Copilot CLI 101](https://github.blog/ai-and-ml/github-copilot/github-copilot-cli-101-how-to-use-github-copilot-from-the-command-line/) · [EF Core migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/) · [EF Core raw SQL queries](https://learn.microsoft.com/ef/core/querying/sql-queries)
