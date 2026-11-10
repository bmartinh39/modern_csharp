# Unit 7 — Protect a salary range during updates

Status: Completed with targeted guidance. Learner implementation reviewed on 2026-11-09;
all 12 requirement cases pass, exception-message wording is corrected and the static-helper
reasoning check is complete. The original starter was Codex-authored.

Implement the constructor and `Update` in `src/ExerciseWorkspace/SalaryRange.cs`.
Keep both properties externally read-only. This is a deliberately mutable class to
practise safe updates; it is not a prescription for modelling every salary value.

## Requirements

- Every successfully constructed or updated range satisfies `0 <= Minimum <= Maximum`.
- Zero and equal bounds are valid.
- A negative bound throws `ArgumentOutOfRangeException` with the matching parameter
  name (`minimum` or `maximum`). Check negative bounds before comparing their order.
- Nonnegative bounds in reversed order throw `ArgumentException`.
- A valid update replaces both bounds, even if the new interval is entirely above
  or below the old one.
- A rejected update leaves both original bounds unchanged.
- Let exceptions propagate to the caller; this class has no recovery to perform.
- Choose any small private helper if useful. Do not change the tests to fit your code.

A guard clause rejects invalid input near the beginning of an operation. Think about
the order of validation and state changes so that throwing cannot leave a partial update.
`nameof(minimum)` gives the parameter name as a string without hard-coding it.

## Run

From the repository root:

```powershell
dotnet test --filter FullyQualifiedName~SalaryRangeTests
```

There are 12 cases. The original starter failed with `NotImplementedException`; the learner implementation now passes them.
`dotnet test` also runs the separate infrastructure check. The console remains a readiness
placeholder; no console wiring is required for this task.

After implementing, ask for a review. Passing tests checks the specified behaviour;
the review will also examine your reasoning and implementation.
