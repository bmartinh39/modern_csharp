# 6. Modern Modelling, Equality & Pattern Matching

This topic resolves several questions we've deliberately left open in JobTracker.

The central question is:

> **When should something be an entity with identity, and when should it simply represent a value?**
> 

That leads naturally into records, equality and immutability. Then we'll use pattern matching to express domain decisions cleanly.

We'll do this in two halves:

**A. Modelling:** class vs record vs struct, identity, value equality, immutability, `with`.

**B. Pattern matching:** switch expressions, type/property/relational patterns and some list patterns.

We're not going to redesign JobTracker immediately. First you need enough theory to make the decisions yourself.

---

# 1. Retrieval warm-up

No research. Short answers are enough.

### 1.1

What is the important difference between these?

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

### 1.2

Given:

```
var first = new Company("Microsoft", "microsoft.com");
var second = new Company("Microsoft", "microsoft.com");
```

Assume `Company` is an ordinary class and doesn't override equality.

What would you expect?

```
first == second
```

And why?

### 1.3

From Topic 4:

```
HashSet<Company> companies = [];
companies.Add(first);
companies.Add(second);
```

How many elements would you expect the set to contain?

### 1.4

What's wrong with this reasoning?

> "`readonly List<JobApplication>` means nobody can modify the list."
> 

---

# 2. Identity versus value

This is the foundation for the entire topic.

Consider these two concepts:

```
Company
SalaryRange
```

Suppose we construct:

```
var company1 = new Company("Microsoft", "microsoft.com");
var company2 = new Company("Microsoft", "microsoft.com");
```

Are those necessarily the **same company**?

Not obviously. Perhaps companies eventually have:

```
Guid Id
```

and their names/websites can change while they're still the same entity.

Now consider:

```
var salary1 = new SalaryRange(50_000m, 60_000m);
var salary2 = new SalaryRange(50_000m, 60_000m);
```

Do you care whether these are literally the same object?

Probably not.

What matters is that they represent the same value:

```
50,000 – 60,000
```

This gives us two useful domain concepts.

**Entity:** identity matters over time.

```
Company #123
Borja changes its Name
Borja changes its Website

→ still Company #123
```

**Value object:** its attributes define what it is.

```
SalaryRange(50k, 60k)
SalaryRange(50k, 60k)

→ conceptually the same value
```

These aren't C# rules. They're modelling concepts.

C# gives us different tools for representing them.

---

# 3. Ordinary classes and equality

Consider:

```
public class SalaryRange
{
    public SalaryRange(decimal minimum, decimal maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public decimal Minimum { get; }
    public decimal Maximum { get; }
}
```

Then:

```
var a = new SalaryRange(50_000m, 60_000m);
var b = new SalaryRange(50_000m, 60_000m);

Console.WriteLine(a == b);
```

For an ordinary class with no custom equality implementation, these are different references.

Conceptually:

```
a ─────→ SalaryRange { 50000, 60000 }

b ─────→ SalaryRange { 50000, 60000 }
```

Same contents.

Different objects.

That's often exactly what we want for **entities**.

It's often inconvenient for **values**.

We *could* implement equality ourselves by overriding `Equals` and `GetHashCode`, implementing `IEquatable<T>`, etc.

But modern C# provides another useful tool.

---

# 4. Records

Compare:

```
public class SalaryRange
{
    public decimal Minimum { get; init; }
    public decimal Maximum { get; init; }
}
```

with:

```
public record SalaryRange(decimal Minimum, decimal Maximum);
```

That second declaration gives us much more than shorter syntax.

A record is designed primarily for data where **value-based equality** is useful.

Try predicting this:

```
var a = new SalaryRange(50_000m, 60_000m);
var b = new SalaryRange(50_000m, 60_000m);

Console.WriteLine(a == b);
```

With a record, we'd expect:

```
True
```

because record equality considers the values of its components.

That immediately affects collections:

```
HashSet<SalaryRange> ranges =
[
    new(50_000m, 60_000m),
    new(50_000m, 60_000m)
];
```

How many elements should that contain?

Think about Topic 4 before answering.

---

# 5. Important: record does NOT mean value type

This distinction matters.

Consider:

```
public record SalaryRange(decimal Minimum, decimal Maximum);
```

Unless we explicitly say `record struct`, this is a **reference type**.

So don't build the mental model:

```
class = reference type
record = value type
```

That's wrong.

Instead:

```
class         → reference type
record class  → reference type

struct        → value type
record struct → value type
```

The interesting distinction between a normal class and a record class is primarily the generated semantics around data/equality, not whether the variable contains a reference.

---

# 6. Records and immutability

Records work particularly nicely with immutable data.

With the positional declaration:

```
public record SalaryRange(decimal Minimum, decimal Maximum);
```

the generated properties are effectively initialization-oriented.

So rather than:

```
range.Minimum = 70_000m;
```

you can create another value.

C# provides `with` expressions:

```
var original = new SalaryRange(50_000m, 60_000m);

var improved = original with
{
    Maximum = 65_000m
};
```

Conceptually:

```
original → SalaryRange(50000, 60000)
improved → SalaryRange(50000, 65000)
```

We're describing a **new value** rather than mutating the old one.

### Micro-exercise

Without running it, predict:

```
var first = new SalaryRange(40_000m, 50_000m);

var second = first with
{
    Maximum = 55_000m
};

Console.WriteLine(first.Maximum);
Console.WriteLine(second.Maximum);
Console.WriteLine(first == second);
```

---

# 7. Records aren't automatically good domain models

Here's where I don't want you to fall into the common trap:

> “Modern C# uses records, therefore replace all classes with records.”
> 

No.

Consider:

```
public record JobApplication(
    Company Company,
    string Position,
    ApplicationStatus Status);
```

Value equality now suggests that two applications with identical values are the same.

But are they?

Imagine you applied to the same position twice:

```
Application A — Microsoft — Developer — 1 March
Application B — Microsoft — Developer — 15 September
```

These may be separate entities with their own histories.

An entity often wants explicit identity:

```
public Guid Id { get; }
```

Its attributes can change without changing what entity it is.

So a normal class may be the more natural representation.

---

# 8. First design decision

Consider our current concepts:

```
JobApplication
Company
SalaryRange
```

For each one, classify it as:

```
Entity
Value
Not sure
```

Then explain **why**.

Don't decide `class`/`record` yet.

Decide the *domain semantics first*.

That's an important professional habit:

```
What does this thing mean?
        ↓
What semantics do I need?
        ↓
Which C# construct expresses them?
```

not:

```
Records are modern
        ↓
Let's use records
```

---

# 9. Validation problem

Here's an immediate problem with our beautiful one-line record:

```
public record SalaryRange(decimal Minimum, decimal Maximum);
```

What stops this?

```
new SalaryRange(90_000m, 20_000m);
```

Nothing.

Our previous class protected this invariant.

So brevity has cost us something.

You can absolutely put validation and behaviour into records. Records aren't restricted to one-line declarations.

For example, we might eventually build something more sophisticated.

But I'm deliberately **not giving you the implementation yet**.

### Exercise

Design a `SalaryRange` record that:

- stores `Minimum`
- stores `Maximum`
- prevents negative salaries
- prevents `Minimum > Maximum`
- remains immutable after creation
- has value equality

Write your version.

This is your first substantial coding exercise.

---

# 10. `record struct`

C# also allows:

```
public readonly record struct Coordinate(int X, int Y);
```

Now we have both:

- generated record-style value equality
- value-type semantics

Small values can sometimes be good candidates.

But don't conclude:

> Value object → struct.
> 

Domain “value object” and CLR “value type” are different concepts.

A domain value object can perfectly well be a record **class**.

For JobTracker, I'd want you to understand `record struct`, but we don't currently have a compelling reason to optimise `SalaryRange` around value-type allocation/copy semantics.

We'll therefore keep this mostly conceptual.

---

# 11. Pattern matching

Now the second half.

You've already used conditions like:

```
if (application.CurrentStatus == ApplicationStatus.Applied)
{
    ...
}
```

That's perfectly fine.

But domain decisions often involve multiple possible states.

Traditional code might become:

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

Modern C# gives us **switch expressions**.

