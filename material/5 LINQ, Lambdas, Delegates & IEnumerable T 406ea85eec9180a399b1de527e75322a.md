# 5. LINQ, Lambdas, Delegates & IEnumerable<T>

**Project:** JobTracker

**Main objective:** Understand what LINQ is actually doing, then use it confidently to query and transform collections.

This topic has more conceptual depth than simply learning `Where`, `Select`, and `OrderBy`. The important chain is:

```
methods as values
      ↓
delegates
      ↓
lambdas
      ↓
IEnumerable<T>
      ↓
LINQ
      ↓
deferred execution
      ↓
materialisation
```

By the end, code like this should not just look familiar:

```
var results = applications
    .Where(a => a.Remote)
    .OrderByDescending(a => a.DateDiscovered)
    .Select(a => new { a.Company.Name, a.Position });
```

You should understand what each operation receives, what it returns, and approximately **when the work actually happens**.

Microsoft's current documentation confirms that LINQ to Objects operates over `IEnumerable<T>`, that most sequence-returning operators use deferred execution, and that LINQ lambdas compile to delegates when working with `IEnumerable<T>`. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/linq/get-started/introduction-to-linq-queries?utm_source=chatgpt.com)

---

## 1. Retrieval warm-up

Keep this short.

### A — Collections

Choose the most natural type:

```
1. Ordered collection of applications.
2. Company retrieved by Guid.
3. Unique required technologies.
```

Choose between:

```
List<T>
Dictionary<TKey, TValue>
HashSet<T>
```

### B — Encapsulation

What's wrong with this?

```
public List<JobApplication> Applications => _applications;
```

if `JobTracker` is supposed to control how applications are added and removed?

### C — Write from memory

Write:

```
A List<int> containing 2, 5, 8, 11 and 14.

Use foreach to print only the even numbers.
```

Don't use LINQ yet.

---

# 2. The problem LINQ solves

Suppose we have:

```
List<int> numbers = [2, 5, 8, 11, 14];
```

and want even numbers.

You already know how:

```
List<int> evenNumbers = [];

foreach (var number in numbers)
{
    if (number % 2 == 0)
    {
        evenNumbers.Add(number);
    }
}
```

Nothing is wrong with that.

But notice the generic algorithm:

```
take sequence
     ↓
inspect each element
     ↓
apply condition
     ↓
keep matching elements
```

The part that changes is:

```
number % 2 == 0
```

Maybe tomorrow we want:

```
number > 10
```

or:

```
number < 0
```

What if we could pass that **condition itself** into a reusable filtering algorithm?

That's where delegates enter.

---

# 3. Delegates — methods as values

A delegate is a type capable of referencing methods with a compatible parameter list and return type. This lets C# pass behaviour around as data. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/delegates-lambdas?utm_source=chatgpt.com)

Suppose we have:

```
static bool IsEven(int number)
{
    return number % 2 == 0;
}
```

We can store a reference to that method:

```
Func<int, bool> condition = IsEven;
```

Read:

```
Func<int, bool>
     │     │
     │     └── returns bool
     │
     └── receives int
```

So:

```
Func<int, bool>
```

means:

> Something callable that receives an `int` and returns a `bool`.
> 

Then:

```
bool result = condition(10);
```

invokes the referenced method.

`result` is:

```
true
```

---

# 4. `Func<>` and `Action<>`

You'll encounter these constantly.

## `Func`

A `Func` **returns something**.

```
Func<int, bool>
```

means:

```
int → bool
```

This:

```
Func<string, int>
```

means:

```
string → int
```

This:

```
Func<int, int, decimal>
```

means:

```
int + int → decimal
```

The **last generic argument is always the return type**.

For example:

```
Func<int, int, int> add = Add;

static int Add(int a, int b)
{
    return a + b;
}
```

---

## `Action`

An `Action` returns nothing:

```
Action<string>
```

means approximately:

```
string → void
```

Example:

```
Action<string> print = Print;

static void Print(string text)
{
    Console.WriteLine(text);
}
```

Microsoft's current documentation describes `Func` as representing methods that return values and `Action` as representing methods returning `void`. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/delegates-lambdas?utm_source=chatgpt.com)

---

