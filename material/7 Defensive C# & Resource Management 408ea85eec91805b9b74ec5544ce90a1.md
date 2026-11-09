# 7. Defensive C# & Resource Management

Until now, most of our code has assumed that things go reasonably well.

We construct valid objects, call methods with sensible arguments, query collections, and update application state.

Professional software cannot make that assumption.

Real programs encounter:

- invalid input;
- missing data;
- failed network requests;
- unavailable files;
- database errors;
- programmer mistakes;
- resources that must be released even when something fails.

This unit is about writing code that behaves predictably when those things happen.

There are two broad themes:

**A. Defensive C#**

- validation;
- guard clauses;
- exceptions;
- `try` / `catch` / `finally`;
- exception filters;
- choosing appropriate exception types;
- custom exceptions where they genuinely add meaning;
- null guards.

**B. Resource management**

- resource lifetime;
- `IDisposable`;
- `Dispose`;
- `using`;
- `using` declarations;
- `IAsyncDisposable`;
- `await using`.

The goal is not to put `try/catch` around everything.

The goal is to understand:

> **Which problems should be prevented, which should be represented as exceptions, which should be handled, and which resources must always be cleaned up?**
> 

---

# 1. Retrieval warm-up

Short answers.

### 1.1

What invariant were we protecting with a salary range such as:

```
Minimum = 50,000
Maximum = 70,000
```

?

What should happen if someone tries:

```
Minimum = 90,000
Maximum = 20,000
```

?

### 1.2

Why is this generally preferable:

```
public string Name { get; private set; }
```

to:

```
public string Name { get; set; }
```

when arbitrary external modification would create invalid state?

### 1.3

What does nullable reference syntax communicate here?

```
string? recruiterName
```

### 1.4

Suppose a method cannot sensibly continue because an argument supplied by its caller is invalid.

What options can you think of besides silently continuing?

---

# 2. Defensive programming

Defensive programming means designing code so that invalid situations are:

- prevented where possible;
- detected close to their source;
- reported clearly;
- not silently converted into corrupted state.

Consider:

```
public void SetSalary(decimal salary)
{
    Salary = salary;
}
```

If negative salary values are meaningless in our domain, this method gives callers too much power.

We could instead validate:

```
public void SetSalary(decimal salary)
{
    if (salary < 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(salary));
    }

    Salary = salary;
}
```

Now the invalid state is rejected immediately.

The principle is:

> **Fail near the source of the problem rather than allowing invalid state to travel through the application.**
> 

---

# 3. Validation versus exception handling

These are related, but not identical.

**Validation** asks:

> Is this input/state acceptable?
> 

For example:

```
if (minimum > maximum)
{
    ...
}
```

**Exception handling** asks:

> What should happen when an operation fails by throwing an exception?
> 

For example:

```
try
{
    ...
}
catch (...)
{
    ...
}
```

A useful distinction:

```
validation
    → detect invalid conditions deliberately

exception
    → represent an operation that could not complete normally

catch
    → decide whether this layer can meaningfully handle that failure
```

Do not treat `catch` as your normal validation mechanism.

---

# 4. Guard clauses

A **guard clause** checks a condition near the beginning of a method and exits immediately when the condition is invalid.

Instead of:

```
public void Apply(JobApplication application)
{
    if (application is not null)
    {
        if (application.CurrentStatus ==
            ApplicationStatus.Interested)
        {
            // actual work
        }
    }
}
```

we can reject invalid conditions first:

```
public void Apply(JobApplication application)
{
    if (application is null)
    {
        throw new ArgumentNullException(
            nameof(application));
    }

    if (application.CurrentStatus !=
        ApplicationStatus.Interested)
    {
        throw new InvalidOperationException(
            "Only interested applications can be applied.");
    }

    // actual work
}
```

The main path is no longer buried inside nested conditions.

Guard clauses are especially useful for:

- argument validation;
- state preconditions;
- early rejection of impossible operations.

---

# 5. Choosing what to validate

Not every method needs twenty defensive checks.

Validation should normally happen where a boundary or invariant requires it.

Good candidates include:

```
constructor arguments
public method arguments
external input
state transitions
configuration
user-supplied data
```

For example, `SalaryRange` should protect:

```
Minimum >= 0
Maximum >= 0
Minimum <= Maximum
```

because those rules define whether the object itself is valid.

