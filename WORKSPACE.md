# Exercise workspace

Infrastructure prepared on 2026-11-09. The remaining Unit 7 batch is ready in [SESSION.md](SESSION.md): ten activities, with coding instructions in [JobApplication.cs](src/ExerciseWorkspace/JobApplication.cs) and [ApplicationTextFile.cs](src/ExerciseWorkspace/ApplicationTextFile.cs).
The integrated Unit 7 exit task, [ApplicationBatchImporter.cs](src/ExerciseWorkspace/ApplicationBatchImporter.cs), is complete with guidance; its 13 tests pass as of 2026-11-10. Initial Unit 7 practice is complete with targeted reviews retained in PROGRESS.md. Earlier learner implementations are preserved.
The [stream ownership task](RESOURCE-EXERCISE.md) is set aside after the learner requested source-material-focused practice; its reader-disposal issue remains recorded.
The completed [salary range task](EXERCISE.md) and learner implementation are preserved.
Existing course materials, AGENTS.md and README.md have been preserved.

## Unit 7 checkpoint readiness — 2026-11-10

The full solution test run passes all 49 tests. TextPreview's unassessed reader-disposal
issue remains documented despite its passing stream-lifetime tests.
The local checkpoint commit is pending: include relevant learner source and tests
(currently untracked), AGENTS.md, PROGRESS.md, SESSION.md and workspace/exercise docs.
The solution/configuration files are already tracked; include them if changed.
Generated bin/obj/TestResults files are ignored. No reorganisation is needed to build
or begin Unit 8. Follow AGENTS.md's transition workflow when moving on.

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
Its success is not evidence of C# mastery or exercise completion. Exercise tests are
now present as well. The initial Unit 7 tasks and integrated batch importer have been
implemented and individually reviewed; the latest 13 batch-import tests pass.
Use the filtered command in the active source file while working on each task.

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

Put concise exercise requirements and the relevant test command in comments next
to the code the learner edits. Use SESSION.md to index activities when preparing
a batch; separate exercise Markdown files are not the default.

Before replacing files, inspect them for learner contributions. Preserve valuable work
and obtain explicit permission before overwriting or deleting learner implementations.
Keep tests separate from implementations and record observed learning in PROGRESS.md.
