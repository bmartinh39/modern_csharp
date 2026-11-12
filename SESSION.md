# Unit 9 — Async C#: intensive practice

Status: Teaching in progress; U9-07 reviewed on 2026-11-12, 10/10 targeted tests pass with guidance.
Current: U9-08 next. Session checkpoint: sessions/Unit9-2026-11-12.md.
Consolidation: correctly predicted early exit without joining recruiter; throwing versus
thread suspension needed correction. Briefly revisit next session, then continue U9-08.
Working evidence: Correct workload distinction, variance direction, ownership, dependency
order, concurrency-capacity risks, cancellation forwarding/cooperation, WhenAll waiting
and failure-versus-absence reasoning. Task/result distinction, synchronous prefix and
Task.Run needed explanation; completed-await and missing-await predictions correct after
support. Revisit these through implementation; no independent coding competence claimed.
Source: material/9 Async C# 40dea85eec9180be9557cd6855bc21cd.md.
Previous session: sessions/Unit8-2026-11-10.md (archived); learner code preserved.

## Teaching workflow

- Intensive means deeper reasoning and integrated coding, not a large quiz dump.
- Work across multiple study blocks if useful; natural pauses after U9-07 and U9-09.
- Present one question/activity at a time. Skip checks already clearly demonstrated.
- Explain prerequisites before requiring them. Give API/syntax reminders directly.
- Teach Task/await and cancellation before the service; async disposal before the
  second resource method; async iteration and WithCancellation before stream coding.
- Use chat for teaching/hints; source comments contain coding specifications and commands.
- No tool calls between ordinary theory answers. Inspect/run only the relevant tests
  at coding review. Update progress after substantial coding or at session end.
- Tests use supplied controllable tasks/probes; TaskCompletionSource, WaitAsync and
  test-probe ValueTask code are instructor infrastructure, not learner prerequisites.
- Check code for blocking/detached work and reasoning as well as passing tests.
- Retain Unit 7 async-disposal and Unit 8 variance/snapshot reviews, without restarting units.
- Do not teach advanced schedulers, Channels, custom awaiters, or exhaustive aggregate
  exception internals. No databases, HTTP services, additional projects or packages.

## Activity queue

### U9-01 — Retrieval and interleaving
Unit/topics: 9 Task and workloads; 7 ownership; 8 consumer variance.
Type/difficulty: Retrieval / Foundation. Status: Ready.
Source: sections 1, 4–6; prior review queue. Files/tests: Chat only.
Ask individually:
1. A method returns Task<string>. What do you have before awaiting it?
2. How does waiting for a database response differ from calculating a large hash?
3. A callback accepts every Animal. Can it safely be passed where Action<Dog> is expected? Why?
4. Why must an owned stream outlive the operation consuming it?
Use answers to tune explanation depth. No answers supplied until learner responds.

### U9-02 — Execution before and after await
Unit/topics: 9 synchronous prefix, incomplete/completed awaits, Task versus thread.
Type/difficulty: Explanation + prediction / Application. Status: Ready.
Source: sections 2–13. Files/tests: Chat only.
Explain task completion/result/fault/cancellation, async return types, synchronous work
before first incomplete await, continuation, and no guaranteed thread switch.
Present after explanation:
```csharp
static async Task TraceAsync(Task gate)
{
    Console.WriteLine("inside: start");
    await gate;
    Console.WriteLine("inside: end");
}
Console.WriteLine("caller: before");
var task = TraceAsync(gate);
Console.WriteLine("caller: after call");
await task;
Console.WriteLine("caller: done");
```
Assume supplied gate is incomplete and released only after caller: after call.
Ask for output order. Then separately ask what changes if gate is already complete.
Do not use Task.Delay duration as proof an await must suspend; do not promise thread IDs.

### U9-03 — Blocking, fake async and work ownership
Unit/topics: 9 I/O versus CPU, Task.Run, .Result/.Wait(), async void, forgotten awaits.
Type/difficulty: Code review / Application. Status: Ready.
Source: sections 19–24, 35–36, 44–49. Files/tests: Chat only.
Explain genuine async I/O versus thread-pool offloading, blocking scalability, Task
returning methods versus async void, and what missing await changes.
Present separately, ask for consequence and proposed correction:
- var text = File.ReadAllTextAsync(path).Result;
- await Task.Run(() => File.ReadAllTextAsync(path));
- async void SaveAsync() { await store.SaveAsync(); }
- SaveAsync(); Console.WriteLine("Saved"); (assuming Task-returning SaveAsync)
Contrast CPU-heavy hashing in a UI with request handling; do not imply Task.Run makes
CPU work cheaper or every blocking call inevitably deadlocks in ASP.NET Core.
Mention ConfigureAwait only for recognition: continuation context, not a new thread
or a universal fix for blocking. No independent mastery score from this mention.

