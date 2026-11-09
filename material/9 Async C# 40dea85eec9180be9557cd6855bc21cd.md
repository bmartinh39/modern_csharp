# 9. Async C#

Async programming is one of the most important parts of modern .NET.

You will encounter it constantly in:

- ASP.NET Core;
- Entity Framework Core;
- HTTP clients;
- file and stream APIs;
- database access;
- cloud SDKs;
- message queues;
- external services.

The goal of this unit is not simply to learn the syntax:

```
async
await
Task
```

The important goal is to understand the execution model well enough that you can answer questions such as:

- What is actually happening while an operation is awaited?
- Does `await` create another thread?
- When should operations run sequentially?
- When can they run concurrently?
- What does `Task` actually represent?
- How do exceptions behave in async code?
- How do we cancel work correctly?
- Why is `.Result` often dangerous?
- What is `IAsyncEnumerable<T>` for?

This matters directly before ASP.NET Core, because async code there is normal rather than exceptional.

---

# 1. Retrieval warm-up

Short answers.

### 1.1

What happens to the current method when it calls another method normally?

For example:

```
var result = Calculate();
```

Does the caller continue before `Calculate()` returns?

### 1.2

What is the difference between CPU work and I/O work?

For example, classify:

```
sorting 10 million numbers
waiting for a database query
compressing an image
waiting for an HTTP response
reading a file from disk
calculating a hash
```

### 1.3

From Unit 7:

Why would holding a database connection or file resource longer than necessary be undesirable?

### 1.4

What do you think this type represents?

```
Task<string>
```

Do not worry if the answer is incomplete.

---

# 2. The problem async programming solves

Consider a synchronous operation:

```
var text = File.ReadAllText("applications.json");
```

Conceptually:

```
start read
   ↓
wait for storage
   ↓
data arrives
   ↓
continue
```

During the waiting period, there may be little useful CPU work for the calling code to perform.

The operation is waiting on **I/O**.

The same thing happens with:

```
database query
HTTP request
network stream
file access
external API
```

In server software this becomes especially important.

Imagine an ASP.NET Core server handling thousands of requests.

If every request occupied a thread merely waiting for a database or network response, the server would waste threads on work that is not actually using the CPU.

Async programming allows the code to express:

> Start this operation, and when it cannot make progress because it is waiting for I/O, don't block the executing thread unnecessarily.
> 

That is the central motivation.

---

# 3. Blocking

Consider:

```
Thread.Sleep(5000);
```

The executing thread is blocked for five seconds.

It cannot do other work.

Conceptually:

```
thread
  ↓
work
  ↓
SLEEPING / BLOCKED
  ↓
5 seconds pass
  ↓
continue
```

For a desktop application, blocking the UI thread could freeze the interface.

For a server, blocking many request-handling threads reduces scalability.

Async I/O gives us a different model.

---

# 4. `Task`

The central type in modern C# async code is:

```
Task
```

A `Task` represents an asynchronous operation.

It is useful to think of it as:

> **an object representing work that may not have completed yet.**
> 

For example:

```
Task delay = Task.Delay(1000);
```

`delay` represents an operation that will complete later.

A `Task` does not necessarily mean:

> a new thread is running.
> 

That is a very important misconception.

A task represents **completion of an operation**.

How that operation is implemented is a separate question.

---

# 5. `Task<T>`

Sometimes an asynchronous operation eventually produces a value.

Then we use:

```
Task<T>
```

For example:

```
Task<string>
```

means approximately:

> an asynchronous operation that will eventually complete and produce a `string`.
> 

Likewise:

```
Task<JobApplication>
```

means:

> an operation that eventually produces a `JobApplication`.
> 

Compare synchronous:

```
string LoadText()
```

with asynchronous:

```
Task<string> LoadTextAsync()
```

The synchronous method directly returns:

```
string
```

The async method returns:

```
Task<string>
```

