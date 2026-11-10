# Unit 7 — Prepared completion session

Status: Initial Unit 7 practice completed on 2026-11-10; F09 async-disposal understanding remains scheduled for review.
Next activity: Brief transfer retrieval of retained weak points, then prepare Unit 8. No unfinished mandatory coding assignment.
Aim: finish the remaining initial Unit 7 practice, not exhaustive language mastery.
Do not reveal prediction answers in advance. No learner solutions or answer key are stored here.

## Delivery rules

- Use this queue without new scaffolding or rereading files between theory answers.
- Deliver any listed explanation before its check; API names/syntax are reminders, not puzzles.
- Coding instructions are in the source comments. Link the file rather than repeating the specification.
- Run only the relevant tests when a coding implementation is ready for review.
- Adapt: skip redundant checks when understanding is clear; use the prepared alternative if needed.
- Keep working results in conversation; update this file at a natural boundary. Update PROGRESS.md after substantial coding, significant mastery changes or session end.
- Preserve SalaryRange, ImportAttempt and TextPreview. TextPreview is not a completion prerequisite.
- Difficulty adjustment after F07: learner finds coding tasks too easy. Skip repeated isolated basics; prefer integrated tasks with real design/debugging decisions and behavioural requirements that do not prescribe every construct. Keep necessary API reminders. Prepare any replacement task at a natural boundary, not between short answers.

## Prepared queue

All section references below refer to the existing Unit 7 material.

### U7-F01 — Short retrieval

- Unit/topics: 7; validation precedence, filters, finally.
- Type/difficulty: Retrieval / Foundation.
- Status: Completed; recalled validation precedence, false-filter propagation and finally on return. Tutor clarified other catches in the same structure may still match.
- Sources: sections 9, 16, 18; completed SalaryRange.cs and ImportAttempt.cs.
- Tests: None; answer in chat.
- Prompts, presented individually:
  1. A public method receives two bounds; one is negative and their order is also invalid. How do you decide which validation error to report first?
  2. A catch filter is false. Does the catch run, and what can happen next?
  3. A return statement runs inside try. What happens to an associated finally block?
- Do not drill exception names again if the reasoning is sound. If a gap appears, use F04 or F08 rather than generating another worksheet.

### U7-F02 — Null guards and public boundaries

- Unit/topics: 7; validation versus handling, null guards; Unit 2 nullability interleave.
- Type/difficulty: Explain + immediate check / Foundation.
- Status: Completed; correctly distinguished nullable annotations and compiler guidance from runtime enforcement.
- Sources: sections 3–5, 21–22.
- Tests: None; leads to F03.
- Teaching prerequisite: Explain compile-time nullable annotations versus runtime validation at public boundaries; demonstrate the section 21 ThrowIfNull syntax and nameof reminder. Do not require unfamiliar null-forgiving syntax from the learner.
- Check: A parameter is declared JobApplication rather than JobApplication?. Does that declaration alone guarantee the method can never receive null at runtime? Explain the boundary decision.
- Prepared alternative: A private helper is called only after a public method has validated its argument. Must that helper repeat the same null guard? Discuss whether an independent boundary or invariant requires it.

### U7-F03 — Application transitions and a null guard

- Unit/topics: 7; guards, argument versus state exceptions, invariants, public boundaries.
- Type/difficulty: Implementation / Application.
- Status: Completed; 9/9 tests pass. Review found correct transitions, pre-mutation guards, private setter and ThrowIfNull before storage; no functional defects. No learner code changed.
- Sources: sections 4–10, 21–22, 30.
- Starter/specification: [JobApplication.cs](src/ExerciseWorkspace/JobApplication.cs), including ApplicationInbox.
- Tests: [ApplicationWorkflowTests.cs](tests/ExerciseWorkspace.Tests/ApplicationWorkflowTests.cs), 9 cases.
- Command: `dotnet test --filter FullyQualifiedName~ApplicationWorkflowTests`
- Present: Ask the learner to implement only the methods marked in the source comments.
- Review focus: Correct transitions, unchanged state on failure, external mutation protection and the null guard before list mutation. Passing the preinitialized-state test alone is not learner evidence.
- After coding, ask one reasoning question: Why is a null argument a different failure category from calling Apply on a rejected application?

