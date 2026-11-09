# 8. Generics & Abstractions

You've already used generics throughout the course:

```
List<JobApplication>
Dictionary<Guid, Company>
IEnumerable<JobApplication>
Func<JobApplication, bool>
```

So this unit is not about discovering `<T>` for the first time.

The goal is to understand:

- what generic types and methods actually are;
- why they exist;
- how constraints work;
- how generics interact with interfaces and abstractions;
- what covariance and contravariance mean at a practical level;
- a few related modern C# tools such as tuples, deconstruction and `nameof`.

This should be a compact unit.

---

# 1. Retrieval warm-up

Short answers.

### 1.1

What does this mean?

```
List<JobApplication>
```

What role does `JobApplication` play?

### 1.2

What's the difference between:

```
IEnumerable<JobApplication>
```

and:

```
List<JobApplication>
```

in terms of what the caller is allowed to assume?

### 1.3

You've already used:

```
Func<JobApplication, bool>
```

What does that delegate represent?

### 1.4

Why might this be preferable:

```
IEnumerable<JobApplication>
```

to returning:

```
List<JobApplication>
```

from a method that only promises enumeration?

---

# 2. Why generics exist

Imagine we want a collection of job applications.

Without generics, one crude possibility would be:

```
List<object>
```

Then:

```
var items = new List<object>();

items.Add(new JobApplication(...));
items.Add("hello");
items.Add(42);
```

The collection can contain almost anything.

When retrieving values, the compiler cannot give us useful type guarantees.

Generics let us parameterise a type with another type:

```
List<JobApplication>
```

Now the compiler knows:

> this list contains `JobApplication` values.
> 

So:

```
applications.Add(jobApplication);
```

is allowed.

But:

```
applications.Add("hello");
```

is rejected at compile time.

The core benefit is:

> **reusable code without giving up static type safety.**
> 

---

# 3. Generic types

A generic type declares one or more **type parameters**.

For example:

```
public class Box<T>
{
    public T Value { get; }

    public Box(T value)
    {
        Value = value;
    }
}
```

`T` is a placeholder for a type.

We can create:

```
var numberBox = new Box<int>(42);
var textBox = new Box<string>("hello");
```

The same type definition works with different concrete types.

Conceptually:

```
Box<T>
   ↓
Box<int>
Box<string>
Box<JobApplication>
```

---

## Type parameters are types

Inside:

```
public class Box<T>
```

`T` can be used wherever a type is expected:

```
public T Value { get; }

public void Replace(T value)
{
    ...
}
```

It is not a variable containing a runtime value.

It is a **type parameter**.

---

# 4. Multiple type parameters

A generic type can have several type parameters.

You already know:

```
Dictionary<TKey, TValue>
```

Conceptually:

```
public class Something<TKey, TValue>
{
    ...
}
```

When used as:

```
Dictionary<Guid, JobApplication>
```

we get:

```
TKey   → Guid
TValue → JobApplication
```

So:

```
Guid
```

is the key type and:

```
JobApplication
```

is the value type.

---

# 5. Generic methods

A method can also introduce its own type parameter.

For example:

```
static T First<T>(IEnumerable<T> items)
{
    return items.First();
}
```

We can call:

```
var number = First(new[] { 1, 2, 3 });

var name = First(new[] { "Alice", "Bob" });
```

The compiler usually infers `T`.

For the first call:

```
T = int
```

For the second:

```
T = string
```

So you normally don't need to write:

```
First<int>(...)
```

unless inference cannot determine the type.

---

# 6. Why not just use `object`?

Compare:

```
static object First(IEnumerable<object> items)
```

with:

```
static T First<T>(IEnumerable<T> items)
```

The generic version preserves type information.

If you call:

```
var application =
    First(applications);
```

and `applications` is:

```
IEnumerable<JobApplication>
```

the compiler knows:

```
application
```

is a:

```
JobApplication
```

No cast is required.

Generics let reusable APIs remain strongly typed.

---

# 7. Generic-method check