because the string may not exist yet when the method initially returns control.

---

# 6. First mental model of `Task`

Suppose:

```
Task<string> task =
    LoadTextAsync();
```

You do not yet necessarily have:

```
the string
```

You have:

```
an object representing the operation that will eventually produce the string
```

Conceptually:

```
Task<string>
    ↓
Pending
    ↓
eventually
    ↓
Completed
    ↓
string result available
```

A task can also complete because:

- the operation succeeded;
- it failed;
- it was cancelled.

We'll return to those states later.

---

# 7. `await`

`await` is how we asynchronously wait for a task to complete.

Example:

```
string text =
    await LoadTextAsync();
```

Read this approximately as:

> If the task has not completed, suspend this method's continuation without unnecessarily blocking the current thread. Resume when the task completes, then give me its result.
> 

The result of awaiting:

```
Task<string>
```

is:

```
string
```

So:

```
Task<string> task = LoadTextAsync();

string text = await task;
```

---

# 8. `await` does not mean "start another thread"

This misconception is extremely common.

Consider:

```
var response =
    await httpClient.GetAsync(url);
```

You should not automatically imagine:

```
main thread
    ↓
spawn another thread
    ↓
that thread waits for network
```

For genuine asynchronous I/O, there may be no thread sitting there waiting for the network response.

Very roughly:

```
start network operation
        ↓
OS/network subsystem handles waiting
        ↓
thread can return to other work
        ↓
network operation completes
        ↓
continuation becomes ready to run
```

The exact implementation is more sophisticated, but this is the useful mental model.

Async I/O is primarily about **not occupying a thread while waiting**.

---

# 9. `async`

To use `await` naturally, a method is usually marked:

```
async
```

Example:

```
static async Task<string> LoadTextAsync(
    string path)
{
    var text =
        await File.ReadAllTextAsync(path);

    return text;
}
```

Notice the return type:

```
Task<string>
```

but inside the method we return:

```
string
```

The compiler transforms the async method so that the caller receives a `Task<string>` representing its eventual completion.

---

# 10. Async return types

The most common async return types are:

```
Task
Task<T>
```

Use:

```
Task
```

when there is no result value.

Example:

```
static async Task SaveAsync()
{
    await ...
}
```

Use:

```
Task<T>
```

when the operation eventually produces a value.

Example:

```
static async Task<JobApplication>
    LoadAsync()
{
    ...
}
```

You will also encounter:

```
ValueTask
ValueTask<T>
```

but we do not need to use them routinely at this stage.

They are specialised tools and should not replace `Task` by default.

---

# 11. Why async method names often end in `Async`

.NET convention commonly uses:

```
Async
```

as a suffix:

```
ReadAsync()
SaveAsync()
GetByIdAsync()
SendAsync()
```

This makes asynchronous APIs immediately recognisable.

For example:

```
GetApplication()
```

versus:

```
GetApplicationAsync()
```

The suffix is convention rather than special compiler syntax.

---

# 12. First prediction check

Consider:

```
static async Task<string> GetMessageAsync()
{
    await Task.Delay(1000);

    return "Finished";
}
```

And:

```
Task<string> task =
    GetMessageAsync();
```

Immediately after this line:

```
Task<string> task =
    GetMessageAsync();
```

do we necessarily already have the string `"Finished"`?

What do we actually have?

Then:

```
string message = await task;
```

what is the type of `message`?

---

# 13. Control flow around `await`

Consider:

```
Console.WriteLine("A");

await Task.Delay(1000);

Console.WriteLine("B");
```

The logical order is still:

```
A
B
```

`await` does not mean:

> ignore the operation and immediately execute the next line.
> 

Instead, the remainder of the method becomes a **continuation**.

Conceptually:

```
execute A
   ↓
start / inspect task
   ↓
task incomplete?
   ↓
suspend this async method
   ↓
task completes
   ↓
resume continuation
   ↓
execute B
```

