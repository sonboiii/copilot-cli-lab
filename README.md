# Copilot CLI Challenges

A 20-minute hands-on lab for **COMP 4976**: two challenges that use **GitHub Copilot CLI**, an AI agent in your terminal, on the course's own stack (ASP.NET Core, EF Core, SQLite).

| Challenge | Skill |
|---|---|
| 1. EF Core: courses, enrollments, a migration and an endpoint | Driving `dotnet ef`, build and test from the terminal |
| 2. Security review: SQL injection and data leaks | Reviewing before you approve |

**Start here:** [LAB.md](LAB.md) (or [LAB.pdf](LAB.pdf))

## Quick start

```bash
# Fork this repo, then:
git clone https://github.com/YOUR-USERNAME/copilot-cli-lab.git
cd copilot-cli-lab
dotnet tool restore
npm run check                  # your progress: 0/2

npm install -g @github/copilot
copilot
```

Requirements: GitHub Copilot (Free works), the .NET 10 SDK, Node.js 22+, Git (Windows: Git Bash).

## Repo layout

```text
api/HilltopApi/             minimal API + EF Core + SQLite (Challenges 1 and 2)
api/HilltopApi.Tests/       xUnit integration tests: don't edit these
scripts/check.js            self-check: npm run check
logs/app.log                sample logs used in the presentation demo
docs/instructor/            reference solutions (try first!)
```

## License

MIT