```
return status switch
{
    ApplicationStatus.Interested => "Interested",
    ApplicationStatus.Applied => "Applied",
    ApplicationStatus.Rejected => "Rejected",
    _ => "Unknown"
};
```

Read it approximately as:

> Take `status`, match it against these patterns, and produce the corresponding value.
> 

Notice that this is an **expression**.

It produces a value.

That's different from thinking of `switch` merely as a control-flow statement.

---

# 12. Switch-expression exercise

Write:

```
static string GetStatusDescription(ApplicationStatus status)
```

using a switch expression.

Requirements:

```
Interested → "Not applied yet"
Applied    → "Application submitted"
Rejected   → "Application unsuccessful"
```

Do you think `_` is necessary if `ApplicationStatus` currently has exactly those three values?

Don't just answer yes/no. Think about what can happen with enums.

---

# 13. Relational patterns

Pattern matching isn't restricted to equality.

For example:

```
static string ClassifySalary(decimal salary) =>
    salary switch
    {
        < 30_000m => "Low",
        < 50_000m => "Medium",
        < 80_000m => "High",
        _ => "Very high"
    };
```

Notice something subtle.

We don't need:

```
>= 30_000m and < 50_000m
```

for the second arm.

Why?

Because switch arms are evaluated in order. If we reached:

```
< 50_000m
```

then `< 30_000m` has already failed.

### Exercise

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

# 14. Logical patterns

Patterns can be combined:

```
salary switch
{
    >= 30_000m and < 50_000m => "Medium",
    >= 50_000m and < 80_000m => "High",
    _ => "Other"
};
```

Relevant operators include:

```
and
or
not
```

These are **pattern combinators**, not `&&`, `||`, `!`.

---

# 15. Property patterns

Now pattern matching becomes particularly useful for domain objects.

Instead of:

```
if (application.Remote &&
    application.CurrentStatus == ApplicationStatus.Applied)
{
    ...
}
```

we can express the shape/state we're looking for:

```
if (application is
    {
        Remote: true,
        CurrentStatus: ApplicationStatus.Applied
    })
{
    ...
}
```

This is a **property pattern**.

Read it as:

> Is `application` an object whose `Remote` property is `true` and whose `CurrentStatus` is `Applied`?
> 

This becomes more interesting when nested.

Suppose:

```
application.Company.Name
```

We can pattern-match object structure rather than manually building a long boolean expression.

---

# 16. Pattern matching is not automatically better

This:

```
if (application.Remote)
```

is better than:

```
if (application is { Remote: true })
```

when that's literally all you're checking.

Pattern matching earns its keep when the decision becomes structural or combines several conditions.

Don't modernise code merely to make it look newer.

---

# 17. Domain classification exercise

Implement:

```
static string Classify(JobApplication application)
```

with these rules:

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

Try to solve this primarily with a switch expression and property patterns.

The order of the arms matters.

I want you to think about **why**.

---

# 18. Type patterns

Pattern matching can also inspect runtime types.

Suppose we had:

```
abstract class Contact
{
    public string Name { get; init; }
}

class Recruiter : Contact
{
    public string Agency { get; init; }
}

class HiringManager : Contact
{
    public string Company { get; init; }
}
```

We could write:

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

This combines:

- runtime type checking
- safe extraction of the typed variable
- decision making

It's considerably cleaner than old-style casts.

---

# 19. Type + property patterns

They can be combined.

Conceptually:

```
contact switch
{
    Recruiter { Agency: "Hays" } recruiter => ...,
    Recruiter recruiter => ...,
    HiringManager manager => ...,
    _ => ...
};
```

Specific case first.

General case afterwards.

That ordering principle will appear repeatedly.

---

# 20. List patterns

Modern C# can pattern-match sequences too.

For example:

```
int[] numbers = [1, 2, 3];

if (numbers is [1, 2, 3])
{
    ...
}
```

Or:

```
if (numbers is [1, ..])
{
    ...
}
```

meaning roughly:

> starts with `1`, followed by zero or more elements.
> 

Another:

```
if (numbers is [_, _, _])
```

matches exactly three elements.

This is useful, but much less central to everyday backend C# than property/type patterns.

I want you to recognise and understand it rather than spend half the lesson constructing clever list patterns.

---