### U9-04 — Dependencies and overlapping waits
Unit/topics: 9 sequential/concurrent operations, WhenAll, dependency boundaries.
Type/difficulty: Design + prediction / Application. Status: Ready.
Source: sections 14–18, 37–38, 51–52. Files/tests: Chat; prepares service.
Explain starting operations versus awaiting them; WhenAll joins existing tasks.
Scenario: find application, then load its company and recruiter, then build details.
Ask one at a time: what must happen first? What can overlap? For independent two-second
I/O waits, how do sequential/concurrent totals differ? When would overlap be unsafe?
Explicitly include source capacity/shared dependencies and 50,000-request fan-out.
Concurrency limits are a design discussion, not an unintroduced semaphore assignment.
Optional experiment: learner uses Task.Delay and Stopwatch in disposable scratch code;
provide API reminders. No timing thresholds in correctness tests.

### U9-05 — Cooperative cancellation
Unit/topics: 9 CancellationToken/Source, propagation, pre-cancellation, task state.
Type/difficulty: Explanation + debugging / Application. Status: Ready.
Source: sections 28–34, 49. Files/tests: Chat; prepares service.
Explain ownership: caller owns/disposes source, callee observes token; Cancel requests
rather than kills work. Show using var cts = new CancellationTokenSource(), cts.Token,
cts.Cancel(), ThrowIfCancellationRequested and Task.Delay(milliseconds, token).
Then present:
```csharp
static async Task LoadAsync(CancellationToken token)
{
    await Task.Delay(10000);
}
```
Ask why cancellation has no effect. Separately: if a downstream API ignores its token,
does cancelling guarantee all work stops immediately? Distinguish cancelled from faulted.
Discuss service's explicit pre-cancellation policy; accepting a token alone is insufficient.

### U9-06 — Failures while other work is still running
Unit/topics: 9 async exceptions/WhenAll; 7 propagation and catch boundaries.
Type/difficulty: Explanation + prediction / Professional. Status: Ready.
Source: sections 25–27, 51–52. Files/tests: Chat; prepares service.
Explain await surfaces task failures; try must include the await. Distinguish a faulted
returned task from an ordinary non-async method throwing before it returns a task.
Service sources promise returned tasks, avoiding an unintroduced synchronous-start policy.
Explain WhenAll waits for every supplied task to finish; one fault does not cancel siblings.
Ask: company task faults while recruiter task remains pending. Has the combined task
finished? Must recruiter automatically stop? Ask individually. Follow with two faults:
where can diagnostic code inspect all failures? Teach combined Task.Exception/individual
task state if needed; aggregate internals are recognition, not required implementation.
No broad catches converting cancellation/storage failure into expected absence.

### U9-07 — Application details service
Unit/topics: 9 task composition, dependency order, cancellation, exception propagation.
Type/difficulty: Implementation / Professional. Status: Completed with stepwise guidance; 10/10 targeted tests pass on 2026-11-12. Consolidation discussed; failure versus thread suspension needs retrieval. Cancellation API, async return semantics and typed-result retrieval needed support; no learner code edited by Codex.
Source: sections 51–52, supported by U9-02–06.
Starter/spec: src/ExerciseWorkspace/Unit9/ApplicationDetailsService.cs.
Tests: tests/ExerciseWorkspace.Tests/Unit9/ApplicationDetailsServiceTests.cs (10 cases).
Command: dotnet test --filter FullyQualifiedName~ApplicationDetailsServiceTests
Requirements live in source comments; supplied records/interfaces avoid changing learner
Unit 7/8 types. Learner writes guards, control flow and composition. Do not reveal solution.
Review code for blocking/detached work and failure observation; tests check observable order,
identity, expected absence, token forwarding, overlap, completion and cancellation.
After review ask why WhenAll does not provide a transaction or sibling cancellation.
Natural pause; record actual support needed before continuing.

### U9-08 — Debug an async resource lifetime
Unit/topics: 9 lifetime across awaits; 7 using/ownership/exception paths.
Type/difficulty: Prediction + debugging / Professional. Status: Ready.
Source: section 50 and Unit 7 ownership; requires U9-02 and U9-06.
Starter/spec: AsyncResourceWorkflow.ReadOwnedAsync in src/ExerciseWorkspace/Unit9/AsyncResourceWorkflow.cs.
Tests: tests/ExerciseWorkspace.Tests/Unit9/AsyncResourceWorkflowTests.cs (4 stream cases).
Command: dotnet test --filter "FullyQualifiedName~AsyncResourceWorkflowTests&FullyQualifiedName~Stream"
Before edits, ask when the current using scope ends relative to the pending callback task.
Then learner fixes the starter. Explain Func<Stream> and Task<int> delegate signatures if rusty;
no StreamReader-specific API knowledge required. Review normal/fault/cancel and synchronous
callback throws. Use the full class filter only after U9-09 implementation.

