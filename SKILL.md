---
name: local-dev-workflow
description: Repository investigation, implementation, debugging, build and test workflow for ASP.NET/C#, Docker/Compose, SQL Server/PostgreSQL, Vue and Angular projects.
---

# Local Development Workflow

Load this skill for implementation, debugging, refactoring, build failures, test failures, Docker issues, database-related code, or frontend/backend integration work.

## Core Principle

Do not guess when repository inspection or a shell command can establish the answer.

Use the available tools proactively. In particular, use:

- `grep` for content search;
- `find` for file/project discovery;
- shell commands for inspection and verification;
- `git` for working-tree context, diffs, history, and blame;
- `dotnet` for .NET project discovery, build, and tests.

For .NET work, `dotnet build` and, when tests exist, `dotnet test` are expected verification steps.

## 1. Establish Repository State

Before a substantial change:

```sh
pwd
git status --short
git diff
```

Discover major project files:

```sh
find . -maxdepth 4 -type f \( \
  -name '*.sln' -o \
  -name '*.slnx' -o \
  -name '*.csproj' -o \
  -name 'package.json' -o \
  -name 'angular.json' -o \
  -name 'vite.config.*' -o \
  -name 'compose.yaml' -o \
  -name 'docker-compose*.yml' -o \
  -name 'Dockerfile*' \
\) -print
```

Do not recursively inspect `.git`, `node_modules`, `bin`, `obj`, or generated build directories unless necessary.

## 2. Search Before Editing

Use `grep` whenever you need to find:

- symbols;
- configuration keys;
- routes;
- environment variables;
- service registrations;
- EF mappings;
- Docker service names;
- frontend API calls;
- existing tests.

Examples:

```sh
grep -RIn \
  --exclude-dir=.git \
  --exclude-dir=node_modules \
  --exclude-dir=bin \
  --exclude-dir=obj \
  "SearchTerm" .
```

Use `find` for structural discovery:

```sh
find . -type f -name '*Controller.cs'
find . -type f -name '*Tests.cs'
find . -type f -name '*.vue'
find . -type f -name '*.component.ts'
```

Prefer native OpenCode search/read tools when they are more efficient, but use `grep` and `find` freely when shell inspection is clearer.

## 3. Use Git for Context

Use:

```sh
git status --short
git diff
git diff --check
git log --oneline -n 20
git blame <file>
```

Use `git log`, `git show`, and `git blame` when current code intent is unclear.

Never destroy or overwrite unrelated user changes.

Never commit, push, reset, clean, or rewrite history unless explicitly requested.

## 4. ASP.NET / .NET Workflow

Find the appropriate solution or project first.

Useful commands:

```sh
find . -maxdepth 4 -type f \( -name '*.sln' -o -name '*.slnx' -o -name '*.csproj' \) -print
dotnet --info
```

Inspect relevant:

- `.csproj`;
- `Program.cs`;
- service registrations;
- controllers/endpoints;
- application services/handlers;
- entities;
- `DbContext`;
- EF configurations;
- migrations;
- tests.

### After Code Changes

Run the narrowest relevant build:

```sh
dotnet build path/to/Project.csproj
```

or:

```sh
dotnet build path/to/Solution.sln
```

If restore is needed:

```sh
dotnet restore path/to/Solution.sln
dotnet build path/to/Solution.sln --no-restore
```

### Tests

Determine whether test projects exist:

```sh
find . -type f \( \
  -name '*Test*.csproj' -o \
  -name '*Tests*.csproj' \
\) -print
```

If relevant tests exist, run them.

Prefer a targeted test first:

```sh
dotnet test path/to/Tests.csproj --filter "FullyQualifiedName~RelevantFeature"
```

Then broaden if appropriate:

```sh
dotnet test path/to/Tests.csproj
```

For changes spanning shared projects:

```sh
dotnet test path/to/Solution.sln
```

Do not claim successful verification without actually observing the command succeed.

If tests exist but cannot be run, explain why.

## 5. Build-Failure Workflow