# 5. Micro-exercise — delegates

Without running anything, describe the signature represented by each:

```
Func<int, bool>
Func<string, decimal>
Func<int, int, bool>
Action<string>
Action<int, string>
```

Then write a method that could legally be assigned to:

```
Func<decimal, decimal>
```

Don't use a lambda yet.

---

# 6. Why delegates matter

We could write our own filtering method:

```
static List<int> Filter(
    IEnumerable<int> numbers,
    Func<int, bool> condition)
{
    List<int> result = [];

    foreach (var number in numbers)
    {
        if (condition(number))
        {
            result.Add(number);
        }
    }

    return result;
}
```

Then:

```
static bool IsEven(int number)
{
    return number % 2 == 0;
}

var even = Filter(numbers, IsEven);
```

But the same filtering machinery can now receive:

```
static bool IsLarge(int number)
{
    return number > 100;
}
```

and:

```
var large = Filter(numbers, IsLarge);
```

We've separated:

```
HOW TO FILTER

from

WHAT COUNTS AS A MATCH
```

That's an extremely important abstraction.

And it's basically the idea behind LINQ's `Where`.

---

# 7. Lambdas

Creating a named method for every tiny condition would get tedious:

```
static bool IsEven(int number)
{
    return number % 2 == 0;
}
```

C# allows us to express the same behaviour inline:

```
number => number % 2 == 0
```

That's a **lambda expression**. Microsoft describes a lambda as a compact anonymous function that can be converted to a compatible delegate type (or, in other contexts, an expression tree). [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/delegates-lambdas?utm_source=chatgpt.com)

So:

```
Func<int, bool> isEven =
    number => number % 2 == 0;
```

means:

```
number
   ↓
number % 2 == 0
   ↓
bool
```

---

# 8. Reading lambdas

Get into the habit of reading:

```
x => x * 2
```

as:

> Given `x`, return `x * 2`.
> 

And:

```
application => application.Remote
```

as:

> Given an application, return whether it's remote.
> 

And:

```
application => application.Company.Name
```

as:

> Given an application, return its company's name.
> 

The compiler often infers the parameter and return types from context.

---

# 9. Expression vs statement lambdas

Simple:

```
x => x * 2
```

More complicated:

```
x =>
{
    var doubled = x * 2;
    Console.WriteLine(doubled);
    return doubled;
}
```

The first is an **expression lambda**.

The second has a statement body.

Don't expand a lambda into five lines just to prove that you can. Once a lambda becomes complicated, a named method may be clearer.

---

# 10. Micro-exercise — translate between methods and lambdas

Convert this:

```
static bool IsRemote(JobApplication application)
{
    return application.Remote;
}
```

into a lambda compatible with:

```
Func<JobApplication, bool>
```

Then convert:

```
application => application.Position
```

into a named method.

Finally answer:

What `Func<...>` type could hold that second lambda?

---

# 11. Now `Where()` stops being magic

Suppose:

```
List<JobApplication> applications = [...];
```

Then:

```
var remote = applications.Where(
    application => application.Remote);
```

Conceptually, `Where` receives:

```
1. A sequence of JobApplication
2. A function:

   JobApplication → bool
```

That function is called a **predicate**: something that answers yes/no for an element.

Conceptually:

```
Job A → predicate → true  → keep
Job B → predicate → false → discard
Job C → predicate → true  → keep
```

The relevant `Enumerable.Where` overload effectively has the shape:

```
IEnumerable<T> Where<T>(
    IEnumerable<T> source,
    Func<T, bool> predicate)
```

Don't memorise that signature. Understand it.

---

# 12. Extension-method syntax

You usually write:

```
applications.Where(a => a.Remote);
```

But `Where` isn't an instance method you wrote inside `List<T>`.

LINQ to Objects operators are extension methods supplied by `System.Linq.Enumerable`.

Conceptually:

```
applications.Where(...)
```

corresponds to:

```
Enumerable.Where(applications, ...)
```

Extension methods let static methods be invoked using instance-method syntax when their first parameter is declared with `this`.

This is why LINQ works with many different sequence types rather than being a special feature of `List<T>`.

---

# 13. `IEnumerable<T>` is the key