So asynchronous does not mean random execution order.

The method still has understandable control flow.

---

# 14. Sequential async operations

Consider:

```
var company =
    await LoadCompanyAsync();

var applications =
    await LoadApplicationsAsync();
```

These operations are sequential.

Conceptually:

```
start company load
wait for completion
        ↓
start applications load
wait for completion
```

If each takes two seconds, the total may be approximately:

```
2 seconds + 2 seconds = 4 seconds
```

That may be exactly what we want.

For example, the second operation may depend on the first:

```
var company =
    await LoadCompanyAsync();

var applications =
    await LoadApplicationsAsync(company.Id);
```

Here concurrency would not make sense because we need `company.Id` first.

---

# 15. Async does not automatically mean concurrent

This is important.

This:

```
var first =
    await GetFirstAsync();

var second =
    await GetSecondAsync();
```

is asynchronous, but normally still **sequential**.

The first operation is awaited before the second one starts.

So:

```
async
```

does not automatically mean:

```
parallel
```

or:

```
concurrent
```

These are separate decisions.

---

# 16. Starting independent operations before awaiting

Suppose the operations are independent:

```
GetCompaniesAsync()
GetApplicationsAsync()
```

We can start both:

```
Task<List<Company>> companiesTask =
    GetCompaniesAsync();

Task<List<JobApplication>> applicationsTask =
    GetApplicationsAsync();
```

Neither has been awaited yet.

Then:

```
var companies =
    await companiesTask;

var applications =
    await applicationsTask;
```

Now both operations had an opportunity to progress concurrently.

Conceptually:

```
start companies ──────────┐
                          │
start applications ───────┤
                          │
both waiting/progressing  │
                          ↓
await results
```

This can reduce total waiting time when the operations are independent I/O operations.

---

# 17. `Task.WhenAll`

When several tasks should all complete, C# provides:

```
Task.WhenAll(...)
```

For example:

```
var companiesTask =
    GetCompaniesAsync();

var applicationsTask =
    GetApplicationsAsync();

await Task.WhenAll(
    companiesTask,
    applicationsTask);
```

Then:

```
var companies =
    await companiesTask;

var applications =
    await applicationsTask;
```

All tasks are allowed to progress before we wait for the combined completion.

Another form:

```
var results =
    await Task.WhenAll(tasks);
```

can directly return the results when the tasks all produce compatible values.

---

# 18. Sequential versus concurrent reasoning

Suppose:

```
await OperationAAsync();
await OperationBAsync();
```

and each takes approximately two seconds.

Sequential total:

```
~4 seconds
```

Now:

```
var a = OperationAAsync();
var b = OperationBAsync();

await Task.WhenAll(a, b);
```

If they are independent and both mostly spend time waiting on I/O:

```
~2 seconds
```

may be possible.

But concurrency is not automatically better.

You must ask:

- Are the operations independent?
- Does one depend on the other's result?
- Can the external service handle concurrent requests?
- Is there a rate limit?
- Is the database operation safe to run concurrently?
- Are we creating too much simultaneous work?

The engineering question is:

> **Can these operations overlap safely and usefully?**
> 

---

# 19. Concurrency is not parallel CPU execution

We need another distinction.

**Concurrency** means multiple operations are in progress during overlapping periods.

**Parallelism** means multiple computations are literally executing at the same time, usually on different CPU cores.

Async I/O often provides concurrency without requiring parallel CPU execution.

For example:

```
HTTP request A waiting for network
HTTP request B waiting for network
```

Both operations are in progress.

But neither may currently be using a CPU core.

This is why async is so valuable for I/O-heavy server applications.

---

# 20. CPU-bound work

Suppose we have:

```
CalculateHugePrimeNumber();
```

The CPU is actively doing work.

Making the method:

```
async
```

does not magically make that computation non-blocking.

This:

```
async Task<int> CalculateAsync()
{
    return CalculateHugePrimeNumber();
}
```

