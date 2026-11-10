# Unit 7 — Debug stream ownership

Status: First learner implementation reviewed; all six stream tests pass, but reader
disposal is still missing. Correction pending. Original deliberate bugs supplied by Codex.
Previous SalaryRange work is preserved.

## API reminders before starting

- `StreamReader` wraps a byte stream to read text. `ReadLine()` returns a line without
  its newline, or null at end of input. Disposing a reader closes its underlying stream
  by default, even if that stream was supplied by someone else.
- The reader has a constructor option that changes that ownership behaviour:
  `new StreamReader(source, leaveOpen: true)`. `leaveOpen:` is a named argument.
  This lets you dispose the reader while keeping the underlying stream open.
- `Func<Stream>` is a delegate with no arguments that returns a Stream. Calling
  `openStream()` acquires a stream. For this exercise, LoadFirstLine owns that result.
- `?? throw` throws when the value to its left is null. Here an empty input throws
  InvalidDataException, meaning the input content is unsuitable for this operation.
- Do not assume a reader leaves the byte position just after the first line: it buffers
  input. This task requires the borrowed stream to remain open, not a specific position.

## Your task

Fix the resource lifetimes in `src/ExerciseWorkspace/TextPreview.cs`.

1. ReadFirstLine borrows its stream and must leave it open on success and failure.
2. It must still dispose its own StreamReader on every exit path.
3. LoadFirstLine owns the stream returned by its delegate and must dispose it on
   success and failure.
4. Return the first line. Preserve InvalidDataException for empty input and let I/O
   exceptions propagate unchanged. No new catch blocks are needed.

Assume a non-null, readable stream and a valid delegate; additional argument validation
is outside this focused exercise. Do not change public signatures or tests.

## Run

```powershell
dotnet test --filter FullyQualifiedName~TextPreviewTests
```

Six cases check borrowed and owned streams under success, empty input and I/O failure.
Some initially pass because one ownership mistake masks another. Re-run all six after
each change. The tests use in-memory streams; no files or external services are needed.

Start by predicting which method owns each object. Then make the smallest corrections
and request review. Tests observe stream disposal; review also checks reader disposal.