Topic 4 introduced:

```
IEnumerable<T>
```

as essentially:

> Something you can enumerate.
> 

That's the common abstraction LINQ to Objects builds upon. Microsoft explicitly describes in-memory LINQ as operating on types implementing `IEnumerable<T>`. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/linq/?view=net-7.0&utm_source=chatgpt.com)

Therefore LINQ can work with:

```
List<T>
arrays
HashSet<T>
Dictionary collections
many custom collections
LINQ results themselves
...
```

because they can expose sequences through `IEnumerable<T>`.

This is fundamental:

```
List<JobApplication>
        │
        ▼
IEnumerable<JobApplication>
        │
        ▼
Where(...)
        │
        ▼
IEnumerable<JobApplication>
        │
        ▼
OrderBy(...)
        │
        ▼
IOrderedEnumerable<JobApplication>
        │
        ▼
Select(...)
        │
        ▼
IEnumerable<SomethingElse>
```

LINQ operations compose because their outputs can become inputs to subsequent operations.

---

# 14. `Where` — filtering

Example:

```
var remoteApplications =
    applications.Where(a => a.Remote);
```

Multiple conditions:

```
var interesting =
    applications.Where(a =>
        a.Remote &&
        a.CurrentStatus == ApplicationStatus.Interested);
```

Or chain filters:

```
var interesting = applications
    .Where(a => a.Remote)
    .Where(a => a.CurrentStatus == ApplicationStatus.Interested);
```

Both can be legitimate.

The first may communicate a single combined condition better.

---

# 15. `Select` — projection

This one is crucial.

`Where` asks:

> Should this element remain?
> 

`Select` asks:

> What should this element become?
> 

Given:

```
IEnumerable<JobApplication>
```

we can produce:

```
IEnumerable<string>
```

with:

```
var positions =
    applications.Select(a => a.Position);
```

Conceptually:

```
JobApplication
      ↓
   Position
      ↓
    string
```

The sequence type has changed.

---

# 16. Projection isn't filtering

This distinction should become automatic:

```
Where(...)
```

changes **which elements** remain.

```
Select(...)
```

changes **what each element becomes**.

Example:

```
var companyNames = applications
    .Where(a => a.Remote)
    .Select(a => a.Company.Name);
```

Pipeline:

```
JobApplication
JobApplication
JobApplication
      │
      │ Where(Remote)
      ▼
JobApplication
JobApplication
      │
      │ Select(Company.Name)
      ▼
string
string
```

---

# 17. Anonymous-type projections

Sometimes we don't want an entire domain object.

We want:

```
Company
Position
Status
```

We can project:

```
var summaries = applications.Select(a => new
{
    Company = a.Company.Name,
    a.Position,
    a.CurrentStatus
});
```

This creates an **anonymous type**.

You haven't declared:

```
class ApplicationSummary
```

The compiler creates a type suitable for that local projection.

This is useful for temporary transformations.

Later, especially at API boundaries, explicit DTO types will often be preferable.

---

# 18. `OrderBy` and `ThenBy`

```
var ordered =
    applications.OrderBy(a => a.DateDiscovered);
```

Descending:

```
var ordered =
    applications.OrderByDescending(a => a.DateDiscovered);
```

Secondary ordering:

```
var ordered = applications
    .OrderBy(a => a.Company.Name)
    .ThenBy(a => a.Position);
```

Important distinction:

```
.OrderBy(...)
.OrderBy(...)
```

is not equivalent to:

```
.OrderBy(...)
.ThenBy(...)
```

`ThenBy` adds a secondary ordering criterion.

---

# 19. Micro-exercise — pipeline reading

Don't run this:

```
var result = applications
    .Where(a => a.Remote)
    .OrderByDescending(a => a.DateDiscovered)
    .Select(a => a.Position);
```

Answer:

1. What is the source element type?
2. What does `Where` receive as its lambda parameter?
3. What does its lambda return?
4. What element type comes out of `Where`?
5. What does `OrderByDescending` use as its key?
6. What element type finally comes out of `Select`?

---

# 20. `Any()` and `All()`

`Any` asks:

> Does at least one element satisfy this?
> 

```
bool hasRemote =
    applications.Any(a => a.Remote);
```

