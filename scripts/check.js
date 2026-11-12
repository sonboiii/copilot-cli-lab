// Self-check for the three challenges. Run:  npm run check
// You may read this file, but passing by editing it doesn't count!
const { spawnSync } = require("child_process");
const fs = require("fs");
const path = require("path");
const crypto = require("crypto");

const root = path.join(__dirname, "..");
const HASHES = {
  "api/HilltopApi.Tests/ApiTests.cs": "e8eeb47ff9856afe47f772bbf4b2555304898eb9150bed27f30c19b1d7837204",
};
const run = (cmd, args) =>
  spawnSync(cmd, args, { cwd: root, encoding: "utf8", maxBuffer: 64 * 1024 * 1024 });

function assertUnchanged(file) {
  const body = fs.readFileSync(path.join(root, file), "utf8").replace(/\r\n/g, "\n");
  if (crypto.createHash("sha256").update(body).digest("hex") !== HASHES[file])
    throw new Error(`${file} was edited. Undo with: git checkout -- ${file}`);
}

let buildError = null;
function dotnetTests(challenge) {
  assertUnchanged("api/HilltopApi.Tests/ApiTests.cs");
  if (buildError) throw new Error(buildError);
  const r = run("dotnet", ["test", "api/HilltopApi.Tests/HilltopApi.Tests.csproj", "--no-build", "--nologo",
    "--filter", `Challenge=${challenge}`]);
  if (r.error) throw new Error("dotnet not found. Install the .NET 10 SDK.");
  if (r.status !== 0) {
    const out = r.stdout + r.stderr;
    const failed = (out.match(/Failed:\s+(\d+)/) || [])[1] || "some";
    const first = (out.match(/^\s*Failed (HilltopApi\.Tests\.\S+)/m) || [])[1];
    throw new Error(`${failed} test(s) failing${first ? `, e.g. ${first.split(".").pop()}` : ""}. Run: dotnet test api/HilltopApi.slnx`);
  }
}

const checks = [
  ["Challenge 1: courses, enrollments, AddCourses migration, endpoint", () => dotnetTests(1)],
  ["Challenge 2: search is injection-safe, no private fields leak", () => dotnetTests(2)],
];

console.log("Building the .NET API (the first run takes a little longer)...");
const build = run("dotnet", ["build", "api/HilltopApi.slnx", "--nologo", "-v", "q"]);
if (build.error) buildError = "dotnet not found. Install the .NET 10 SDK.";
else if (build.status !== 0) {
  const err = (build.stdout + build.stderr).split("\n").find((l) => /error [A-Z]+\d+/.test(l));
  buildError = "the API doesn't build" + (err ? `: ${err.trim().replace(/^.*?: error/, "error")}` : ". Run: dotnet build api/HilltopApi.slnx");
}

let passed = 0;
for (const [name, fn] of checks) {
  try { fn(); passed++; console.log(`PASS  ${name}`); }
  catch (e) { console.log(`FAIL  ${name}\n      ${e.message}`); }
}
console.log(`\n${passed}/${checks.length} challenges complete`);
process.exitCode = passed === checks.length ? 0 : 1;