### U9-09 — Await cleanup, not just processing
Unit/topics: 9 async lifetime; 7 IAsyncDisposable, await using, disposal versus GC.
Type/difficulty: Explanation + implementation / Professional. Status: Ready.
Source: section 50; Unit 7 async disposal and existing high-priority review item.
Starter/spec: AsyncResourceWorkflow.ProcessOwnedAsync in the same source file.
Tests: AsyncResourceWorkflowTests.cs (4 async-cleanup cases; 8 class cases total).
Command: dotnet test --filter FullyQualifiedName~AsyncResourceWorkflowTests
First explain IAsyncDisposable.DisposeAsync, await using and that synchronous cleanup
may take time; async cleanup may suspend, not guarantee suspension or instant completion.
Use a tiny unrelated instructor example, never implement the active method in chat.
Prediction: processing finishes, cleanup is still pending. Should the owning method's
returned task have finished? Then learner implements, preserving exception/result flow.
Tests deliberately hold cleanup pending and cover success, fault, cancellation and
synchronous callback throws. No custom ValueTask implementation required from learner.
Natural pause and progress review.

### U9-10 — LINQ returns tasks, and enumeration can start work
Unit/topics: 9 async LINQ/loops; 5 deferred execution and multiple enumeration.
Type/difficulty: Code review / Professional. Status: Ready.
Source: sections 37–39; Unit 5 enumeration. Files/tests: Chat only.
Explain Select(async ...) yields tasks, not completed results; execution deferred until
query enumeration and repeating enumeration may invoke the selector again.
Then present, assuming LoadAsync returns Task<string>:
```csharp
var tasks = ids.Select(id => LoadAsync(id, token));
await Task.WhenAll(tasks);
foreach (var task in tasks)
    Console.WriteLine(await task);
```
Ask whether each ID is necessarily loaded only once; learner proposes correction.
Do not reveal fix before response. Follow with how the design changes for thousands
of inputs or strict ordering. Revisit shallow collection snapshot versus awaiting results.

### U9-11 — Async streaming and ordered processing
Unit/topics: 9 IAsyncEnumerable, await foreach, iterators, cancellation and enumerator lifetime.
Type/difficulty: Explanation + implementation / Application/Professional. Status: Ready.
Source: sections 40–43; U9-05/08/09 prerequisites.
Starter/spec: src/ExerciseWorkspace/Unit9/ApplicationStreamConsumer.cs.
Tests: tests/ExerciseWorkspace.Tests/Unit9/ApplicationStreamConsumerTests.cs (6 cases).
Command: dotnet test --filter FullyQualifiedName~ApplicationStreamConsumerTests
Before coding explain Task<List<T>> versus async sequence, deferred iterator execution,
yield return and early enumeration exit. Use the source's small async numbers example;
writing a producer is not this assignment. Explain await foreach, await in its body and
WithCancellation(token) explicitly; no guessing the extension method name.
Explain await foreach releases its enumerator on early exit/fault; cancellation must be
observed by producer/callback and is not forced by an interface alone.
One check: can processing the first item start before the final item is available?
Then learner implements the consumer; requirements remain in source comments.
Tests verify order, callback completion before next request, both failure origins,
cancellation forwarding and enumerator disposal. No buffering/all-at-once concurrency.

### U9-12 — Transfer, review and next step
Unit/topics: 9 integrated async reasoning; Units 7/8 interleaving as needed.
Type/difficulty: Exit/design / Professional. Status: Ready.
Source: sections 54–55 and observed gaps. Files/tests: Chat; existing implementation references only.
Choose up to three short fresh questions, individually:
- A web caller no longer wants results. Who requests cancellation, who must observe it,
  and what happens to resources already acquired?
- A query returns Task<List<T>>: does async imply it streams or uses another thread?
- Two independent lookups fault: does joining them roll back effects or stop other work?
Revisit uncertain variance/snapshot points only briefly. Review code/testing results
versus reasoning separately; preparation/starter passes never count as learner completion.
Record scores and review intervals from actual evidence. When adequate fluency is
shown, recommend moving to ASP.NET Core; do not chase exhaustive language mastery.

## Verification and continuation

One full solution test pass on 2026-11-10: both projects compile, 81 cases discovered.
57 existing cases pass unchanged. Of 24 Unit 9 cases, two deliberate-debugging starter
cases pass and 22 fail as expected (unimplemented methods/constructor guard and premature
disposal). The two starter passes do not demonstrate learner performance.
No packages, services, additional projects or learner solutions were added.
After every meaningful coding review, record actual outcomes and hints in PROGRESS.md.
Initial reviews: next study session, then approximately +3/+7/+21 days as appropriate.

Next chat: brief failure-versus-suspension retrieval, then U9-08 in teaching mode;
one activity at a time. Preserve completed service and remaining starters.