`All` asks:

> Does every element satisfy this?
> 

```
bool allRemote =
    applications.All(a => a.Remote);
```

Useful distinction:

```
applications.Count() > 0
```

versus:

```
applications.Any()
```

If all you need to know is whether an element exists, `Any()` communicates that intent directly.

---

# 21. `First`, `FirstOrDefault`, `Single`, `SingleOrDefault`

These are frequently confused.

## `First`

```
applications.First(a => a.Remote);
```

Meaning:

> Give me the first match.
> 

No match → exception.

---

## `FirstOrDefault`

```
applications.FirstOrDefault(a => a.Remote);
```

Meaning:

> Give me the first match, or the default value if there isn't one.
> 

For a reference type, the default is normally:

```
null
```

So nullable reference analysis matters here.

---

## `Single`

```
applications.Single(a => a.Id == id);
```

Meaning:

> Exactly one matching element must exist.
> 

Zero → exception.

More than one → exception.

This expresses a stronger invariant than `First`.

---

## `SingleOrDefault`

Meaning:

> Zero or one match is valid. More than one is an error.
> 

So don't choose between `FirstOrDefault` and `SingleOrDefault` based on which one avoids exceptions.

Choose based on **what the domain says about cardinality**.

---

# 22. Exercise — choose the operator

Choose:

```
First
FirstOrDefault
Single
SingleOrDefault
```

for each situation and explain why.

### A

Find the first application discovered after a particular date. There may be none.

### B

Find the application with a unique `Id`. If the ID doesn't exist, absence is acceptable, but duplicate IDs indicate a serious bug.

### C

Get the earliest item from a sequence that is guaranteed by earlier logic to contain at least one element.

### D

Find the only application associated with a one-time external reference number. Exactly one must exist.

Don't just give operator names. Explain the semantic difference.

---

# 23. `Count`, `Sum`, `Average`, `Min`, `Max`

These return scalar values:

```
var total = applications.Count();

var remoteCount =
    applications.Count(a => a.Remote);
```

If salaries are suitable:

```
var averageMinimumSalary =
    applications.Average(a => a.SalaryRange.LowEnd);
```

These differ from sequence-returning operators because they need to produce a result immediately.

This becomes important in a moment.

---

# 24. `Distinct` and `DistinctBy`

Suppose several applications belong to the same company.

You might want unique company names:

```
var names = applications
    .Select(a => a.Company.Name)
    .Distinct();
```

Modern LINQ also provides `DistinctBy`:

```
var companies = applications
    .Select(a => a.Company)
    .DistinctBy(c => c.Id);
```

Notice the connection to Topic 4:

**equality matters.**

`Distinct()` needs equality semantics for the elements.

`DistinctBy(...)` lets you identify uniqueness through a selected key.

---

# 25. `GroupBy`

Suppose:

```
Interested: 4
Applied:    7
Rejected:   3
```

We're grouping applications by status.

```
var groups =
    applications.GroupBy(a => a.CurrentStatus);
```

Conceptually:

```
Application 1 ─┐
Application 2 ─┼── Applied
Application 3 ─┘

Application 4 ─┐
Application 5 ─┼── Interested
Application 6 ─┘
```

Each group has:

```
Key
+
sequence of elements
```

so:

```
foreach (var group in groups)
{
    Console.WriteLine(group.Key);
    Console.WriteLine(group.Count());
}
```

`GroupBy` is one of the operators where understanding the shape of the result matters more than memorising syntax. Microsoft defines it in terms of a key-selector function that groups elements according to the selected key. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable?view=netframework-4.8&utm_source=chatgpt.com)

---

# 26. `SelectMany` — flattening

This is another important one.

Imagine each application contains:

```
HashSet<string> RequiredSkills
```

and we have three applications:

```
Application A
    C#
    SQL

Application B
    C#
    Docker

Application C
    React
    TypeScript
```

If we do:

```
applications.Select(a => a.RequiredSkills)
```

we conceptually get:

```
sequence
    ↓
set
set
set
```

Something like:

```
IEnumerable<HashSet<string>>
```

But perhaps we want:

```
C#
SQL
C#
Docker
React
TypeScript
```

That's flattening.