### U7-F04 — Catch ordering review

- Unit/topics: 7; exception inheritance and specific catches before general catches.
- Type/difficulty: Code review / Foundation.
- Status: Completed with correction: learner recognised unreachable catch but initially thought it would compile. Tutor clarified compiler rejection. Revisit briefly later.
- Source: section 14.
- Tests/files: Chat only; intentionally non-compiling snippet is not part of a project.
- Prerequisite reminder: FileNotFoundException derives from IOException.
- Prompt: Does this compile? Explain and suggest the smallest correction.

```csharp
try
{
    ReadConfiguration();
}
catch (IOException)
{
    Console.WriteLine("I/O failure");
}
catch (FileNotFoundException)
{
    Console.WriteLine("Configuration missing");
}
```

### U7-F05 — Is a custom exception useful?

- Unit/topics: 7; standard versus custom exceptions and meaningful handling.
- Type/difficulty: Explain + design decision / Application.
- Status: Completed; correctly explained that identical caller handling does not justify a custom exception by itself. Implementation syntax not assessed.
- Source: section 20, with sections 10 and 13.
- Tests/files: None; no custom-exception implementation is required merely for coverage.
- Teaching prerequisite: Explain deriving from Exception, a message constructor forwarding to base(message), and the section 20 criterion: a distinct type should improve caller handling or carry meaningful domain information. Use the source's worked example; do not add serialization or framework conventions.
- Prompt: In one service, all invalid application transitions receive the same response. In another, callers must specifically distinguish a policy-blocked transition from unrelated invalid operations. In which case could a dedicated exception type be useful, and why?
- Follow-up only if needed: Is renaming an ordinary argument error with a domain-specific exception automatically an improvement?

### U7-F06 — Validation and expected outcomes

- Unit/topics: 7; validation boundaries, expected absence, meaningful catches.
- Type/difficulty: Code review / Application.
- Status: Completed; correctly identified broad-catch information loss and ambiguity between not-found and infrastructure failure.
- Sources: sections 3–5, 13, 19, 22.
- Tests/files: Chat only; assume Find returns JobApplication? and may throw IOException on storage failure.
- Prompt: The caller wants null only when an ID does not exist. Review this wrapper: what distinct situations does it collapse, and where should invalid input and storage failure be handled?

```csharp
JobApplication? FindApplication(Guid id)
{
    try
    {
        return repository.Find(id);
    }
    catch (Exception)
    {
        return null;
    }
}
```

- Explain Guid syntax directly if needed; do not test an unrelated API. No broad repository design assignment is intended.

### U7-F07 — Own and dispose a file reader

- Unit/topics: 7; synchronous disposal, resource ownership, propagation.
- Type/difficulty: Implementation / Application.
- Status: Completed; 4/4 tests pass. Reader ownership, disposal on all exits and exception propagation correct by review. Learner reports task too easy; avoid another similarly narrow coding task.
- Sources: sections 23–28 and section 30 Resource-management practice.
- Starter/specification: [ApplicationTextFile.cs](src/ExerciseWorkspace/ApplicationTextFile.cs).
- Tests: [ApplicationTextFileTests.cs](tests/ExerciseWorkspace.Tests/ApplicationTextFileTests.cs), 4 cases.
- Command: `dotnet test --filter FullyQualifiedName~ApplicationTextFileTests`
- Prerequisite: StreamReader(path) and ReadToEnd are explained directly in the source comments. This is the actual source exercise; no leaveOpen or fake disposable domain resource.
- Review focus: Reader disposal on every exit path; no broad catch or swallowed failure. Tests check text, success-path file release and missing-file propagation. Code review must additionally verify cleanup if reading itself throws; passing tests alone does not prove that case.
- After coding, one question: If ReadToEnd throws, what ensures reader disposal before the exception reaches the caller?