When `dotnet build` fails:

1. Read the first meaningful compiler errors.
2. Locate the referenced files.
3. Fix the underlying cause.
4. Re-run the same build.
5. Repeat until clean or blocked by an external problem.

Do not hide errors by weakening nullable, analyzer, or compiler settings unless requested and justified.

Use:

```sh
dotnet build <target> --no-restore
```

after the initial restore when possible.

## 6. Test-Failure Workflow

When `dotnet test` fails:

1. Identify the failing test(s).
2. Read the test implementation.
3. Read the production code under test.
4. Determine whether the defect is in implementation or test expectation.
5. Fix the correct side.
6. Run the failed test/filter again.
7. Run the broader relevant test project afterward.

Do not modify assertions merely to obtain a green test suite.

## 7. Entity Framework / Database Workflow

Before changing database-related code, inspect:

```sh
grep -RIn --exclude-dir=bin --exclude-dir=obj "DbContext" .
find . -type d -iname '*Migration*' -print
find . -type f -iname '*Migration*.cs' -print
```

Understand:

- provider: SQL Server or PostgreSQL;
- schema;
- entity configuration;
- keys/indexes;
- relationships;
- migration state.

Keep SQL provider-specific behavior in mind.

For SQL Server, avoid assuming PostgreSQL syntax/features.

For PostgreSQL, avoid assuming SQL Server semantics.

Use parameterized SQL and existing EF/database abstractions.

Do not execute destructive database commands without explicit authorization.

## 8. Docker / Compose Workflow

Discover the configuration:

```sh
find . -maxdepth 4 -type f \( \
  -name 'Dockerfile*' -o \
  -name 'compose.yaml' -o \
  -name 'compose.yml' -o \
  -name 'docker-compose*.yml' -o \
  -name 'docker-compose*.yaml' \
\) -print
```

Validate Compose before changing runtime state:

```sh
docker compose config
```

Useful diagnostics:

```sh
docker compose ps
docker compose logs --tail=200
docker compose logs --tail=200 <service>
```

When appropriate:

```sh
docker compose build <service>
docker compose up -d <service>
```

Do not destroy volumes or prune Docker state without explicit permission.

Never assume container/service names—read Compose first.

## 9. Vue Workflow

Inspect:

```sh
cat package.json
find . -maxdepth 3 -type f \( -name '*.vue' -o -name '*.ts' -o -name '*.js' \) -print
```

Determine:

- Vue version;
- Vite/other bundler;
- TypeScript vs JavaScript;
- Composition API vs Options API;
- Pinia/Vuex;
- test/lint scripts.

Search existing components/composables before adding new ones.

After changes, inspect package scripts and run the relevant existing command, commonly:

```sh
npm run build
npm test
npm run test
npm run lint
```

Do not invoke scripts that do not exist.

## 10. Angular Workflow

Inspect:

```sh
cat package.json
cat angular.json
```

Determine:

- Angular version;
- standalone components vs NgModules;
- test tooling;
- lint tooling;
- project/application names.

Search existing services/components/patterns before introducing new structures.

Run available build/test/lint scripts after changes.

## 11. Frontend + ASP.NET Integration

When debugging cross-stack behavior, verify both sides.

Check:

- frontend base URL/config;
- proxy configuration;
- ASP.NET routes;
- CORS;
- authentication headers/cookies;
- serialization;
- API request/response models;
- Docker networking and exposed ports.

Use `grep` to locate the same route/config key on both frontend and backend.

Do not assume `localhost` means the same host from inside a container.

## 12. Completion Checklist

Before concluding implementation work:

```sh
git status --short
git diff --check
git diff
```

For .NET changes:

```sh
dotnet build <relevant-target>
```

If tests are present and relevant:

```sh
dotnet test <relevant-test-target>
```

For frontend changes, run the repository's available build/test/lint script.

Report:

- files/areas changed;
- build command and result;
- test command and result;
- any remaining limitation or unverified behavior.

Do not claim verification you did not perform.