```
var skills =
    applications.SelectMany(a => a.RequiredSkills);
```

Conceptually:

```
many collections
       ↓
SelectMany
       ↓
one sequence
```

And:

```
var uniqueSkills = applications
    .SelectMany(a => a.RequiredSkills)
    .Distinct();
```

becomes a very natural query.

---

# 27. Micro-exercise — `Select` or `SelectMany`?

For each, choose one.

**A.** Get every application's position.

**B.** Get the collection of skills for every application, keeping each application's skill collection separate.

**C.** Get one flat sequence containing all skills across all applications.

**D.** Get every company's name.

Explain the resulting sequence shape.

---

# 28. `ToList`, `ToArray`, `ToDictionary`

These are **materialisation** operators.

```
var result = applications
    .Where(a => a.Remote)
    .ToList();
```

Now you actually have:

```
List<JobApplication>
```

rather than merely a LINQ sequence describing work to perform.

Similarly:

```
.ToArray()
```

and:

```
.ToDictionary(
    a => a.Id,
    a => a);
```

`ToDictionary` is especially worth thinking about: if two elements produce the same key, what should happen?

A dictionary cannot contain duplicate keys.

---

# 29. The most important LINQ concept: deferred execution

Consider:

```
var remoteApplications =
    applications.Where(a => a.Remote);
```

A very natural assumption is:

> LINQ has now searched the list and stored the remote applications.
> 

Usually, **it hasn't**.

Most LINQ operators returning `IEnumerable<T>` use **deferred execution**: the query describes work that <u>**occurs when the sequence is enumerated</u>.** [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/linq/get-started/introduction-to-linq-queries?utm_source=chatgpt.com)

So:

```
var remoteApplications =
    applications.Where(a => a.Remote);
```

roughly means:

> Here is a sequence capable of producing remote applications when someone asks for them.
> 

Not:

> Here is a new list containing all current remote applications.
> 

That's a major distinction.

---

# 30. Predict this before reading further

```
List<int> numbers = [1, 2, 3, 4];

var evenNumbers =
    numbers.Where(n => n % 2 == 0);

numbers.Add(6);

foreach (var number in evenNumbers)
{
    Console.WriteLine(number);
}
```

What prints?

The answer is:

```
2
4
6
```

because the query is evaluated when enumerated.

Microsoft explicitly notes that deferred query results depend on the contents of the source **when the query executes**, not necessarily when the query variable was declared. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/linq/get-started/introduction-to-linq-queries?utm_source=chatgpt.com)

---

# 31. Materialisation changes that

Compare:

```
List<int> numbers = [1, 2, 3, 4];

var evenNumbers = numbers
    .Where(n => n % 2 == 0)
    .ToList();

numbers.Add(6);
```

Now `evenNumbers` contains:

```
2
4
```

because:

```
.ToList()
```

forced execution and stored the results at that point.

So mentally:

```
Where(...)
    ↓
query / recipe / pipeline

Where(...).ToList()
    ↓
execute now and store results
```

That's slightly simplified, but it's a useful mental model.

---

# 32. Why deferred execution is useful

It enables composition:

```
var query =
    applications.Where(a => a.Remote);

query =
    query.Where(a =>
        a.CurrentStatus == ApplicationStatus.Interested);
```

Nothing necessarily needs to enumerate yet.

Eventually:

```
foreach (var application in query)
{
    ...
}
```

causes the pipeline to execute.

This can avoid unnecessary intermediate collections and allows query definitions to operate on the current source.

But it can also surprise you.

---

# 33. Multiple enumeration

Consider:

```
var results =
    applications.Where(a => IsInteresting(a));

foreach (var application in results)
{
    ...
}

foreach (var application in results)
{
    ...
}
```

The query may execute **twice**.

That means:

```
IsInteresting(a)
```

may run again for every element.

With an in-memory list and a trivial predicate, perhaps irrelevant.

But later your source might involve:

```
expensive computation
database query
network-backed sequence
file processing
```

Repeated enumeration can become significant.

Sometimes:

```
var results = applications
    .Where(...)
    .ToList();
```

is appropriate specifically because you want a stable result that will be reused.

But don't solve this by putting `.ToList()` after every LINQ call.

