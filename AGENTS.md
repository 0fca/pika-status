# OpenCode Project Instructions (pika-status)

## Purpose
Work as a pragmatic software-engineering agent for this repository.

The usual stack is:
- ASP.NET / ASP.NET Core
- C#
- Entity Framework Core
- Docker / Docker Compose
- Microsoft SQL Server / PostgreSQL
- Vue / Angular / JavaScript / TypeScript

Prefer repository evidence over assumptions. Inspect the existing code, project files, configuration, tests, and conventions before making architectural or style decisions.

## General Working Rules

1. Understand before editing.
   - Inspect relevant files and nearby implementations first.
   - Search for existing abstractions, patterns, tests, naming conventions, and configuration before introducing new ones.
   - Do not create a second implementation of something that already exists.

2. Keep changes scoped.
   - Implement requested behavior with the smallest coherent change.
   - Avoid unrelated refactors unless necessary for correctness.
   - Do not reformat unrelated files.
   - Preserve public APIs unless the task requires changing them.

3. Verify changes.
   - After modifying .NET code, run `dotnet build` on the narrowest appropriate solution/project.
   - If relevant tests exist, run `npm test`, `npm run test`, or `dotnet test`.
   - Report failures that pre-existed or are unrelated; do not silently hide them.

4. Use tools rather than guessing.
   - Use repository search, shell commands, Git, and build/test output to establish facts.
   - Prefer deterministic inspection over speculative reasoning.

5. Do not invent infrastructure.
   - Do not assume container names, ports, database names, schemas, environment variables, credentials, or service URLs.
   - Inspect `compose.yaml`, `docker-compose*.yml`, Dockerfiles, `.env.example`, launch settings, configuration files, and source code first.

## Repository Discovery

At the beginning of a non-trivial task, determine the repository shape.

Useful commands:
```sh
pwd
git status --short
find . -maxdepth 3 -type f \( -name '*.sln' -o -name '*.slnx' -o -name '*.csproj' -o -name 'package.json' -o -name 'compose.yaml' -o -name 'docker-compose*.yml' \) -print
```

Avoid scanning generated directories: `.git`, `bin`, `obj`, `node_modules`, `dist`, `coverage`, `.angular`, `.nuxt`.

## Git Rules

Before significant edits: `git status --short`, `git diff`.
After edits: `git diff --check`, `git diff --stat`, `git diff`.

Do not commit, push, rewrite history, or use destructive commands (`git reset --hard`, `git clean -fd`) without explicit permission.

## ASP.NET / C# Rules

Follow existing conventions. Prefer DI, async APIs, and framework-provided abstractions.
Respect nullable reference types. Do not suppress warnings with `!` unless the invariant is established.

For EF Core: Inspect `DbContext`, entity configuration, migrations, and query patterns before changes. Avoid accidental client-side evaluation.

## Frontend Rules (Vue/Angular)

Determine version and tooling from `package.json`. Respect existing styles (Composition vs Options API, TypeScript usage, etc.).
Always run relevant verification scripts (`npm run build`, `npm test`, `npm run lint`) after changes.

## Security

Never print, commit, or expose passwords, access tokens, private keys, connection strings, or secrets. Treat authentication, authorization, and SQL construction as security-sensitive.

## Completion Standard

Before reporting completion:
- Inspect `git diff`.
- Run relevant build and tests.
- Mention exactly what was verified and any verification that could not be performed.
- Keep the final response concise and factual. Do not say "done" or "fixed" without evidence.