contains no meaningful asynchronous operation.

The CPU work still runs synchronously.

For CPU-bound workloads, parallelism or offloading work may sometimes be appropriate.

For ASP.NET Core, however, casually wrapping CPU work in:

```
Task.Run(...)
```

is usually not the same scalability strategy as using real async I/O.

The main async use case we're targeting is:

```
I/O-bound work
```

---

# 21. Async I/O versus `Task.Run`

Compare:

```
await File.ReadAllTextAsync(path);
```

with:

```
await Task.Run(
    () => File.ReadAllText(path));
```

The first uses an async API designed for asynchronous I/O.

The second takes synchronous blocking work and schedules it onto a thread-pool thread.

They are not equivalent in design.

For genuine I/O, prefer the async API when available.

A useful rule:

```
I/O-bound
    → use naturally asynchronous APIs

CPU-bound
    → may require a different strategy
```

Do not reach for `Task.Run` simply because you want an `await`.

---

# 22. Async all the way

Suppose:

```
static async Task<string> LoadAsync()
{
    return await File.ReadAllTextAsync("data.txt");
}
```

Then a caller should generally also be asynchronous:

```
var text =
    await LoadAsync();
```

And its caller may also become async.

This leads to the common principle:

> **async tends to propagate through the call chain.**
> 

This is usually good.

Trying to stop that propagation by blocking on async code often causes problems.

---

# 23. `.Result` and `.Wait()`

A task can technically be blocked on:

```
var result = task.Result;
```

or:

```
task.Wait();
```

These synchronously block the calling thread until the task completes.

That defeats much of the purpose of async.

In some application environments, sync-over-async patterns can also contribute to deadlocks.

Modern .NET server code should normally prefer:

```
await task;
```

instead of:

```
task.Result
task.Wait()
```

The simple mental rule is:

> If you're already in async code, await the task instead of synchronously blocking on it.
> 

---

# 24. Why blocking async can hurt scalability

Imagine an ASP.NET Core request:

```
var result =
    GetFromDatabaseAsync().Result;
```

The request thread sits blocked.

Meanwhile the database operation is waiting on external I/O.

So we have:

```
thread
   ↓
blocked waiting
   ↓
database
```

With proper async:

```
var result =
    await GetFromDatabaseAsync();
```

the thread is not unnecessarily occupied during the I/O wait.

That difference becomes significant under load.

---

# 25. Exceptions in async methods

Suppose:

```
static async Task<string> LoadAsync()
{
    await Task.Delay(100);

    throw new InvalidOperationException(
        "Something failed.");
}
```

And:

```
await LoadAsync();
```

The exception surfaces when the task is awaited.

So normal exception handling works:

```
try
{
    var result =
        await LoadAsync();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
}
```

This is one reason `await` makes async code relatively natural to reason about.

The failure appears at the await point.

---

# 26. A task can represent failure

Remember:

```
Task
```

does not merely represent:

```
running / finished
```

A task can complete in different ways:

```
success
faulted
cancelled
```

If it faults, awaiting it rethrows the relevant exception.

This means an async API can communicate failure through the same exception model you're already familiar with.

---

# 27. Exceptions and `Task.WhenAll`

Suppose:

```
var first =
    OperationAAsync();

var second =
    OperationBAsync();

await Task.WhenAll(first, second);
```

If one or more operations fail, the combined await fails.

The individual tasks still contain their own completion/failure state.

For now, the important rule is:

> Starting several tasks concurrently does not make their failures disappear.
> 

Concurrency changes execution overlap, not the fundamental requirement to handle failure properly.

We do not need to turn this section into an exhaustive study of aggregate exception internals.

---

# 28. Cancellation

Async operations sometimes need to stop because the caller no longer cares about the result.

Examples:

- user cancels a request;
- browser disconnects;
- timeout expires;
- application shuts down;
- a long-running operation is superseded.