### U7-F08 — Rethrowing without losing diagnostics

- Unit/topics: 7; throw versus throw ex, cleanup on propagation.
- Type/difficulty: Debugging / Foundation.
- Status: Completed; independently identified throw ex as losing the original stack trace; bare throw reminder confirmed.
- Sources: sections 15–16.
- Tests/files: Chat only; Log is a supplied logging method.
- Prompt: This code intends to log and preserve the original failure location. Identify the problematic line and the smallest correction; explain what happens to finally.

```csharp
try
{
    ImportData();
}
catch (IOException ex)
{
    Log(ex.Message);
    throw ex;
}
finally
{
    Console.WriteLine("Attempt finished");
}
```

- Prepared alternative: If the logging layer cannot recover and has no useful context to add, is this catch needed at all?

### U7-F09 — Introductory asynchronous disposal

- Unit/topics: 7; IAsyncDisposable and await using.
- Type/difficulty: Explain + prediction / Foundation, recognition depth only.
- Status: Introduced; recognises await using. Learner questioned whether synchronous disposal is immediate and why async disposal is not always preferable. Explained cleanup completion versus blocking, pending I/O and capability/context requirements. Do not claim understanding settled; revisit briefly next study session and in Unit 9.
- Source: section 29. Defer detailed async mechanics to Unit 9.
- Tests/files: None; no async source exercise or new dependencies.
- Teaching prerequisite: Explain Dispose/using versus DisposeAsync/await using. DisposeAsync returns ValueTask; the learner need only recognise the signature here. Explain that await using awaits asynchronous cleanup on leaving the scope, including exceptional exits. Use the source's CreateResourceAsync example; assume it returns the indicated resource type.
- Check: A resource implements only IAsyncDisposable. Is ordinary using sufficient, and which cleanup syntax matches its contract?
- Follow-up if needed: Why might an API require cleanup that can itself wait for I/O? Do not require implementing DisposeAsync or Task.Run.

### U7-F10 — Integrated exit review

- Unit/topics: 7; domain invariants, ownership, failure categories, GC versus disposal.
- Type/difficulty: Integrated implementation / Professional.
- Status: Completed with targeted guidance on 2026-11-10; 13/13 tests pass. Learner separated batch validation and narrowed catch scope after explanation. Review found no functional defects; no learner code edited. First review was 9/13 and exposed validation timing/callback exception origin gaps; revisit with fresh examples.
- Sources: sections 3–5, 9, 12–19, 21–28; completed LoadText reused; Unit 4/5 collection/delegate knowledge interleaved.
- Starter/specification: [ApplicationBatchImporter.cs](src/ExerciseWorkspace/ApplicationBatchImporter.cs), concise behavioural requirements in source comments.
- Tests: [ApplicationBatchImporterTests.cs](tests/ExerciseWorkspace.Tests/ApplicationBatchImporterTests.cs), 13 cases.
- Command: `dotnet test --filter FullyQualifiedName~ApplicationBatchImporterTests`
- Preparation verification: Project builds; 13 cases fail on unimplemented starter as expected. No learner code or existing tests modified. Real temporary files are created/cleaned by tests; no packages needed.
- Review focus: Boundary validation before side effects, ordering/counts, distinction between source read failures and same-type callback failures, original exception propagation and reuse of correctly owned file readers. Requirements leave control flow and helpers to the learner.
- No automatic retries, asynchronous implementation, parsers, rollback mechanism or custom exception hierarchy is required.
- Optional exit prompts, only if needed after code review:
  1. A caller supplies null to ApplicationInbox.Add, then separately calls Apply on a rejected object. Classify each failure and explain who protects each contract.
  2. A method returns an open stream and documents that ownership transfers to its caller. Where should disposal occur, and does disposal immediately reclaim managed memory?
  3. Your LoadText method cannot meaningfully recover from a read failure. Explain its validation/cleanup/propagation responsibilities without adding a broad catch.
