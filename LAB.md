# Lab: Copilot CLI Challenges

**COMP 4976 · Son Bui · Time: 20 minutes · macOS, Linux, Windows (Git Bash)**

You've used Copilot inside an editor. This lab is about what changes when the agent lives in your **shell**: it runs `dotnet ef`, `dotnet build` and the tests, reads the errors, and tries again, while **you** review every step. You'll work on a Hilltop University API (ASP.NET Core, EF Core, SQLite) built on the week 9 student data. There are two challenges. Each one has a goal, a spec and an automatic check. There are no copy-paste prompts: writing them is part of the challenge.

| | Challenge | Time |
|---|---|---|
| 0 | Setup | 4 min |
| 1 | EF Core: add courses and enrollments with a migration and an endpoint | 10 min |
| 2 | Security review: find and fix two vulnerabilities in the API | 6 min |

> [!WARNING]
> **Rules**
> 1. Make every change **through Copilot CLI**. You can read files, but don't edit them by hand.
> 2. Approve actions **one at a time**: choose **Yes**, not "Yes, and approve for the rest of the session".
> 3. Don't edit `api/HilltopApi.Tests/ApiTests.cs`. The checker detects edits.
> 4. Run `npm run check` at any time to see your progress.

## Setup (4 min)

You need **GitHub Copilot** (Copilot Free includes the CLI; turn it on at <https://github.com/settings/copilot>), the **.NET 10 SDK**, **Node.js 22+** and **Git**. On Windows, use **Git Bash**.

1. Fork **github.com/sonboiii/copilot-cli-lab**, then clone **your fork** and restore everything:

   ```bash
   git clone https://github.com/YOUR-USERNAME/copilot-cli-lab.git
   cd copilot-cli-lab
   dotnet tool restore      # installs dotnet-ef for this repo
   npm run check            # builds the API; expect 0/2
   ```

2. Install Copilot CLI and start it **inside the project folder**:

   ```bash
   npm install -g @github/copilot     # or run: gh copilot
   copilot                            # choose "Yes, proceed", then /login if asked
   ```

> [!NOTE]
> The old `gh copilot suggest` and `gh copilot explain` commands are retired. Tutorials that use them are out of date.

## Challenge 1: EF Core with migrations (10 min)

`api/HilltopApi` is a minimal API with EF Core and SQLite. Its only entity is `Student`, and it creates its database with `EnsureCreated()`.

**Goal:** add courses and enrollments the way a real team would, using a migration.

**Spec:**

- Add a `Course` entity (`Id`, `Code`, `Title`, `School`) and an `Enrollment` entity (`StudentId`, `CourseId`, with a **composite key**). The DbSets must be named `Courses` and `Enrollments`.
- Seed them with `HasData`:

  | Id | Code | Title | School | Enrolled students (by Id) |
  |---|---|---|---|---|
  | 1 | NURS 1010 | Foundations of Nursing | Nursing | 1, 7 |
  | 2 | MEDI 2200 | Human Anatomy | Medicine | 4, 6, 10 |
  | 3 | BUSN 1500 | Intro to Accounting | Business | 4, 5, 8 |
  | 4 | MINE 3100 | Mine Safety | Mining | 2 |

- Create a migration named **`AddCourses`** with `dotnet ef`.
- On startup, **apply migrations** instead of calling `EnsureCreated()`.
- Add `GET /api/students/{id}/courses`. It returns the student's course **codes**, sorted A to Z, or **404** if the student doesn't exist.

**Tip:** Copilot can read this file. Try asking it to *"implement Challenge 1 from LAB.md, then run the Challenge 1 tests"* and watch the build, fix, re-run loop.

**Done when** `npm run check` shows `PASS  Challenge 1`. Commit your work, including the `Migrations` folder.

> [!NOTE]
> **Troubleshooting:** if the app reports "table Students already exists", delete the old `api/HilltopApi/hilltop.db`. It was made by `EnsureCreated()`.

## Challenge 2: Security review (6 min)

The API passes code review at a glance, but it has **two security bugs**. The Challenge 2 tests describe the attacks, not the fix.

**Goal:** have Copilot **review** `api/HilltopApi/Program.cs` first, then fix what it finds.

**Requirements (from the tests):**

- `/api/students/search?name=...` must be safe from **SQL injection**, and still find real matches.
- Every student endpoint must return **only** `id`, `firstName`, `lastName` and `school`.

**Suggested approach:**

```bash
copilot -p "Review api/HilltopApi/Program.cs for security vulnerabilities. List each issue with its line and severity. Don't edit anything."
```

Then start an interactive session to fix the issues. Read each diff before you approve it.

**Done when** `npm run check` shows `PASS  Challenge 2`. Commit your work.

> [!WARNING]
> **Think about it:** EF Core ships an analyzer (**EF1002**) that flags one of these bugs during `dotnet build`. Did you notice the warning? Did Copilot? Reviewing code, AI-written or not, includes reading warnings.

## Finish

```bash
npm run check      # 2/2 challenges complete
git push
```

**Finished early?** Ask Copilot to add a `POST /api/students` endpoint that validates its input, with a test. Then try **plan mode**: press **Shift+Tab** in a session before a bigger change.

## Troubleshooting

| Problem | Fix |
|---|---|
| `copilot: command not found` | Reopen the terminal. Still failing? Use `gh copilot` instead. |
| "You don't have access to Copilot" | Turn on Copilot Free at github.com/settings/copilot, then `/login` again. |
| `dotnet ef: command not found` | Run `dotnet tool restore` in the repo folder. |
| The check says "the API doesn't build" | Ask Copilot: "dotnet build fails, fix it", or run `dotnet build api/HilltopApi.slnx` yourself. |
| "table Students already exists" | Delete `api/HilltopApi/hilltop.db`, then run the check again. |
| The test file was edited | `git checkout -- api/HilltopApi.Tests/` |
| Copilot writes PowerShell (Windows) | Run the lab in **Git Bash**, or say "use bash, not PowerShell". |
| Out of requests | Pair up with a neighbour. |

---

**References:** [About GitHub Copilot CLI](https://docs.github.com/en/copilot/concepts/agents/about-copilot-cli) · [GitHub Copilot CLI 101](https://github.blog/ai-and-ml/github-copilot/github-copilot-cli-101-how-to-use-github-copilot-from-the-command-line/) · [EF Core migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/) · [EF Core raw SQL queries](https://learn.microsoft.com/ef/core/querying/sql-queries)