.NET represents cooperative cancellation using:

```
CancellationToken
```

---

# 29. `CancellationToken`

A method can accept:

```
CancellationToken cancellationToken
```

For example:

```
static async Task<string> LoadAsync(
    string path,
    CancellationToken cancellationToken)
{
    return await File.ReadAllTextAsync(
        path,
        cancellationToken);
}
```

The token communicates:

> The caller may request that this operation stop.
> 

The important word is **request**.

Cancellation is cooperative.

The token does not generally reach into arbitrary code and forcibly kill it.

The operation has to observe or pass along the cancellation request.

---

# 30. Propagating cancellation

Suppose:

```
static async Task<string> LoadDataAsync(
    CancellationToken cancellationToken)
{
    return await SomeApiAsync(
        cancellationToken);
}
```

A good async call chain usually passes the token downward.

Conceptually:

```
caller
  ↓ cancellation token
service
  ↓ cancellation token
HTTP/database/file API
```

This lets cancellation propagate through the work.

You will see this constantly in ASP.NET Core.

---

# 31. Checking cancellation manually

For code that performs work in a loop, we can check:

```
cancellationToken.ThrowIfCancellationRequested();
```

Example:

```
foreach (var application in applications)
{
    cancellationToken
        .ThrowIfCancellationRequested();

    Process(application);
}
```

If cancellation has been requested, this throws an:

```
OperationCanceledException
```

associated with cooperative cancellation.

---

# 32. `CancellationTokenSource`

A cancellation token is normally obtained from a:

```
CancellationTokenSource
```

Example:

```
using var source =
    new CancellationTokenSource();

CancellationToken token =
    source.Token;
```

Later:

```
source.Cancel();
```

requests cancellation.

The relationship is:

```
CancellationTokenSource
    → can request cancellation

CancellationToken
    → observes cancellation request
```

Many frameworks give you a token directly, so you won't always create the source yourself.

ASP.NET Core is a good example.

---

# 33. Cancellation is not failure in the ordinary sense

This distinction is useful.

If the user cancels an operation, the code may not have "failed" because of a programming or infrastructure error.

It was intentionally stopped.

So:

```
fault
```

and:

```
cancellation
```

are represented separately in task state.

Your higher-level logic may treat them differently.

---

# 34. Cancellation check

Suppose:

```
static async Task LoadAsync(
    CancellationToken token)
{
    await Task.Delay(
        TimeSpan.FromSeconds(10),
        token);
}
```

And the token is cancelled after one second.

Should the caller expect to wait the full ten seconds?

What is the purpose of passing the token into `Task.Delay` rather than merely accepting it as a parameter and ignoring it?

---

# 35. `async void`

Most async methods should return:

```
Task
```

or:

```
Task<T>
```

There is also:

```
async void
```

but this should normally be reserved for event-handler scenarios where the framework contract requires `void`.

Why avoid it?

Because callers cannot:

```
await
```

a `void`.

So they cannot naturally:

- wait for completion;
- compose it with other tasks;
- handle its exceptions through ordinary task-based flow.

Prefer:

```
async Task SaveAsync()
```

over:

```
async void SaveAsync()
```

unless an event-handler API specifically requires `void`.

---

# 36. Forgetting to await

Consider:

```
SaveAsync();
Console.WriteLine("Saved");
```

This starts the async method, but we do not wait for its completion.

`"Saved"` may be printed before saving actually finishes.

The compiler often warns about this.

Normally:

```
await SaveAsync();

Console.WriteLine("Saved");
```

is correct when the second operation depends on successful completion.

Fire-and-forget behaviour should be deliberate, not accidental.

---

# 37. Sequential loops

Suppose:

```
foreach (var company in companies)
{
    await LoadJobsAsync(company);
}
```

This is sequential.

Each company's jobs are loaded before the next operation starts.

That may be intentional.

Conceptually:

```
Company A
   ↓ complete
Company B
   ↓ complete
Company C
```

If each operation takes one second:

```
~3 seconds
```

---

# 38. Concurrent loops

If operations are independent:

```
var tasks =
    companies.Select(
        company => LoadJobsAsync(company));

await Task.WhenAll(tasks);
```

Now they can overlap.

Conceptually:

```
A ──────────┐
B ──────────┼── concurrent progress
C ──────────┘
```

Potentially much faster for independent I/O.

But this introduces another engineering question:

> Is unlimited concurrency safe?
> 

If `companies` contains 50,000 entries, immediately issuing 50,000 network requests may be a terrible idea.

Concurrency needs control.

We will encounter more sophisticated patterns when real applications require them.

---

# 39. Async and LINQ

A common mistake is assuming ordinary LINQ operators understand async delegates automatically.

For example:

```
applications.Select(
    async application =>
        await EnrichAsync(application));
```

The result is effectively a sequence of tasks:

```
IEnumerable<Task<...>>
```

not a sequence of completed results.

So you may then need:

```
var tasks =
    applications.Select(
        application =>
            EnrichAsync(application));

var results =
    await Task.WhenAll(tasks);
```

This is worth recognising because it appears frequently in real code.

---

# 40. `IAsyncEnumerable<T>`

So far we've dealt with:

```
Task<T>
```

which eventually produces one result.

But what if results arrive gradually?

For example:

```
database rows
network messages
large streamed dataset
```

We may not want to wait for every item before processing the first one.

.NET provides:

```
IAsyncEnumerable<T>
```

This represents:

> an asynchronous sequence of values.
> 

---

# 41. `await foreach`

An async sequence can be consumed with:

```
await foreach
```

Example:

```
await foreach (
    var application in
        StreamApplicationsAsync())
{
    Console.WriteLine(
        application.Position);
}
```

Conceptually:

```
wait for next item
    ↓
process it
    ↓
wait for next item
    ↓
process it
```

The full sequence does not necessarily need to exist in memory before processing begins.

---

# 42. Async iterators

An async iterator can look like:

```
static async IAsyncEnumerable<int>
    GenerateAsync()
{
    for (var i = 0; i < 3; i++)
    {
        await Task.Delay(500);

        yield return i;
    }
}
```

Then:

```
await foreach (
    var number in GenerateAsync())
{
    Console.WriteLine(number);
}
```

produces items over time.

This combines two concepts you've already seen:

```
async waiting
+
yield-based iteration
```

You do not need deep mastery before ASP.NET Core, but you should understand what `IAsyncEnumerable<T>` represents when you encounter it.

---

# 43. `Task<T>` versus `IAsyncEnumerable<T>`

A useful distinction:

```
Task<List<JobApplication>>
```

means approximately:

> eventually give me the whole list.
> 

Whereas:

```
IAsyncEnumerable<JobApplication>
```

means:

> asynchronously give me applications one at a time as they become available.
> 

Neither is universally superior.

Choose based on the API's semantics.

---

# 44. `ConfigureAwait`

You may encounter code such as:

```
await SomeOperationAsync()
    .ConfigureAwait(false);
```

This relates to whether the continuation attempts to resume on a captured synchronization context.

It matters particularly in certain UI and library scenarios.

For our current path into modern ASP.NET Core, you should **recognise** it but not build your async mental model around it.

ASP.NET Core does not use the old request synchronization context model that caused much of the classic `ConfigureAwait(false)` discussion.

We can revisit it if a real codebase requires it.

---

# 45. Common mistake: fake async

This:

```
static async Task<int> GetNumberAsync()
{
    return 42;
}
```

has no `await`.

There is no asynchronous operation happening.

The compiler will normally warn about that.

Sometimes an interface contract may force a task-returning method, but generally do not add:

```
async
```

just to make code appear asynchronous.

Async should represent real asynchronous work.

---

# 46. Common mistake: unnecessary `Task.Run`