Understand whether you need materialisation.

---

# 34. Side effects in LINQ are dangerous territory

Technically:

```
var query = applications.Where(a =>
{
    Console.WriteLine(a.Position);
    return a.Remote;
});
```

works.

But now the logging happens **when the query is enumerated**, potentially more than once.

Even worse would be mutating domain state inside predicates.

Prefer LINQ expressions that describe transformations and conditions without surprising side effects.

This is easier to reason about:

```
.Where(a => a.Remote)
```

than:

```
.Where(a =>
{
    a.MarkAsReviewed();
    return a.Remote;
})
```

LINQ is generally clearest when query operations behave like pure transformations.

---

# 35. Exercise — deferred execution

Predict the exact output:

```
List<int> numbers = [1, 2, 3];

var query = numbers.Where(n =>
{
    Console.WriteLine($"Checking {n}");
    return n > 1;
});

Console.WriteLine("Query created");

foreach (var number in query)
{
    Console.WriteLine($"Result {number}");
}

Console.WriteLine("Again");

foreach (var number in query)
{
    Console.WriteLine($"Result {number}");
}
```

Don't run it first.

Write the complete expected output.

Then run it and compare.

This exercise matters more than memorising another five LINQ operators.

---

# 36. Query syntax vs method syntax

C# also supports:

```
var result =
    from application in applications
    where application.Remote
    orderby application.DateDiscovered
    select application;
```

instead of:

```
var result = applications
    .Where(a => a.Remote)
    .OrderBy(a => a.DateDiscovered);
```

Both are LINQ.

The query-expression syntax is translated by the compiler into query method calls.

You should be able to read both, but for our work we'll mostly use **method syntax**, because:

- it's extremely common in production C#;
- all LINQ operators fit naturally into it;
- it exposes the pipeline clearly;
- you'll encounter it heavily with EF Core.

Microsoft documents both syntaxes and notes they can be mixed. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/linq/?view=net-7.0&utm_source=chatgpt.com)

---

# 37. `IEnumerable<T>` versus `IQueryable<T>`

We're only introducing this now.

You do **not** need to master `IQueryable<T>` yet.

Consider:

```
IEnumerable<JobApplication>
```

LINQ to Objects works by executing .NET code against objects.

A lambda such as:

```
a => a.Remote
```

can become a delegate:

```
JobApplication → bool
```

But later, with EF Core, we might have:

```
IQueryable<JobApplication>
```

Now something very different can happen.

The expression:

```
applications.Where(a => a.Remote)
```

might ultimately become approximately:

```
SELECT ...
FROM JobApplications
WHERE Remote = 1
```

How?

Because the lambda can be represented as an **expression tree**: data describing the expression rather than merely executable code.

Microsoft's documentation distinguishes these directly: `IEnumerable<T>` queries compile their lambdas to delegates, while `IQueryable<T>` queries can use expression trees that a provider interprets. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions?utm_source=chatgpt.com)

This distinction will become very important with EF Core.

For now remember:

```
IEnumerable<T>
      ↓
LINQ to Objects
      ↓
execute C# against objects

IQueryable<T>
      ↓
query provider
      ↓
expression can potentially be translated
      ↓
SQL, etc.
```

This is one reason understanding LINQ now will make EF Core much easier later.

---

# 38. Main exercise — JobTracker queries

Now work against the JobTracker model you've built.

Add enough test data to make the queries meaningful: roughly **8–12 applications**, multiple companies, different statuses, salaries, dates, remote values and required skills.

Don't create another application architecture. A console program with test objects is enough.

Implement the following queries.

### Query 1 — Remote opportunities

Return all remote applications ordered by discovery date, newest first.

---

### Query 2 — Active C# opportunities

Find applications that:

```
are not rejected
AND
require "C#"
```

Do this with LINQ.

Think about whether skill comparison should be case-sensitive. You don't have to solve every internationalisation problem—just make an explicit decision.

---

### Query 3 — Application summary

Produce a sequence containing only:

```
Company name
Position
Status
Remote
```

Do **not** return the complete `JobApplication`.

Choose either an anonymous type or create a small explicit type if you think it improves the design.

---

### Query 4 — Companies

