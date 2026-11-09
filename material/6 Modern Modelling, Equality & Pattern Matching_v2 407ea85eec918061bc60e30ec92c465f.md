# 6. Modern Modelling, Equality & Pattern Matching_v2

This unit resolves several questions we've deliberately left open in JobTracker.

Until now we've used classes mainly to represent behaviour and state. We've also seen that collections such as `HashSet<T>`, `Dictionary<TKey,TValue>` and operations such as `Distinct()` depend on some notion of equality.

Now we need to make those ideas more precise.

The central modelling question is:

> **When does an object have an identity of its own, and when does it simply represent a value?**
> 

That leads us into:

- reference types and value types;
- `struct`;
- reference identity and value equality;
- entities and value objects;
- records;
- immutability;
- `with`;
- `record struct`;
- pattern matching.

We'll eventually use all of this to reconsider `Company`, `SalaryRange` and `JobApplication`.

But first we need the language semantics required to make those decisions.

---

# 1. Retrieval warm-up

Short answers. No research.

### 1.1

Given:

```
var first = new Company("Microsoft", "microsoft.com");
var second = first;
```

If `Company` is a class and we later write:

```
second.Name = "Microsoft Corporation";
```

what do you expect `first.Name` to contain?

Why?

### 1.2

What's the difference between:

```
public string Name { get; set; }
```

```
public string Name { get; private set; }
```

```
public string Name { get; init; }
```

```
public string Name { get; }
```

### 1.3

From the collections unit:

```
HashSet<Company> companies = [];
```

What property of a `HashSet<T>` makes equality important?

### 1.4

What's wrong with this statement?

> "`readonly List<JobApplication>` means nobody can modify the list."
> 

---

# 2. Reference types and value types

This distinction is required before we can understand records properly.

## Reference types

A class is a **reference type**.

Consider:

```
public class Company
{
    public string Name { get; set; } = "";
}
```

Then:

```
var first = new Company
{
    Name = "Microsoft"
};

var second = first;
```

The assignment:

```
var second = first;
```

does **not** create a second `Company`.

It copies the reference.

Conceptually:

```
first ─────┐
           ▼
      Company object
      Name = Microsoft
           ▲
second ────┘
```

Therefore:

```
second.Name = "Microsoft Corporation";

Console.WriteLine(first.Name);
```

prints:

```
Microsoft Corporation
```

Both variables refer to the same object.

This is **reference semantics**.

---

## Value types

Now compare:

```
int first = 10;
int second = first;

second = 20;
```

Now:

```
Console.WriteLine(first);
Console.WriteLine(second);
```

prints:

```
10
20
```

Here the value itself was copied.

Conceptually:

```
first  = 10
second = 20
```

The two variables are independent after the assignment.

This is **value semantics**.

Common value types include:

```
int
bool
double
decimal
DateTime
Guid
enum
```

And C# lets us define our own value types using `struct`.

---

# 3. `struct` and copy semantics

A `struct` can contain properties, methods and constructors just like other C# types.

For example:

```
public struct Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
}
```

Now:

```
var first = new Coordinate
{
    X = 10,
    Y = 20
};

var second = first;

second.X = 100;
```

What happens?

```
Console.WriteLine(first.X);
Console.WriteLine(second.X);
```

prints:

```
10
100
```

Why?

Because:

```
var second = first;
```

copied the `Coordinate` value.

Compare the two models.

### Class

```
first ─────┐
           ▼
         object
           ▲
second ────┘
```

### Struct

```
first               second
┌─────────┐         ┌─────────┐
│ X = 10  │         │ X = 100 │
│ Y = 20  │         │ Y = 20  │
└─────────┘         └─────────┘
```

The important mental model is therefore:

```
class
    → reference type
    → assignment copies a reference

struct
    → value type
    → assignment copies a value
```

Do not reduce this to:

> classes are on the heap, structs are on the stack.
> 

That is not a reliable semantic model. A struct can, for example, exist as part of an object that is itself stored on the heap.

What matters here is **copying and identity semantics**, not memorising storage locations.

### Quick check

Predict:

```
public class CounterClass
{
    public int Value { get; set; }
}

public struct CounterStruct
{
    public int Value { get; set; }
}

var classA = new CounterClass { Value = 10 };
var classB = classA;

var structA = new CounterStruct { Value = 10 };
var structB = structA;

classB.Value = 20;
structB.Value = 20;

Console.WriteLine(classA.Value);
Console.WriteLine(classB.Value);
Console.WriteLine(structA.Value);
Console.WriteLine(structB.Value);
```