Predict the inferred type of `T` in each call:

```
var a = First(new[] { 1, 2, 3 });

var b = First(new[]
{
    new Company(...),
    new Company(...)
});

var c = First(new List<string>
{
    "C#",
    "SQL"
});
```

For each:

```
T = ?
```

---

# 8. The limitation of an unconstrained `T`

Suppose:

```
static void PrintId<T>(T value)
{
    Console.WriteLine(value.Id);
}
```

This does not compile.

Why?

Because for an arbitrary `T`, the compiler cannot assume:

```
T has an Id property
```

`T` could be:

```
int
string
DateTime
Company
JobApplication
```

The compiler only knows what the generic declaration guarantees.

This is where **constraints** become useful.

---

# 9. Generic constraints

A constraint says:

> `T` is not completely arbitrary; it must satisfy this requirement.
> 

Suppose:

```
public interface IHasId
{
    Guid Id { get; }
}
```

Then:

```
static Guid GetId<T>(T value)
    where T : IHasId
{
    return value.Id;
}
```

Now the compiler knows that every valid `T` provides:

```
Id
```

So this works.

---

# 10. `where`

Generic constraints use the `where` syntax:

```
where T : SomeConstraint
```

For example:

```
static void Print<T>(T value)
    where T : IHasId
{
    Console.WriteLine(value.Id);
}
```

Read:

```
where T : IHasId
```

as:

> `T` must implement `IHasId`.
> 

This gives the generic implementation permission to use members guaranteed by that interface.

---

# 11. Common constraints

You will see constraints such as:

```
where T : class
```

Meaning:

> `T` must be a reference type.
> 

And:

```
where T : struct
```

Meaning:

> `T` must be a non-nullable value type.
> 

And:

```
where T : BaseClass
```

Meaning:

> `T` must derive from `BaseClass`.
> 

And:

```
where T : ISomething
```

Meaning:

> `T` must implement that interface.
> 

You may also encounter:

```
where T : new()
```

which requires an accessible parameterless constructor.

Example:

```
static T Create<T>()
    where T : new()
{
    return new T();
}
```

---

# 12. Multiple constraints

Constraints can be combined:

```
static T CreateEntity<T>()
    where T : class, IHasId, new()
{
    return new T();
}
```

Now `T` must:

- be a reference type;
- implement `IHasId`;
- have an accessible parameterless constructor.

Do not add constraints merely because you can.

The constraint should express what the generic code genuinely requires.

---

# 13. Constraint exercise

Suppose:

```
public interface IHasName
{
    string Name { get; }
}
```

Write a generic method:

```
static string GetName<T>(T value)
```

that is only valid for types implementing `IHasName`.

Then answer:

> Why is the constraint useful beyond simply documenting intent?
> 

---

# 14. Generics and abstraction

Generics become especially useful when combined with interfaces.

Suppose we want a repository abstraction:

```
public interface IRepository<T>
{
    void Add(T item);

    T? GetById(Guid id);

    IEnumerable<T> GetAll();
}
```

Now we can express:

```
IRepository<JobApplication>
```

or:

```
IRepository<Company>
```

The abstraction is reusable while still preserving the entity type.

This is a common pattern in professional C#.

But it does **not** mean every class needs a generic repository.

The design still needs a reason.

---

# 15. Generic abstraction versus unnecessary abstraction

This can be useful:

```
IRepository<JobApplication>
```

if several implementations genuinely need the same contract.

For example:

```
InMemoryJobApplicationRepository
SqlJobApplicationRepository
```

But creating:

```
IRepository<T>
IService<T>
IManager<T>
IHandler<T>
```

for everything simply because generics exist can make a codebase harder to understand.

Generics reduce duplication when several types genuinely share a common algorithm or contract.

They do not automatically make a design better.

---

# 16. Extension methods revisited

You've already used extension methods extensively through LINQ.

For example:

```
applications.Where(...)
```

An extension method is declared using `this` on its first parameter:

```
public static class JobApplicationExtensions
{
    public static IEnumerable<JobApplication>
        Active(
            this IEnumerable<JobApplication> applications)
    {
        return applications.Where(
            a => a.CurrentStatus ==
                 ApplicationStatus.Applied);
    }
}
```

Now:

```
applications.Active()
```

looks like an instance method even though it is a static method.

---

## Generic extension methods

Extension methods can also be generic.

Example:

```
public static bool IsEmpty<T>(
    this IEnumerable<T> source)
{
    return !source.Any();
}
```

Now it works for:

```
IEnumerable<JobApplication>
IEnumerable<Company>
IEnumerable<string>
IEnumerable<int>
```

because the behaviour does not depend on a particular element type.

This is exactly the kind of problem generics are good at solving.

---

# 17. Delegates and generics revisited

You've already used:

```
Func<T, bool>
```

through LINQ-style APIs.

For example:

```
static IEnumerable<T> Filter<T>(
    IEnumerable<T> source,
    Func<T, bool> predicate)
{
    foreach (var item in source)
    {
        if (predicate(item))
        {
            yield return item;
        }
    }
}
```

Now:

```
Filter(
    applications,
    a => a.Remote);
```

works without `Filter` needing to know anything specifically about `JobApplication`.

Its algorithm is:

```
for each T
    ask predicate whether to keep it
```

This combination of:

- generics;
- delegates;
- interfaces;

is a major part of how APIs such as LINQ achieve reuse.

---

# 18. Variance: the problem

Now for a concept that often sounds more complicated than it needs to.

Suppose:

```
public class Animal
{
}

public class Dog : Animal
{
}
```

A `Dog` is an `Animal`.

So this is valid:

```
Animal animal = new Dog();
```

But what about:

```
List<Dog> dogs = [];
List<Animal> animals = dogs;
```

This is **not** allowed.

Why?

Imagine it were:

```
animals.Add(new Cat());
```

Then a `List<Dog>` would suddenly contain a `Cat`.

That would break type safety.

So:

```
Dog → Animal
```

does not imply:

```
List<Dog> → List<Animal>
```

`List<T>` is invariant.

---

# 19. Covariance

Some generic interfaces can safely support conversion in the output direction.

For example:

```
IEnumerable<Dog>
```

can be treated as:

```
IEnumerable<Animal>
```

Why is this safe?

Because `IEnumerable<T>` fundamentally **produces** `T` values.

If something produces dogs, every value it produces is also an animal.

Conceptually:

```
IEnumerable<Dog>
        ↓
can be viewed as
        ↓
IEnumerable<Animal>
```

This is **covariance**.

In declarations, you'll see:

```
out T
```

For example, conceptually:

```
interface IEnumerable<out T>
```

`out` tells us that `T` is used covariantly.

For this course, the main goal is recognition and reasoning, not designing sophisticated variant APIs.

---

# 20. Contravariance

The reverse idea can apply to consumers.

Suppose something can handle **any Animal**:

```
Action<Animal>
```

Then it can certainly handle a `Dog`.

So an `Action<Animal>` can be used where:

```
Action<Dog>
```

is required.

Why?

Because every dog passed to it is an animal.

This direction is called **contravariance**.

You'll see:

```
in T
```

in suitable generic interface/delegate declarations.

A useful simplified model is:

```
out T
    → produces T
    → covariance

in T
    → consumes T
    → contravariance
```

Do not try to derive every variance rule from memory yet.

The important professional skill is recognising what the compiler is protecting.

---

# 21. Variance check

Given:

```
class Animal { }
class Dog : Animal { }
```

Which should be valid?

### A

```
Animal a = new Dog();
```

### B

```
List<Animal> animals =
    new List<Dog>();
```

### C

```
IEnumerable<Animal> animals =
    new List<Dog>();
```

For each one, explain why.

---

# 22. Tuples

C# can return multiple related values without immediately creating a dedicated type.

Example:

```
static (int Applied, int Rejected)
    CountApplications(
        IEnumerable<JobApplication> applications)
{
    var applied = applications.Count(
        a => a.CurrentStatus ==
             ApplicationStatus.Applied);

    var rejected = applications.Count(
        a => a.CurrentStatus ==
             ApplicationStatus.Rejected);

    return (applied, rejected);
}
```

Then:

```
var counts = CountApplications(applications);

Console.WriteLine(counts.Applied);
Console.WriteLine(counts.Rejected);
```

This is a **tuple**.

Tuples are useful for small, local groupings of related values.

If the concept has meaningful domain behaviour or travels widely through the application, a named type is often clearer.

---

# 23. Deconstruction

Tuples can be deconstructed:

```
var (applied, rejected) =
    CountApplications(applications);
```

Now we directly have:

```
applied
rejected
```

Some types can also define their own deconstruction behaviour.

You should recognise this syntax, but we do not need a large exercise around it.

---

# 24. `nameof`

`nameof` returns the name of a symbol as a string.

Example:

```
nameof(application)
```

produces:

```
"application"
```

You have already implicitly encountered the use case in:

```
throw new ArgumentNullException(
    nameof(application));
```

This is better than:

```
throw new ArgumentNullException(
    "application");
```

because `nameof` is tied to the actual symbol.

If the variable is renamed by a refactoring tool, `nameof` remains correct.

It is a small feature, but one you'll see constantly in professional C#.

---

# 25. Main JobTracker exercise

Create a small generic in-memory repository.

Start from this requirement:

```
public interface IEntity
{
    Guid Id { get; }
}
```

Assume:

```
JobApplication
Company
```

can implement `IEntity`.

Create:

```
public class InMemoryRepository<T>
```

with an appropriate constraint.

It should internally store items and provide:

```
void Add(T item)

T? GetById(Guid id)

IEnumerable<T> GetAll()
```

Requirements:

- `T` must expose an `Id`;
- callers must not be able to directly mutate the repository's internal collection;
- `GetById` should return an appropriate result when the entity does not exist;
- do not introduce extra abstractions beyond what the exercise needs.

Then instantiate:

```
InMemoryRepository<JobApplication>
InMemoryRepository<Company>
```

and verify that the same repository implementation works for both.

---

## Design questions

After implementing it, answer:

1. Why is this better than implementing the repository around `object`?
2. What does the generic constraint give the implementation permission to do?
3. Why return:

```
IEnumerable<T>
```

rather than exposing the internal:

```
List<T>
```

?

1. Would you automatically introduce this generic repository into a real EF Core application?

Why or why not?

That final question is deliberately about engineering judgement rather than C# syntax.

---

# 26. Exit test

### 1

What problem do generics solve?

### 2

What is the difference between:

```
class Box<T>
```

and:

```
static T First<T>(...)
```

?

### 3

Why can't unconstrained generic code do this?

```
value.Id
```

### 4

What does:

```
where T : IEntity
```

guarantee?

### 5

What does:

```
where T : class
```

mean?

### 6

Why is:

```
List<Dog>
```

not assignable to:

```
List<Animal>
```

even when `Dog : Animal`?

### 7

Why can:

```
IEnumerable<Dog>
```

be used as:

```
IEnumerable<Animal>
```

?

### 8

At recognition level, what do these suggest?

```
out T
in T
```

### 9

When might a tuple be preferable to creating a new class or record?

### 10

Why use:

```
nameof(application)
```

instead of:

```
"application"
```

?

---

# 27. Durable mental model

```
Generics
    → reusable code
    + compile-time type safety

T
    → type parameter

Constraints
    → tell the compiler what T guarantees

where T : Interface
    → generic code may use that interface contract

Interfaces + generics
    → reusable typed abstractions

Generic extension methods
    → reusable operations over many types

List<Dog> ≠ List<Animal>
    → mutable generic types may be invariant

out T
    → produces T
    → covariance

in T
    → consumes T
    → contravariance

Tuple
    → small local grouping of values

nameof
    → compiler-aware symbol name

Use generics when behaviour genuinely generalises.
Do not create abstractions merely because the language allows them.
```