Return each company for which there is at least one application.

Each company should appear only once.

Think about Topic 4 equality.

---

### Query 5 — Applications by status

Produce groups by:

```
ApplicationStatus
```

and print:

```
Interested: X
Applied: X
Rejected: X
```

Use `GroupBy`.

---

### Query 6 — All required technologies

From every application's skills, produce:

```
one flat sequence
of unique skills
ordered alphabetically
```

This should naturally make you consider:

```
SelectMany
Distinct
OrderBy
```

Don't treat that as an instruction that you must use all three if your existing model makes one unnecessary.

---

### Query 7 — Salary

Find the application with the highest maximum salary.

Decide what should happen if there are no applications.

Don't blindly use an operator before thinking about the empty-sequence case.

---

### Query 8 — Multiple applications per company

Find companies for which you've tracked **more than one application**.

This requires more reasoning.

Think:

```
applications
     ↓
group by ?
     ↓
which groups satisfy ?
     ↓
what should final result contain?
```

---

### Query 9 — Skill frequency

Produce something conceptually like:

```
C#              6
ASP.NET Core    4
React           3
Docker          5
Azure           2
```

across all applications.

This combines:

```
flatten
group
project/count
order
```

Try to derive the pipeline yourself.

---

# 39. Main exercise — second part: query design

For each of your nine queries, write a short comment answering:

```
1. What is the source element type?

2. What does each operation change?
   Filtering?
   Ordering?
   Projection?
   Grouping?
   Flattening?

3. What is the final element type?

4. Is execution deferred?

5. If I add ToList(), what changes?
```

You don't need an essay.

Example style:

```
Source: IEnumerable<JobApplication>
Where: filters; element type unchanged
Select: JobApplication → string
Final: IEnumerable<string>
Deferred: yes
ToList: executes query and stores List<string>
```

This is the part that turns LINQ from syntax recognition into understanding.

---

# 40. Refactoring exercise — loop versus LINQ

Write this once using ordinary control flow:

> Get the positions of remote applications discovered during the last 30 days, ordered alphabetically.
> 

Then implement it using LINQ.

Compare them.

Don't automatically conclude:

> LINQ version shorter → LINQ version better.
> 

Instead ask:

- Which communicates intent more clearly?
- Which is easier to modify?
- Is the LINQ pipeline obvious?
- Would a junior developer understand either version?
- Did LINQ eliminate incidental mechanics?

LINQ is a tool for expressing queries, not a code-golf competition.

---

# 41. Delegate exercise — without LINQ

Before finishing, prove that you understand the layer underneath LINQ.

Write:

```
static IEnumerable<JobApplication> FilterApplications(
    IEnumerable<JobApplication> applications,
    Func<JobApplication, bool> predicate)
```

Implement the filtering yourself with:

```
foreach
yield return
```

I haven't taught `yield return` yet in this lesson deliberately.

Look it up in the official C# documentation and work out what it does.

The result should let you write something like:

```
var remote = FilterApplications(
    applications,
    a => a.Remote);
```

Then:

```
var applied = FilterApplications(
    applications,
    a => a.CurrentStatus == ApplicationStatus.Applied);
```

The point isn't to replace `Where`.

It's to understand why passing a predicate is powerful and how an enumerable can produce elements lazily.

---

# 42. Optional challenge — generic filter

If the previous exercise is straightforward, change:

```
FilterApplications
```

into something capable of filtering:

```
JobApplication
Company
int
string
anything else
```

without duplicating the method.

Don't look up the implementation immediately.

Ask yourself what part of:

```
Func<JobApplication, bool>
```

and:

```
IEnumerable<JobApplication>
```

needs to become variable.

This previews **Topic 8 — Generics** using something concrete.

---

# 43. Knowledge check

Answer these without looking back first.

**1.** What is a delegate?

**2.** What does this mean?

```
Func<JobApplication, bool>
```

**3.** What's the difference between `Func<>` and `Action<>`?

**4.** What is a lambda expression?

**5.** What does this lambda mean?

```
a => a.Company.Name
```

**6.** What's the conceptual difference between `Where` and `Select`?

**7.** Why can `Select` change the element type of a sequence?