---

# 4. Identity and equality

Now we can distinguish two different questions.

## Reference identity

For reference types, we can ask:

> Do these two variables refer to the same object?
> 

Consider:

```
var first = new Company
{
    Name = "Microsoft"
};

var second = first;
```

`first` and `second` refer to the same object.

Now compare:

```
var first = new Company
{
    Name = "Microsoft"
};

var second = new Company
{
    Name = "Microsoft"
};
```

These are two separate objects.

They happen to contain the same `Name`, but they are not the same instance.

---

## Value equality

Sometimes object identity is not what we care about.

Suppose:

```
var first =
    new SalaryRange(50_000m, 60_000m);

var second =
    new SalaryRange(50_000m, 60_000m);
```

For a salary range, the meaningful question is probably:

> Do these represent the same salary range?
> 

The answer appears to be yes.

Both represent:

```
€50,000 – €60,000
```

This is **value equality**.

So:

```
reference identity
```

and:

```
value equality
```

are different concepts.

---

# 5. Why equality matters beyond `==`

Equality matters to collections too.

From Unit 4:

```
HashSet<T>
```

represents unique membership.

If we do:

```
companies.Add(first);
companies.Add(second);
```

the set needs to determine whether `second` is already represented.

Likewise:

```
Dictionary<TKey, TValue>
```

must determine whether a key is equivalent to an existing key.

And LINQ operations such as:

```
Distinct()
```

also depend on equality.

So equality semantics affect real program behaviour.

They are not merely an academic discussion about the `==` operator.

---

# 6. Entities and value objects

Now we can connect the language concepts to domain modelling.

These terms are **domain concepts**, not C# keywords.

## Entity

An entity has an identity that matters over time.

Imagine:

```
Company #123
Name = Microsoft
Website = microsoft.com
```

Later:

```
Company #123
Name = Microsoft Corporation
Website = microsoft.com
```

Its data changed.

But conceptually it is still:

```
Company #123
```

The identity survived the state change.

A `JobApplication` is another likely example.

If you apply to the same company twice:

```
Application A — Microsoft — Developer — March
Application B — Microsoft — Developer — September
```

those can be two different applications even if some property values happen to match.

Identity matters.

---

## Value object

A value object is defined primarily by its meaningful components.

For example:

```
SalaryRange(50000, 60000)
SalaryRange(50000, 60000)
```

We probably do not care which physical instance is which.

They represent the same conceptual value.

Examples of things often modelled as values include:

```
Money
Coordinate
DateRange
Address
SalaryRange
```

depending on the domain.

---

## Important terminology

Do not confuse:

```
value object
```

with:

```
value type
```

A **value object** is a domain-modelling idea.

A **value type** is a C#/.NET language/runtime category.

Therefore:

```
value object ≠ struct
```

A domain value object can perfectly well be implemented as a reference type.

That distinction matters when we introduce records.

---

# 7. Ordinary classes and equality

Suppose `SalaryRange` is an ordinary class:

```
public class SalaryRange
{
    public SalaryRange(
        decimal minimum,
        decimal maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public decimal Minimum { get; }
    public decimal Maximum { get; }
}
```

Now:

```
var first =
    new SalaryRange(50_000m, 60_000m);

var second =
    new SalaryRange(50_000m, 60_000m);

Console.WriteLine(first == second);
```

For an ordinary class with no custom equality implementation, these are distinct references.

Conceptually:

```
first  ───→ SalaryRange { 50000, 60000 }

second ───→ SalaryRange { 50000, 60000 }
```

Same property values.

Different objects.

A class **can** implement value equality manually using mechanisms such as:

```
Equals
GetHashCode
IEquatable<T>
```

But modern C# provides another construct particularly suited to data where value equality is useful.

---

# 8. Records

A record can be declared very compactly:

```
public record SalaryRange(
    decimal Minimum,
    decimal Maximum);
```

A record is designed to provide useful semantics for data-oriented types, including generated **value-based equality**.

Now:

```
var first =
    new SalaryRange(50_000m, 60_000m);

var second =
    new SalaryRange(50_000m, 60_000m);

Console.WriteLine(first == second);
```

prints:

```
True
```