- If a concept remains weak, select its prepared alternative in F02/F05/F08 and record a later review; do not restart the unit or add an unbounded series of questions.

## Completion and review

- Complete this initial Unit 7 practice when the two coding tasks are reviewed and remaining topic groups have actual assessment evidence; mark any unresolved subtopics for review rather than inventing mastery.
- Topic coverage remains learner-reported prior coverage until evidence is added. Preparation itself changes no mastery scores.
- Ask brief reasoning checks as needed; no requirement to bring every topic to 4/4 before moving to Unit 8.
- At a natural pause, record outcomes and review dates in PROGRESS.md. Review next study session, then roughly +3/+7/+21 days adjusted to performance.
- Before the next unit, prepare its batch between teaching sessions.

## Preparation verification — 2026-11-09

- Reused the existing .NET solution and test project; no new packages or projects.
- One full `dotnet test ModernCSharp.slnx --no-restore` pass built both projects and discovered 36 cases.
- All 23 pre-existing cases passed. Of 13 new cases, the supplied initial-state check passed and 12 failed on NotImplementedException, as expected for unfinished starters.
- Inspected the test results: no other failure cause and no regression in existing tests.
- Result file: TestResults/Unit7Preparation/unit7-preparation.trx (ignored generated output).
- Expected current state: the full suite remains red until the two learner assignments are implemented. This is not an infrastructure error or assessment result.
- During teaching, run each task's filtered command from its source comments when ready; do not rerun the whole suite between conversational answers.

## Activities already used

| ID | Unit/topics | Type / difficulty | Source / tests | Status |
|----|-------------|-------------------|----------------|--------|
| U7-R1 | 2/7: invariants, exceptions, disposal | Retrieval / Foundation | Unit 7 warm-up and sections 2–10, 15, 23–28; PROGRESS.md evidence | Completed; exact exception names needed reminders |
| U7-C1 | 7: guards and state preservation | Implementation / Application | src/ExerciseWorkspace/SalaryRange.cs; SalaryRangeTests.cs | Completed with targeted guidance; 12 cases passed at review |
| U7-R2 | 7: resource ownership and scope | Prediction / Foundation | Unit 7 sections 23–28; chat stream-lifetime examples | Completed; conceptual evidence recorded |
| U7-C2 | 7: reader/stream ownership | Debugging / Application | src/ExerciseWorkspace/TextPreview.cs; TextPreviewTests.cs | Set aside; stream tests pass but reader disposal remains incomplete. leaveOpen API focus exceeded desired curriculum scope |
| U7-R3 | 7: handling, expected absence, finally, filters | Prediction / Foundation | Unit 7 sections 12–19; chat examples | Completed |
| U7-C3 | 7: catch filters, propagation, finally | Implementation / Application | src/ExerciseWorkspace/ImportAttempt.cs comments; ImportAttemptTests.cs | Completed; four cases passed on 2026-11-09; no functional review findings |

## Previous checkpoint constraints

- This batch has now been prepared from PROGRESS.md and Unit 7 source sections.
- Revisit exception-validation precedence, filters and finally briefly with fresh examples.
- Remaining unassessed Unit 7 topics include custom exceptions, null guards and asynchronous disposal; specific catch ordering also remains unassessed within a partially assessed topic.
- Keep practical tasks focused on source material. Supply incidental API details directly.
- Put concise coding requirements, essential reminders and test commands in the learner's C# file comments. Use this file for metadata and references.
- Present one activity at a time; avoid tool calls between short theory answers.
- Preserve learner code. Do not silently complete TextPreview or require it to be finished before moving on.