On the other hand, repeatedly checking an invariant that the class already guarantees internally may add noise without adding safety.

The aim is:

> **Make invalid state difficult to create, rather than repeatedly checking for it everywhere afterwards.**
> 

---

# 6. First check — validation

Suppose:

```
public void Reject(string reason)
{
    CurrentStatus = ApplicationStatus.Rejected;
    RejectionReason = reason;
}
```

Assume an empty rejection reason is not allowed.

Where would you prefer to reject it?

A. Somewhere later when displaying the application.

B. Inside `Reject`.

C. Every time `RejectionReason` is read.

Explain why.

---

# 7. Exceptions

An exception represents a failure to complete the normal operation.

You've certainly encountered code such as:

```
throw new ArgumentException(
    "The value is invalid.");
```

`throw` interrupts normal execution.

Control travels up the call stack until:

- a matching `catch` handles the exception; or
- the exception remains unhandled and eventually terminates the operation/program.

For example:

```
static void ValidateAge(int age)
{
    if (age < 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(age));
    }
}
```

Calling:

```
ValidateAge(-5);
```

does not return normally.

---

# 8. Exceptions are objects

Exceptions are themselves objects.

All normal exception types ultimately derive from:

```
System.Exception
```

Common examples include:

```
ArgumentException
ArgumentNullException
ArgumentOutOfRangeException
InvalidOperationException
IOException
FileNotFoundException
TimeoutException
```

The exception object can carry information such as:

```
exception.Message
exception.StackTrace
exception.InnerException
```

The type itself also communicates meaning.

That is why:

```
throw new ArgumentNullException(...);
```

is generally more informative than:

```
throw new Exception("Bad.");
```

---

# 9. Common argument exceptions

When a caller provides a bad argument, several standard exception types are useful.

### `ArgumentNullException`

The argument must not be `null`.

```
if (application is null)
{
    throw new ArgumentNullException(
        nameof(application));
}
```

### `ArgumentOutOfRangeException`

The argument's type is valid, but its value lies outside an acceptable range.

```
if (salary < 0)
{
    throw new ArgumentOutOfRangeException(
        nameof(salary));
}
```

### `ArgumentException`

The argument is invalid for another reason.

```
if (minimum > maximum)
{
    throw new ArgumentException(
        "Minimum cannot exceed maximum.");
}
```

These communicate:

> The caller gave this method an invalid argument.
> 

---

# 10. `InvalidOperationException`

Sometimes the arguments are fine, but the operation is not valid in the object's **current state**.

For example:

```
public void Apply()
{
    if (CurrentStatus !=
        ApplicationStatus.Interested)
    {
        throw new InvalidOperationException(
            "This application has already been processed.");
    }

    CurrentStatus = ApplicationStatus.Applied;
}
```

The problem is not an argument.

The problem is:

```
this operation cannot be performed now
```

That makes `InvalidOperationException` a natural candidate.

Compare:

```
bad value supplied to method
    → argument exception

valid call shape, invalid object state
    → InvalidOperationException
```

---

# 11. Exception-selection check

Choose the most natural standard exception.

### A

```
SetMaximumSalary(-1000);
```

### B

```
Save(application);
```

where `application` is `null`.

### C

Calling:

```
application.Apply();
```

when the application is already `Rejected`.

Explain each choice.

---

# 12. `try` and `catch`

So far we've created exceptions.

Now let's handle one.

```
try
{
    var text = File.ReadAllText("applications.json");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine(
        $"File not found: {ex.Message}");
}
```

The `try` block contains code that may fail.

A compatible `catch` handles a specific exception.

Execution works approximately like this:

```
enter try
   ↓
operation succeeds?
   ├─ yes → skip catch
   └─ no
       ↓
 matching catch?
       ├─ yes → execute catch
       └─ no  → exception continues upward
```

---

# 13. Catch the exceptions you can actually handle

This is legal:

```
try
{
    ...
}
catch (Exception)
{
    ...
}
```

But broad catches are often a warning sign.

Suppose:

```
try
{
    SaveApplication(application);
}
catch (Exception)
{
    Console.WriteLine("Something went wrong.");
}
```

This might hide:

- programming bugs;
- database failures;
- invalid state;
- configuration errors;
- unexpected nulls.

The key question before writing a `catch` is:

> **Can this layer actually do something useful about this failure?**
> 

Useful responses might include:

- retrying;
- choosing an alternative;
- converting the failure into an appropriate API response;
- logging and terminating the current operation;
- asking the user for different input.

Simply swallowing an exception is rarely useful.

---

# 14. Multiple `catch` blocks

Different failures may require different responses.

```
try
{
    var text = File.ReadAllText(path);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine(
        $"Could not find {path}: {ex.Message}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine(
        $"Access denied: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine(
        $"I/O error: {ex.Message}");
}
```

Ordering matters when exception types have inheritance relationships.

More specific exceptions should appear before broader ones.

For example, because `FileNotFoundException` derives from `IOException`, this ordering makes sense:

```
FileNotFoundException
IOException
```

The reverse would make the more specific case unreachable.

This should feel familiar from pattern matching:

> **specific before general**
> 

---

# 15. Rethrowing

Sometimes a layer wants to perform some work when an exception occurs but cannot fully handle it.

For example:

```
try
{
    SaveApplication(application);
}
catch (IOException)
{
    Console.WriteLine(
        "Saving the application failed.");

    throw;
}
```

The bare:

```
throw;
```

rethrows the current exception.

This allows a higher layer to continue handling it.

An important distinction:

```
throw;
```

and:

```
throw ex;
```

are not interchangeable.

When rethrowing the current exception, prefer:

```
throw;
```

because it preserves the original stack-trace information correctly.

---

# 16. `finally`

Sometimes an action must happen whether the operation succeeds or fails.

That is what `finally` is for.

```
try
{
    Console.WriteLine("Starting...");
    DoWork();
}
catch (IOException ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Finished attempt.");
}
```

The `finally` block runs when control leaves the `try`/`catch` structure, including when an exception occurs.

Historically, this is extremely important for resource cleanup.

Conceptually:

```
acquire resource

try
    use resource
finally
    release resource
```

We will shortly see that modern C# provides a safer abstraction for this common pattern: `using`.

---

# 17. `try` / `catch` / `finally` prediction

Consider:

```
try
{
    Console.WriteLine("A");

    throw new InvalidOperationException();

    Console.WriteLine("B");
}
catch (InvalidOperationException)
{
    Console.WriteLine("C");
}
finally
{
    Console.WriteLine("D");
}

Console.WriteLine("E");
```

Before executing it, write the output in order.

Also explain why `"B"` is never printed.

---

# 18. Exception filters

A `catch` can be restricted further using:

```
when
```

Example:

```
try
{
    ProcessApplication(application);
}
catch (InvalidOperationException ex)
    when (application.CurrentStatus ==
          ApplicationStatus.Rejected)
{
    Console.WriteLine(
        "Rejected applications cannot be processed.");
}
```

The exception type must match **and** the filter must evaluate to `true`.

Another common form is filtering using information from the exception:

```
catch (HttpRequestException ex)
    when (ex.StatusCode ==
          HttpStatusCode.NotFound)
{
    ...
}
```

The point is not syntactic cleverness.

Filters are useful when:

> the same exception type can represent different situations and this layer only wants to handle some of them.
> 

---

# 19. When not to use exceptions

Exceptions are not intended as ordinary branching mechanisms.

This would be poor design:

```
try
{
    var application =
        applications.Single(a => a.Id == id);
}
catch (InvalidOperationException)
{
    // means application not found
}
```

if "not found" is an expected, ordinary possibility.

Instead, choose an API that represents the expected outcome more naturally:

```
var application =
    applications.SingleOrDefault(a => a.Id == id);

if (application is null)
{
    ...
}
```

General principle:

```
expected alternative outcome
    → model it explicitly

unexpected failure / violated contract
    → exception may be appropriate
```

Do not use exceptions merely because they can alter control flow.

---

# 20. Custom exceptions

You can define your own exception:

```
public class InvalidApplicationTransitionException
    : Exception
{
    public InvalidApplicationTransitionException(
        string message)
        : base(message)
    {
    }
}
```

Then:

```
throw new InvalidApplicationTransitionException(
    "A rejected application cannot return to Applied.");
```

But custom exceptions should not be created merely because the domain has custom terminology.

Ask:

> Does callers' handling behaviour improve because this failure has its own type?
> 

If nobody needs to distinguish it from:

```
InvalidOperationException
```

then the custom type may add ceremony without value.

Custom exceptions become more useful when:

- callers genuinely need to catch a specific domain failure;
- the failure needs domain-specific information;
- an application boundary translates different failures differently.

---

# 21. Null guards

Nullability annotations help prevent null-related errors at compile time, but runtime checks are still relevant at public boundaries.

Traditional form:

```
public void Add(JobApplication application)
{
    if (application is null)
    {
        throw new ArgumentNullException(
            nameof(application));
    }

    ...
}
```

Modern .NET also provides:

```
ArgumentNullException.ThrowIfNull(application);
```

So:

```
public void Add(JobApplication application)
{
    ArgumentNullException.ThrowIfNull(application);

    ...
}
```

expresses the same intent concisely.

This is a **guard helper**.

It communicates:

> This parameter is contractually required to be non-null.
> 

---

# 22. Other guard helpers and explicit validation

You may encounter framework helpers for common checks, but do not force every validation into a helper.

Sometimes explicit domain code is clearer:

```
if (minimum > maximum)
{
    throw new ArgumentException(
        "Minimum cannot exceed maximum.");
}
```

Likewise:

```
if (CurrentStatus != ApplicationStatus.Interested)
{
    throw new InvalidOperationException(
        "Only interested applications can be applied.");
}
```

The important distinction is:

```
generic programming precondition
    → standard guard/helper may fit

domain rule
    → explicit domain validation often communicates more
```

---

# 23. Resource lifetime

Now we move to the second major part of the unit.

Some objects represent or control external resources.

Examples include:

- files;
- streams;
- network connections;
- database connections;
- operating-system handles;
- some graphics resources.

These resources may need explicit cleanup.

Suppose:

```
var stream = File.OpenRead("applications.json");
```

The stream is associated with an underlying file resource.

At some point that resource must be released.

We therefore have a **lifetime**:

```
acquire
   ↓
use
   ↓
release
```

The difficult part is guaranteeing that `release` happens even if `use` throws an exception.

---

# 24. `IDisposable`

.NET represents synchronous cleanup through the interface:

```
public interface IDisposable
{
    void Dispose();
}
```

A type implementing `IDisposable` is telling callers:

> I own something that should be explicitly released when you're finished with me.
> 

You could manually write:

```
var stream = File.OpenRead(path);

try
{
    // use stream
}
finally
{
    stream.Dispose();
}
```

This guarantees cleanup through `finally`.

But this pattern is common enough that C# has dedicated syntax for it.

---

# 25. `using`

The `using` statement manages an `IDisposable` object.

```
using (var stream = File.OpenRead(path))
{
    // use stream
}
```

When execution leaves the block, C# ensures that `Dispose()` is called.

Conceptually, it corresponds roughly to:

```
var stream = File.OpenRead(path);

try
{
    // use stream
}
finally
{
    stream.Dispose();
}
```

including when an exception occurs.

This is why `using` is fundamentally about:

> **deterministic cleanup**
> 

not just shorter syntax.

---

# 26. Using declarations

Modern C# also supports a **using declaration**:

```
using var stream =
    File.OpenRead(path);

// use stream
```

There are no braces.

The object is disposed when execution leaves the current scope.

Example:

```
static void ReadFile(string path)
{
    using var stream =
        File.OpenRead(path);

    // use stream

} // stream disposed here
```

This often produces flatter code than nesting everything inside:

```
using (...)
{
    ...
}
```

Both forms express resource ownership.

The scope determines the lifetime.

---

# 27. Disposal is not garbage collection

This distinction matters.

The garbage collector manages **managed memory**.

`Dispose()` deals with the timely release of resources whose lifetime should not be left to eventual garbage collection.

So do not build the mental model:

```
Dispose = delete object from memory
```

That is wrong.

Instead:

```
GC
    → manages managed-memory lifetime

Dispose
    → explicit cleanup protocol for disposable resources
```

The object may still technically exist after `Dispose()`.

Its resource is simply no longer supposed to be used.

---

# 28. Ownership matters

Suppose a method creates a stream:

```
using var stream =
    File.OpenRead(path);
```

That method clearly owns it and should dispose it.

But suppose a stream is passed in:

```
static void Process(Stream stream)
{
    ...
}
```

Should `Process` dispose it?

Not automatically.

The caller may still need it.

This leads to an important engineering principle:

> **The code that owns a disposable resource should normally control its lifetime.**
> 

APIs should make ownership expectations clear.

Otherwise one component may dispose a resource while another still expects to use it.