# 21. Null patterns

Pattern matching also gives us a clean null check:

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

You've probably seen these in modern C# code.

There are some semantic reasons `is null` has historically been attractive compared with overloaded equality operators, but the important thing at this stage is simply to recognise the idiom.

---

# 22. Main JobTracker exercise

Now combine both halves of the topic.

Don't start coding until you've made the modelling decisions.

Your task is to refactor the relevant part of JobTracker.

You need:

```
Company
JobApplication
SalaryRange
ApplicationStatus
```

Decide whether each should be:

```
class
record class
struct
record struct
enum
```

You must justify each decision based on:

```
identity
equality
mutability
lifecycle
invariants
```

Then implement the model.

Requirements:

- `SalaryRange` must prevent invalid ranges.
- Two equivalent salary ranges should compare equal.
- A `JobApplication` has identity independent of its current state.
- Changing an application's status must not create a different application identity.
- `Company` requires an explicit decision from you: entity or value?
- Domain state should not be arbitrarily writable from outside.
- Use a switch expression somewhere genuinely useful.
- Use at least one property pattern.
- Don't add inheritance just to satisfy the exercise.

Then demonstrate your equality decisions.

Something along these lines—not necessarily these exact assertions:

```
salary1 == salary2
company1 == company2
application1 == application2
```

Before running it, write down what you expect each result to be.

---

# 23. One additional trap: shallow immutability

Suppose:

```
public record Candidate(string Name, List<string> Skills);
```

Then:

```
var candidate = new Candidate(
    "Alice",
    ["C#", "SQL"]
);
```

The record itself gives us useful initialization/equality semantics.

But what about:

```
candidate.Skills.Add("Docker");
```

This is legal.

The record hasn't magically made the `List<string>` immutable.

And `with` performs a **shallow copy**, not a recursive deep clone of every referenced object.

This is important enough to remember:

> **A record is not synonymous with deep immutability.**
> 

We'll encounter similar issues when dealing with DTOs and collections later.

---

# 24. Knowledge check

Answer these without looking them up initially:

1. Is a `record` normally a reference type or value type?
2. What's the difference between `record` and `record struct`?
3. Why might `SalaryRange` benefit from record equality?
4. Why might `JobApplication` not?
5. What is the difference between entity identity and value equality?
6. Does `init` make referenced objects immutable?
7. Does `with` mutate the original record?
8. Is a `with` copy necessarily a deep copy?
9. What does `_` mean in a switch expression?
10. Why can switch-arm ordering matter?
11. What's a property pattern?
12. What's a relational pattern?
13. What's a type pattern?
14. What do `and`, `or`, and `not` mean inside patterns?
15. When is `if (x.Remote)` preferable to a property pattern?
16. Can records contain methods and validation?
17. Can an ordinary class implement value equality?
18. Does “domain value object” imply C# `struct`?
19. Why does equality matter to `HashSet<T>`?
20. Why does equality matter to dictionary keys?

---

# 25. Exit test

Once you've completed the main exercise, you should be able to explain this without executing it:

```
public record SalaryRange(decimal Minimum, decimal Maximum);

var first = new SalaryRange(50_000m, 70_000m);
var second = new SalaryRange(50_000m, 70_000m);
var third = first with { Maximum = 80_000m };

Console.WriteLine(first == second);
Console.WriteLine(first == third);
Console.WriteLine(first.Maximum);
Console.WriteLine(third.Maximum);
```

And you should be able to read something like:

```
var description = application switch
{
    { CurrentStatus: ApplicationStatus.Rejected }
        => "Closed",

    {
        CurrentStatus: ApplicationStatus.Applied,
        Remote: true
    }
        => "Active remote",

    {
        SalaryRange.Maximum: >= 70_000m
    }
        => "High value",

    _ => "Other"
};
```

and reason about **which arm wins and why**.

---

## Where I'd start

Don't try to consume all of that and then do the exercises at the end.

Start with **sections 1–8 only**. In particular, send me your answers to the four warm-up questions and your classification of:

```
JobApplication → ?
Company        → ?
SalaryRange    → ?
```

with your reasoning.

Then we'll deal with your `SalaryRange` implementation. I won't solve it for you first.