The two instances were independently created.

But their record components represent equal values.

This also affects:

```
HashSet<SalaryRange> ranges =
[
    new(50_000m, 60_000m),
    new(50_000m, 60_000m)
];
```

Because those records compare equal, the set has one logical member.

---

## Record does not mean value type

This distinction is critical.

This:

```
public record SalaryRange(
    decimal Minimum,
    decimal Maximum);
```

is a **reference type**.

More explicitly, it is a record class.

So:

```
class         → reference type
record class  → reference type

struct        → value type
record struct → value type
```

The interesting difference between ordinary classes and record classes is not:

```
reference versus value type
```

but generated semantics such as value-oriented equality.

---

# 9. Positional records, normal records and behaviour

This:

```
public record SalaryRange(
    decimal Minimum,
    decimal Maximum);
```

is a **positional record**.

It is concise and useful when a small set of values naturally defines the record.

But records do not have to be one-line data declarations.

We can write:

```
public record SalaryRange
{
    public decimal Minimum { get; init; }
    public decimal Maximum { get; init; }
}
```

And records can contain:

- constructors;
- methods;
- properties;
- validation;
- private members;
- domain behaviour.

So this is wrong:

> A record is just a short immutable DTO.
> 

Records are full C# types.

---

# 10. Immutability and invariants

Records work particularly well with immutable-style modelling, but `record` itself does not magically make every piece of state immutable.

Recall:

```
public decimal Minimum { get; init; }
```

An `init` property can be assigned during initialization but cannot normally be assigned later.

For example:

```
var salary = new SalaryRange
{
    Minimum = 50_000m,
    Maximum = 60_000m
};
```

But later:

```
salary.Minimum = 70_000m;
```

is not permitted if `Minimum` is `init`-only.

A getter-only property goes further:

```
public decimal Minimum { get; }
```

Its value normally has to be established internally, usually through a constructor.

That can be useful when protecting an **invariant**.

An invariant is a condition that must remain true for an object to be valid.

For `SalaryRange`:

```
Minimum >= 0
Maximum >= 0
Minimum <= Maximum
```

This is clearly invalid:

```
new SalaryRange(
    90_000m,
    20_000m);
```

The simple positional record:

```
public record SalaryRange(
    decimal Minimum,
    decimal Maximum);
```

does nothing to prevent it.

So brevity is not enough.

Our domain model still needs encapsulation.

---

## Validation in a record

A record can use an ordinary constructor:

```
public record SalaryRange
{
    public SalaryRange(
        decimal minimum,
        decimal maximum)
    {
        if (minimum < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimum));
        }

        if (maximum < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximum));
        }

        if (minimum > maximum)
        {
            throw new ArgumentException(
                "Minimum cannot exceed maximum.");
        }

        Minimum = minimum;
        Maximum = maximum;
    }

    public decimal Minimum { get; }
    public decimal Maximum { get; }
}
```

Now the type combines:

- controlled construction;
- protected invariants;
- immutable public state;
- record value equality.

This does not mean every record should be written exactly this way.

It demonstrates that record semantics and domain encapsulation can coexist.

---

# 11. `with` expressions and shallow copying

Records support `with` expressions.

For example:

```
public record SalaryRange(
    decimal Minimum,
    decimal Maximum);
```

Then:

```
var original =
    new SalaryRange(50_000m, 60_000m);

var improved = original with
{
    Maximum = 65_000m
};
```

`with` creates another record based on the first.

So:

```
Console.WriteLine(original.Maximum);
Console.WriteLine(improved.Maximum);
```

prints:

```
60000
65000
```

The original was not mutated.

This fits naturally with immutable modelling.

---

## But `with` is shallow

Consider:

```
public record Candidate(
    string Name,
    List<string> Skills);
```

Then:

```
var original = new Candidate(
    "Alice",
    ["C#", "SQL"]);
```

And:

```
var copy = original with
{
    Name = "Alice Smith"
};
```

The records are different objects.

But unless we explicitly replace `Skills`, both still refer to the same list.

Therefore:

```
copy.Skills.Add("Docker");
```

also changes what is visible through:

```
original.Skills
```

So:

> **`with` produces a shallow copy, not a recursive deep clone.**
> 

And similarly:

> **A record is not automatically deeply immutable.**
> 

This is the same basic reference-sharing problem we saw with collections earlier.

---

# 12. `record struct`