**8.** What's the difference between `Select` and `SelectMany`?

**9.** When would you use `Any` rather than `Count() > 0`?

**10.** What's the semantic difference between `FirstOrDefault` and `SingleOrDefault`?

**11.** What does `GroupBy` produce conceptually?

**12.** What is deferred execution?

**13.** Does this necessarily execute the predicate?

```
var result = applications.Where(a => a.Remote);
```

**14.** Name two things that could cause that query to execute.

**15.** What does `ToList()` change?

**16.** Why can enumerating the same deferred query twice matter?

**17.** Why should side effects inside LINQ predicates generally make you suspicious?

**18.** Why can LINQ operate on a `List<T>`, array or `HashSet<T>` using the same operators?

**19.** At a high level, what's the difference between `IEnumerable<T>` and `IQueryable<T>`?

**20.** Why will that distinction matter when we reach EF Core?

---

# 44. Code-reading test

Predict this completely before running it:

```
List<int> numbers = [1, 2, 3, 4, 5];

var query = numbers
    .Where(n => n > 2)
    .Select(n => n * 10);

numbers.Add(6);

Console.WriteLine("First:");

foreach (var number in query)
{
    Console.WriteLine(number);
}

numbers.Remove(3);

Console.WriteLine("Second:");

foreach (var number in query)
{
    Console.WriteLine(number);
}
```

Answer:

1. Exact first output.
2. Exact second output.
3. Why the results differ.
4. What adding `.ToList()` immediately after `Select()` would change.

---

# 45. A slightly nastier code-reading test

Without running:

```
List<int> numbers = [1, 2, 3];

var query = numbers.Where(n =>
{
    Console.WriteLine($"Testing {n}");
    return n >= 2;
});

var first = query.Count();
var second = query.Count();

Console.WriteLine($"{first}, {second}");
```

How many times does:

```
Testing ...
```

appear?

And why?

This is exactly the kind of LINQ behaviour worth noticing during code review.

---

# 46. Exit drill

Write from memory:

```
1. Func<int, bool> containing a lambda that tests whether a number is positive.

2. Action<string> that prints its argument.

3. Filter a List<int> to values > 10.

4. Transform those values into strings.

5. Order descending.

6. Check whether any value is 100.

7. Get the first value safely when the sequence may be empty.

8. Materialise a LINQ query into a List<int>.

9. Flatten a collection of collections.

10. Group JobApplication objects by status.
```

This should be a short syntax-retrieval exercise, not another project.

---

# 47. What I want you to retain

The most useful mental model from Topic 5 is this:

```
IEnumerable<T>
      │
      ▼
  LINQ operator
      │
      ├── receives behaviour as lambdas
      │
      ▼
IEnumerable<T> or IEnumerable<TResult>
      │
      ▼
 another operator
      │
      ▼
 query pipeline
      │
      │ enumeration / ToList / scalar operator
      ▼
    EXECUTION
```

And underneath a call like:

```
applications.Where(a => a.Remote)
```

you should now see:

```
sequence
+
function: JobApplication → bool
+
reusable filtering algorithm
```

rather than thinking of `Where` as special C# syntax.

That understanding becomes especially valuable when we eventually go from:

```
IEnumerable<JobApplication>
```

to:

```
IQueryable<JobApplication>
```

and the apparently same LINQ syntax starts producing database queries.

## Official references

The main current references for this topic are [Microsoft Learn — LINQ overview](https://learn.microsoft.com/en-us/dotnet/csharp/linq/?utm_source=chatgpt.com), [Introduction to LINQ queries and execution](https://learn.microsoft.com/dotnet/csharp/linq/get-started/introduction-to-linq-queries?utm_source=chatgpt.com), [Delegates and lambda expressions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/delegates-lambdas?utm_source=chatgpt.com), and [Lambda expression reference](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions?utm_source=chatgpt.com). These are also the sources I checked against the current documentation when building this lesson. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/delegates-lambdas?utm_source=chatgpt.com)

For this topic, I'd particularly avoid rushing through **sections 3–13 and 29–35**. The operators themselves will probably come back to you quickly; delegates, `IEnumerable<T>`, deferred execution and materialisation are the parts worth making sure you actually understand.