---

# 29. `IAsyncDisposable` and `await using`

Some cleanup itself requires asynchronous work.

For that, .NET provides:

```
IAsyncDisposable
```

whose cleanup operation is:

```
ValueTask DisposeAsync();
```

C# supports it with:

```
await using
```

For example:

```
await using var resource =
    await CreateResourceAsync();
```

When the scope ends, asynchronous disposal can be awaited.

This is especially relevant in modern server-side .NET, where resources may involve asynchronous I/O.

You do not need to master `ValueTask` here—we will study asynchronous programming properly in Unit 9.

For now the mental mapping is:

```
IDisposable
    → Dispose()
    → using

IAsyncDisposable
    → DisposeAsync()
    → await using
```

---

# 30. Main JobTracker exercise

Now apply the defensive-programming part to JobTracker.

Do **not** create fake disposable resources merely to demonstrate `IDisposable`.

The domain exercise should use the concepts where they naturally belong.

Review the relevant `JobApplication` and `SalaryRange` behaviour and enforce these rules.

### `SalaryRange`

Construction must reject:

```
negative minimum
negative maximum
minimum > maximum
```

Choose appropriate standard exception types.

### `JobApplication`

Assume these rules:

```
Interested → Applied
Interested → Rejected
Applied    → Rejected
Rejected   → no further transition
```

Protect those transitions.

External code must not be able to bypass them by directly assigning `CurrentStatus`.

### Repository/input boundary

Create a method conceptually similar to:

```
void Add(JobApplication application)
```

It must reject `null` explicitly.

Use an appropriate guard.

### Failure reasoning

For each failure, explain whether it is:

```
invalid argument
invalid object state
expected alternative result
exceptional failure
```

Do not create a custom exception unless you can justify why callers benefit from distinguishing it by type.

---

## Resource-management practice

Separately, write a small method:

```
static string LoadText(string path)
```

that reads a file through a `StreamReader`.

Requirements:

- create the resource yourself;
- ensure it is disposed correctly;
- use modern `using` syntax;
- do not catch exceptions merely to print `"Error"`;
- let failures propagate unless the method can meaningfully recover.

Then answer:

> Why is `using` required even if `ReadToEnd()` throws?
> 

---

# 31. Exit test

Answer without looking back initially.

### 1

What's the difference between validation and exception handling?

### 2

Which is the better fit?

```
negative salary argument
```

- `ArgumentOutOfRangeException`
- `InvalidOperationException`

Why?

### 3

Which is the better fit?

```
calling Reject() on an application that is already rejected
```

Why?

### 4

What does this guarantee?

```
finally
{
    Cleanup();
}
```

### 5

Why is this usually dangerous?

```
catch (Exception)
{
}
```

### 6

What does:

```
catch (IOException ex)
    when (...)
```

add to an ordinary `catch`?

### 7

What is the difference between:

```
throw;
```

and blindly replacing it with:

```
throw ex;
```

when rethrowing?

### 8

Why would you use:

```
ArgumentNullException.ThrowIfNull(value);
```

?

### 9

What contract does implementing:

```
IDisposable
```

communicate?

### 10

What does:

```
using var stream = ...;
```

guarantee?

### 11

Does `Dispose()` mean:

> immediately remove this object from managed memory?
> 

Explain.

### 12

Who should normally dispose a resource: the code that owns it or every method that happens to receive it?

### 13

What is the asynchronous counterpart of:

```
IDisposable + using
```

?

---

# 32. Durable mental model

```
Prevent invalid state where possible.

Bad argument
    → ArgumentException family

Operation invalid for current state
    → often InvalidOperationException

Expected alternative outcome
    → model explicitly; don't use exceptions as ordinary branching

try
    → operation that may fail

catch
    → handle only failures this layer can meaningfully handle

finally
    → work that must happen when leaving the attempt

throw;
    → rethrow current exception

Guard clauses
    → reject invalid conditions early

IDisposable
    → resource has explicit synchronous cleanup
    → Dispose()
    → using

IAsyncDisposable
    → asynchronous cleanup
    → DisposeAsync()
    → await using

GC manages memory.
Dispose manages resource lifetime.

Ownership determines who should dispose.
```

The overall professional rule is:

> **Make invalid state difficult to create, fail clearly when an operation cannot continue, catch only what you can meaningfully handle, and make resource cleanup deterministic.**
>