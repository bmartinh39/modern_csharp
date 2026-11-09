# Exercise workspace

Infrastructure prepared on 2026-11-09. No exercise or lesson is active.
Existing course materials, AGENTS.md and README.md have been preserved.

## Projects

- `ModernCSharp.slnx`: solution containing both projects.
- `src/ExerciseWorkspace/`: reusable .NET 10 console project; nullable reference types and implicit usings enabled.
- `tests/ExerciseWorkspace.Tests/`: separate xUnit test project referencing the source project.
- `PROGRESS.md`: prior coverage, assessment evidence and future review queue.

The SDK is pinned to the installed 10.0.402 feature band in `global.json`, allowing later patches.
The test template pins Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio and
coverlet.collector package versions in its project file. These provide test discovery,
execution, assertions and optional coverage collection. NuGet access is needed for the
first restore on a machine without cached packages. No application dependencies are needed.

## Run from the repository root

```powershell
dotnet restore ModernCSharp.slnx
dotnet build ModernCSharp.slnx --no-restore
dotnet run --project src/ExerciseWorkspace
dotnet test ModernCSharp.slnx --no-restore
```

After editing code, `dotnet test` rebuilds the projects automatically. To run only
the infrastructure check:

```powershell
dotnet test ModernCSharp.slnx --filter Category=Infrastructure
```

The initial console output only confirms readiness. The single infrastructure test
checks that the runner can load the referenced console assembly and find its entry point.
Its success is not evidence of C# mastery or exercise completion.

## Verification on 2026-11-09

- Package restore succeeded for both projects.
- Solution build succeeded with zero warnings and zero errors.
- xUnit discovered and passed the single infrastructure test (zero failures or skips).
- Console execution succeeded and printed `Exercise workspace ready. No exercise is active.`
- Git comparison confirmed no changes to the pre-existing AGENTS.md, README.md or material files.

## Reusing the workspace

Place future learner code alongside `Program.cs`, with requirement tests in the test
project. Keep one active exercise and adapt these projects rather than creating a
new project per activity. The initial Program.cs and InfrastructureTests.cs are
Codex-authored disposable scaffolding, not learner solutions.

Before replacing files, inspect them for learner contributions. Preserve valuable work
and obtain explicit permission before overwriting or deleting learner implementations.
Keep tests separate from implementations and record observed learning in PROGRESS.md.