This:

```
await Task.Run(
    () => httpClient.GetStringAsync(url));
```

is generally misunderstanding the model.

`HttpClient` already offers asynchronous I/O.

You normally just use:

```
await httpClient.GetStringAsync(url);
```

Putting an already asynchronous I/O operation inside `Task.Run` usually adds unnecessary scheduling.

---

# 47. Common mistake: sequentialising independent work

This:

```
var first =
    await GetFirstAsync();

var second =
    await GetSecondAsync();
```

may be correct.

But if the operations are independent, it may unnecessarily double waiting time.

Before writing several consecutive awaits, ask:

> Does operation B depend on operation A?
> 

If no, concurrency may be appropriate.

Do not mechanically parallelise everything, but recognise the choice.

---

# 48. Common mistake: accidental fire-and-forget

This:

```
SendNotificationAsync();
```

without awaiting or otherwise tracking the task can lead to:

- completion happening later than expected;
- lost exceptions;
- application shutdown before completion;
- ordering bugs.

If the work truly needs fire-and-forget semantics, it usually deserves an explicit architectural mechanism rather than an accidentally ignored task.

In server applications that often means a proper background-work mechanism.

---

# 49. Common mistake: cancellation token accepted but ignored

This looks cancellable:

```
static async Task LoadAsync(
    CancellationToken cancellationToken)
{
    await Task.Delay(10000);
}
```

but the token is never used.

So cancellation has no effect.

This is better:

```
static async Task LoadAsync(
    CancellationToken cancellationToken)
{
    await Task.Delay(
        10000,
        cancellationToken);
}
```

The token must be observed or propagated.

---

# 50. Common mistake: async without understanding lifetime

Remember Unit 7.

Suppose:

```
using var stream =
    File.OpenRead(path);

await ProcessAsync(stream);
```

The stream must remain alive until `ProcessAsync` finishes.

This is fine because:

```
await
```

keeps execution within the scope before disposal.

But returning a task while disposing the resource too early can be dangerous if the asynchronous work still needs it.

Resource lifetime and async lifetime must agree.

This connection becomes important with database contexts, streams and HTTP resources.

---

# 51. Main exercise — JobTracker async service

We will now simulate a realistic service layer before introducing ASP.NET Core.

Assume we have asynchronous data sources:

```
Task<IReadOnlyList<JobApplication>>
    LoadApplicationsAsync(
        CancellationToken cancellationToken);

Task<Company?>
    LoadCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken);

Task<RecruiterInfo?>
    LoadRecruiterAsync(
        Guid applicationId,
        CancellationToken cancellationToken);
```

Your task is to build:

```
Task<ApplicationDetails?>
    GetApplicationDetailsAsync(
        Guid applicationId,
        CancellationToken cancellationToken)
```

---

## Requirements

### 1. Load the application

Use an async call to find the requested application.

If no application exists, return:

```
null
```

This is an expected outcome, not an exception.

### 2. Load independent data concurrently

Once you have the application, suppose:

```
Company
RecruiterInfo
```

can be loaded independently.

Start both asynchronous operations before awaiting their results.

Use:

```
Task.WhenAll
```

where appropriate.

### 3. Propagate cancellation

Pass:

```
cancellationToken
```

to every async API that supports it.

Do not merely accept the token and ignore it.

### 4. Do not block

Do not use:

```
.Result
.Wait()
```

Use `await`.

### 5. Do not catch everything

Let unexpected infrastructure exceptions propagate unless your method can actually add meaningful handling.

### 6. Return an aggregated result

Construct something similar to:

```
public record ApplicationDetails(
    JobApplication Application,
    Company? Company,
    RecruiterInfo? Recruiter);
```

The exact model can be adjusted to your existing JobTracker design.

---

# 52. Main exercise reasoning

Before coding, answer:

### A

Which operation must happen first?

Why?

### B