Now that both concepts are understood separately, `record struct` is straightforward.

Example:

```
public readonly record struct Coordinate(
    int X,
    int Y);
```

This combines:

- record-style generated equality;
- value-type semantics.

Therefore:

```
record
```

or:

```
record class
```

is a reference type.

Whereas:

```
record struct
```

is a value type.

A small value such as a coordinate can sometimes be a good candidate.

But do not conclude:

```
domain value object
        ↓
record struct
```

A domain value object can perfectly well be a record class.

Use a struct because you genuinely want value-type semantics.

For JobTracker there is no obvious reason yet to optimise something like `SalaryRange` around struct copying.

Understanding `record struct` is important.

Using it everywhere is not.

---

# 13. First JobTracker design decision

Now consider:

```
JobApplication
Company
SalaryRange
```

Before choosing C# syntax, classify each as:

```
Entity
Value object
Not sure
```

Think in this order:

```
What does the thing mean?
        ↓
Does identity matter?
        ↓
What equality semantics do we want?
        ↓
How should mutation work?
        ↓
Which C# representation fits?
```

### `JobApplication`

If you apply to the same position twice, those can be separate applications.

Its status can change:

```
Interested
Applied
Rejected
```

without it becoming a different application.

That strongly suggests entity semantics.

### `SalaryRange`

Two independently constructed:

```
50000–60000
```

ranges appear to represent the same conceptual value.

That strongly suggests value-object semantics.

### `Company`

This is more interesting.

A company can naturally have identity:

```
same company
different name
different website
```

But JobTracker might not currently require sophisticated company identity.

So `Company` requires an explicit domain decision.

Do not answer:

> Records are modern, so use a record.
> 

Decide what behaviour the domain actually needs.

---

# 14. Pattern matching

The second half of this unit deals with **pattern matching**.

You've already written conditions such as:

```
if (application.CurrentStatus ==
    ApplicationStatus.Applied)
{
    ...
}
```

That is perfectly fine.

Pattern matching becomes useful when a decision involves several possible values, object shapes or types.

We'll begin with the simplest useful form: switch expressions.

---

# 15. Switch expressions and `_`

Traditional code might be:

```
if (status == ApplicationStatus.Interested)
{
    return "Interested";
}
else if (status == ApplicationStatus.Applied)
{
    return "Applied";
}
else if (status == ApplicationStatus.Rejected)
{
    return "Rejected";
}
else
{
    return "Unknown";
}
```

A switch expression expresses the same decision more directly:

```
return status switch
{
    ApplicationStatus.Interested =>
        "Interested",

    ApplicationStatus.Applied =>
        "Applied",

    ApplicationStatus.Rejected =>
        "Rejected",

    _ =>
        "Unknown"
};
```

Read it as:

> Take `status`, match the first applicable pattern, and produce the corresponding result.
> 

Unlike the traditional `switch` statement, this is an **expression**.

It produces a value.

So:

```
var description = status switch
{
    ...
};
```

is natural.

---

## The discard pattern

This:

```
_
```

is the **discard pattern**.

In a switch expression it effectively means:

> anything not matched above.
> 

For example:

```
var description = number switch
{
    0 => "Zero",
    1 => "One",
    _ => "Something else"
};
```

The first matching arm wins.

That becomes important when patterns overlap.

### Exercise

Write:

```
static string GetStatusDescription(
    ApplicationStatus status)
```

using a switch expression.

Requirements:

```
Interested → "Not applied yet"
Applied    → "Application submitted"
Rejected   → "Application unsuccessful"
```

Decide what you want an unexpected enum value to do.

---

# 16. Relational and logical patterns

Patterns do not have to test exact values.

A **relational pattern** can use:

```
<
<=
>
>=
```

For example:

```
static string ClassifySalary(
    decimal salary) =>
    salary switch
    {
        < 30_000m => "Low",
        < 50_000m => "Medium",
        < 80_000m => "High",
        _ => "Very high"
    };
```

Notice the ordering.

We don't need:

```
>= 30_000m and < 50_000m
```

for the second arm.

If that arm is reached, the earlier:

```
< 30_000m
```

has already failed.

So the first matching arm wins.

Predict:

```
25,000
30,000
49,999
50,000
79,999
80,000
```

---

## Logical patterns

Patterns can be combined using:

```
and
or
not
```

For example:

```
salary switch
{
    >= 30_000m and < 50_000m =>
        "Medium",

    >= 50_000m and < 80_000m =>
        "High",

    _ =>
        "Other"
};
```

Or:

```
status is
    ApplicationStatus.Interested
    or ApplicationStatus.Applied
```

These are pattern combinators.

Inside pattern syntax use:

```
and
or
not
```

rather than:

```
&&
||
!
```

---

# 17. Property patterns

Pattern matching becomes particularly useful with domain objects.

Suppose `JobApplication` contains:

```
public bool Remote { get; }
public ApplicationStatus CurrentStatus { get; }
```

Traditional code:

```
if (application.Remote &&
    application.CurrentStatus ==
        ApplicationStatus.Applied)
{
    ...
}
```

A property pattern can express the same object state:

```
if (application is
    {
        Remote: true,
        CurrentStatus:
            ApplicationStatus.Applied
    })
{
    ...
}
```

Read it as:

> Does `application` have `Remote == true` and `CurrentStatus == Applied`?
> 

Property patterns can also appear inside switch expressions:

```
var description = application switch
{
    {
        CurrentStatus:
            ApplicationStatus.Rejected
    }
        => "Closed",

    {
        CurrentStatus:
            ApplicationStatus.Applied,
        Remote: true
    }
        => "Active remote application",

    {
        CurrentStatus:
            ApplicationStatus.Applied,
        Remote: false
    }
        => "Active onsite application",

    _ =>
        "Opportunity"
};
```

---

## Nested property patterns

Patterns can inspect nested state too.

Suppose:

```
JobApplication
    └── SalaryRange
            └── Maximum
```

Then conceptually we can match:

```
application is
{
    SalaryRange:
    {
        Maximum: >= 70_000m
    }
}
```

This combines:

- a property pattern;
- nesting;
- a relational pattern.

---

## Order matters

Suppose:

```
application switch
{
    {
        CurrentStatus:
            ApplicationStatus.Applied
    }
        => "Applied",

    {
        CurrentStatus:
            ApplicationStatus.Applied,
        Remote: true
    }
        => "Remote application",

    _ =>
        "Other"
};
```

The remote case is too late.

A remote applied application already matches the first arm.

So:

```
specific case
before
general case
```

is an important pattern-matching principle.

---

## Pattern matching is not automatically better

This:

```
if (application.Remote)
```

is better than:

```
if (application is { Remote: true })
```

when that is literally the only thing being checked.

Use pattern matching when it clarifies a structural or multi-case decision.

Do not use newer syntax merely to make code look modern.

---

# 18. Type patterns and null patterns

Pattern matching can also inspect runtime types.

Suppose:

```
public abstract class Contact
{
    public string Name { get; init; } = "";
}

public class Recruiter : Contact
{
    public string Agency { get; init; } = "";
}

public class HiringManager : Contact
{
    public string Company { get; init; } = "";
}
```

We could write:

```
if (contact is Recruiter recruiter)
{
    Console.WriteLine(recruiter.Agency);
}
```

This:

```
Recruiter recruiter
```

is a **type pattern**.

It checks the runtime type and gives us a correctly typed variable.

Inside a switch:

```
static string Describe(Contact contact) =>
    contact switch
    {
        Recruiter recruiter =>
            $"Recruiter from {recruiter.Agency}",

        HiringManager manager =>
            $"Hiring manager at {manager.Company}",

        _ =>
            "Contact"
    };
```

---

## Type + property pattern

We can combine them:

```
contact switch
{
    Recruiter { Agency: "Hays" } recruiter =>
        $"Hays recruiter: {recruiter.Name}",

    Recruiter recruiter =>
        $"Recruiter from {recruiter.Agency}",

    HiringManager manager =>
        $"Hiring manager at {manager.Company}",

    _ =>
        "Contact"
};
```

Again:

```
specific before general
```

The Hays recruiter arm must come before the general recruiter arm.

---

## Null patterns

Modern C# also allows:

```
if (application is null)
{
    ...
}
```

and:

```
if (application is not null)
{
    ...
}
```

Null can also be handled in a switch:

```
return application switch
{
    null =>
        "No application",

    {
        CurrentStatus:
            ApplicationStatus.Rejected
    }
        => "Closed",

    _ =>
        "Application"
};
```

---

# 19. List patterns

C# can also pattern-match sequences.

For example:

```
int[] numbers = [1, 2, 3];
```

This matches exactly:

```
if (numbers is [1, 2, 3])
{
    ...
}
```

This:

```
if (numbers is [_, _, _])
```

matches any sequence containing exactly three elements.

And:

```
if (numbers is [1, ..])
```

means approximately:

> starts with `1`, followed by zero or more elements.
> 

List patterns are useful to recognise, but they are less central to our current backend/domain work than:

- property patterns;
- type patterns;
- relational patterns;
- switch expressions.

So recognition-level understanding is enough for now.

---

# 20. Main JobTracker exercise

Now combine the modelling and pattern-matching parts of the unit.

Do not begin by blindly changing declarations.

First decide the semantics.

You need to reconsider:

```
Company
JobApplication
SalaryRange
ApplicationStatus
```

For each, choose among:

```
class
record class
struct
record struct
enum
```

and justify the decision using:

```
identity
equality
mutability
lifecycle
invariants
reference/value semantics
```

### Requirements

#### `SalaryRange`

- prevents negative values;
- prevents `Minimum > Maximum`;
- two equivalent salary ranges compare equal;
- external code cannot arbitrarily corrupt the range.

#### `JobApplication`

- has identity independent of its current state;
- changing status does not create a new application identity;
- domain state is not arbitrarily writable from outside.

#### `Company`

You must make an explicit decision:

```
entity
```

or:

```
value object
```

for the current needs of JobTracker.

There is room for engineering judgement here.

#### `ApplicationStatus`

Use an appropriate representation for a small closed set of named states.

---

Then demonstrate your equality choices.

For example:

```
salary1 == salary2
company1 == company2
application1 == application2
```

Before executing anything, predict the result and explain why.

---

Finally, implement:

```
static string Classify(
    JobApplication application)
```

using a switch expression where it genuinely helps.

Rules:

```
Rejected
    → "Closed"

Applied + Remote
    → "Active remote application"

Applied + not Remote
    → "Active onsite application"

Interested + salary maximum >= €70,000
    → "High-value opportunity"

Anything else
    → "Opportunity"
```

Use at least one property pattern.

Pay attention to arm ordering.

Do not add inheritance or extra abstractions merely to satisfy the exercise.

---

# 21. Exit test

Answer these without looking back initially.

### 1

What is the fundamental difference between assignment of a class variable and assignment of a struct variable?

### 2

Why is:

```
value object
```

not the same thing as:

```
value type
```

?

### 3

Is an ordinary:

```
record SalaryRange(...)
```

a reference type or a value type?

### 4

What is the difference between:

```
record
```

and:

```
record struct
```

?

### 5

Why might `SalaryRange` benefit from record equality while `JobApplication` may not?

### 6

Does `with` mutate the original record?

And does it necessarily create a deep copy?

### 7

Predict:

```
public record SalaryRange(
    decimal Minimum,
    decimal Maximum);

var first =
    new SalaryRange(50_000m, 70_000m);

var second =
    new SalaryRange(50_000m, 70_000m);

var third = first with
{
    Maximum = 80_000m
};

Console.WriteLine(first == second);
Console.WriteLine(first == third);
Console.WriteLine(first.Maximum);
Console.WriteLine(third.Maximum);
```

### 8

Explain which arm wins and why:

```
var description = application switch
{
    {
        CurrentStatus:
            ApplicationStatus.Rejected
    }
        => "Closed",

    {
        CurrentStatus:
            ApplicationStatus.Applied,
        Remote: true
    }
        => "Active remote",

    {
        SalaryRange:
        {
            Maximum: >= 70_000m
        }
    }
        => "High value",

    _ =>
        "Other"
};
```

for an application that is:

```
Applied
Remote
Maximum salary = €100,000
```

---

# 22. Durable mental model

The whole unit can be reduced to this:

```
class
    → reference type
    → assignment copies reference

struct
    → value type
    → assignment copies value

Entity
    → identity matters across state changes

Value object
    → meaningful values define equality

record class
    → reference type
    → generated value-oriented equality

record struct
    → value type
    → generated value-oriented equality

record ≠ automatic immutability
with = new shallow copy

HashSet / Dictionary / Distinct
    depend on equality

Pattern matching
    → describe values, types and object shapes

switch expressions:
    first matching arm wins

Use modern syntax when it makes domain decisions clearer,
not merely because it is modern.
```