Once the `JobApplication` exists, which operations can run concurrently?

### C

Why would this:

```
var company =
    await LoadCompanyAsync(...);

var recruiter =
    await LoadRecruiterAsync(...);
```

potentially be slower?

### D

Why should:

```
null
```

for a missing application not necessarily become an exception?

### E

What happens if cancellation is requested while the external operations are running?

### F

What happens if `LoadCompanyAsync` throws?

Do we silently continue?

---

# 53. Optional timing experiment

This is worth running because async behaviour becomes much clearer when observed.

Create:

```
static async Task<string> OperationAsync(
    string name,
    int milliseconds)
{
    await Task.Delay(milliseconds);

    return name;
}
```

First run sequentially:

```
var stopwatch =
    Stopwatch.StartNew();

var first =
    await OperationAsync("A", 2000);

var second =
    await OperationAsync("B", 2000);

stopwatch.Stop();
```

Predict the approximate elapsed time.

Then:

```
var stopwatch =
    Stopwatch.StartNew();

var firstTask =
    OperationAsync("A", 2000);

var secondTask =
    OperationAsync("B", 2000);

await Task.WhenAll(
    firstTask,
    secondTask);

stopwatch.Stop();
```

Predict again.

The purpose is not benchmarking accuracy.

It is to observe the difference between:

```
sequential waiting
```

and:

```
concurrent waiting
```

---

# 54. Exit test

Answer without looking back initially.

### 1

What does:

```
Task
```

represent?

### 2

What additional meaning does:

```
Task<string>
```

have?

### 3

What does `await` do conceptually?

### 4

Does `await` necessarily create another thread?

### 5

What is the difference between:

```
await A();
await B();
```

and:

```
var a = A();
var b = B();

await Task.WhenAll(a, b);
```

?

### 6

When would the first version be necessary?

### 7

What is the difference between:

```
concurrency
```

and:

```
parallelism
```

?

### 8

For I/O-bound work, why is a genuine async API normally preferable to wrapping the synchronous version in `Task.Run`?

### 9

Why should this generally be avoided in async code?

```
task.Result
```

### 10

How does an exception thrown by an async operation normally surface?

### 11

What is the purpose of:

```
CancellationToken
```

?

### 12

Why is cancellation described as cooperative?

### 13

What is the difference between:

```
CancellationTokenSource
```

and:

```
CancellationToken
```

?

### 14

Why should most async methods avoid:

```
async void
```

?

### 15

What is wrong with this if saving must complete first?

```
SaveAsync();

Console.WriteLine("Saved");
```

### 16

What does:

```
IAsyncEnumerable<T>
```

represent?

### 17

What does:

```
await foreach
```

do?

### 18

When would:

```
Task<List<JobApplication>>
```

be conceptually different from:

```
IAsyncEnumerable<JobApplication>
```

?

---

# 55. Durable mental model

```
Task
    → represents asynchronous completion

Task<T>
    → asynchronous completion + eventual T result

async
    → enables task-based async method structure

await
    → suspend the method's continuation until task completion
    → without synchronously blocking the thread

await ≠ create another thread

I/O-bound async
    → avoid occupying threads while waiting

Sequential:
    await A()
    await B()

Concurrent:
    start A
    start B
    await Task.WhenAll(...)

Concurrency ≠ parallelism

.Result / .Wait()
    → synchronously block
    → generally avoid in async call chains

Async exceptions
    → surface when awaited

CancellationToken
    → cooperative cancellation request
    → propagate it through the call chain

async void
    → generally only for event handlers

IAsyncEnumerable<T>
    → asynchronous stream of T values

await foreach
    → consume those values asynchronously

Use async where waiting exists.
Do not add async merely to decorate synchronous code.
```

The most important idea to carry into ASP.NET Core is:

> **Async does not make work magically faster. It lets applications use threads efficiently while operations are waiting, and it lets independent waits overlap when concurrency is appropriate.**
>