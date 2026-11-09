# 4. Collections, Equality & Protecting Object State

**Project:** JobTracker — managing multiple applications

**Main objective:** Move from modelling one `JobApplication` to managing collections of domain objects correctly.

Topic 3 established relationships *between objects*. Topic 4 is about what happens when we have many of them: which collection represents the problem, how equality affects those collections, and how to avoid accidentally exposing mutable internal state.

Microsoft's current collection documentation groups collections partly by how elements are accessed: by position, by key, or through enumeration. That distinction is more useful than simply memorising collection classes. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/collections?utm_source=chatgpt.com)

---

## 1. Learning objectives

By the end you should be comfortable with:

- arrays and `List<T>`;
- `Dictionary<TKey,TValue>`;
- `HashSet<T>`;
- choosing a collection based on its semantics;
- `IEnumerable<T>`, `ICollection<T>`, `IReadOnlyCollection<T>` and `IReadOnlyList<T>`;
- exposing collections without exposing unnecessary mutation;
- reference equality versus value equality;
- why equality matters to sets and dictionaries;
- modern collection expressions;
- modelling JobTracker as an object containing applications.

We'll deliberately leave serious querying to **Topic 5: LINQ**.

---

# 2. Retrieval warm-up

No looking back unless necessary.

### A — Topic 2

What is the difference between:

```
public string Name { get; }
```

and:

```
public string Name { get; private set; }
```

### B — Topic 3

For each relationship, choose **inheritance**, **composition**, or **interface/capability**, and explain briefly:

```
Car → Engine
Manager → Employee
JobApplication → Company
JobApplication → exportable
```

### C — Write from memory

Declare:

```
a List<string>
add two values
loop through them with foreach
```

That's all.

---

# 3. From one object to many objects

Until now we've mostly dealt with:

```
JobApplication application;
```

JobTracker obviously needs:

```
Application A
Application B
Application C
...
```

The simplest option is:

```
JobApplication[] applications;
```

An array represents a fixed-size sequence of elements of the same type.

```
string[] companies =
{
    "Microsoft",
    "Indra",
    "TUI"
};
```

Arrays remain extremely useful, but application-level collections frequently need to grow and shrink.

That's where generic collections enter.

---

# 4. `List<T>`

You will know this one already:

```
var applications = new List<JobApplication>();

applications.Add(application1);
applications.Add(application2);
```

or:

```
List<JobApplication> applications =
[
    application1,
    application2
];
```

The second syntax uses a **collection expression**, modern C# syntax using `[...]` to construct collection values. Collection expressions can target numerous collection types rather than being specific to `List<T>`. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions?utm_source=chatgpt.com)

A `List<T>` gives us:

```
applications.Add(application);
applications.Remove(application);

var first = applications[0];

int count = applications.Count;
```

and:

```
foreach (var application in applications)
{
    Console.WriteLine(application.Position);
}
```

A list is a good default when:

- order matters;
- duplicates are acceptable;
- positional access is useful;
- you mostly need a sequence of things.

Microsoft describes `List<T>` as the common index-based generic collection. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/collections?utm_source=chatgpt.com)

---

# 5. Don't choose a collection just because you remember its syntax

Suppose JobTracker needs these three things:

```
A. All applications, in the order they were added.

B. One application identified by a unique application ID.

C. A collection of unique skills:
   C#
   React
   Docker
   SQL
```

These have different semantics.

A likely mapping is:

```
A → List<T>
B → Dictionary<TKey, TValue>
C → HashSet<T>
```

The important skill isn't remembering `new List<T>()`.

It's recognising the data structure implied by the requirement.

---

# 6. `Dictionary<TKey, TValue>`

A dictionary maps a **unique key** to a value:

```
Dictionary<int, string> companies = new();

companies.Add(1, "Microsoft");
companies.Add(2, "Indra");
```

Then:

```
string company = companies[2];
```

gives:

```
Indra
```

Conceptually:

```
KEY          VALUE

1     →      Microsoft
2     →      Indra
3     →      TUI
```

For JobTracker:

```
Application ID → JobApplication
```

might naturally be represented as:

```
Dictionary<Guid, JobApplication>
```

Microsoft describes `Dictionary<TKey,TValue>` as a key/value collection implemented using hashing; lookup by key is normally very fast, close to O(1), assuming a suitable hash distribution. Keys must be unique according to the dictionary's equality comparer. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=netcore-2.2&utm_source=chatgpt.com)

---

# 7. Why not just use `List<T>` for everything?

Suppose we have 100,000 applications and need application:

```
ID = 84723
```

With a list, conceptually we may have to search:

```
Is this it?
No.

This?
No.

This?
No.

...
```

That's a linear search: **O(n)**.

With a hash-based dictionary, lookup by key is approximately **O(1)** on average.

Don't obsess over Big-O notation yet. The useful intuition is:

```
List
→ find by searching through elements

Dictionary
→ find directly by key
```

This doesn't mean:

> Dictionary is faster, therefore Dictionary is better.
> 

A dictionary is appropriate when the domain actually gives you a meaningful unique key.

---

# 8. Safer dictionary lookup

This is legal:

```
var application = applications[id];
```

But what happens if `id` isn't present?

The indexer throws.

Often we expect absence to be normal.

`Dictionary<TKey,TValue>` therefore gives us:

```
if (applications.TryGetValue(id, out var application))
{
    Console.WriteLine(application.Position);
}
```

Read that as:

> Try to retrieve this value. Tell me whether you succeeded and, if so, give me the result.
> 

This `Try...` pattern appears frequently in .NET.

You should recognise things like:

```
int.TryParse(...)
dictionary.TryGetValue(...)
```

The operation can fail in an expected way without using exceptions for ordinary control flow.

---

# 9. `HashSet<T>`

A set represents **unique values**.

```
HashSet<string> skills =
[
    "C#",
    "React",
    "Docker"
];
```

If you add:

```
skills.Add("C#");
```

you still have only one `"C#"`.

A `HashSet<T>` doesn't model positional ordering like a list. It models **membership and uniqueness**.

Microsoft describes `HashSet<T>` as an unordered collection with no duplicate elements and efficient set operations. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?view=net-5.0&utm_source=chatgpt.com)

It also supports actual set operations:

```
UnionWith(...)
IntersectWith(...)
ExceptWith(...)
IsSubsetOf(...)
Overlaps(...)
```

For example:

```
My skills:
C#, React, SQL

Required skills:
C#, Docker, Azure
```

Set operations can naturally answer:

```
Which skills overlap?
Which requirements am I missing?
```

We don't need to implement that yet.

---

# 10. Micro-exercise — choose the collection

Don't code anything.

Choose between:

```
array
List<T>
Dictionary<TKey,TValue>
HashSet<T>
```

and explain why.

### A

The seven days of the week.

### B

Job applications in the order the user created them.

### C

Applications accessed frequently by unique `Guid`.

### D

Unique technologies required by a job.

### E

The last five interview scores, where there will always be exactly five slots.

### F

A mapping:

```
ApplicationStatus → number of applications
```

There can be more than one defensible answer in some cases.

---

# 11. The important part: equality

Consider:

```
var company1 = new Company("Microsoft", "microsoft.com");
var company2 = new Company("Microsoft", "microsoft.com");
```

Question:

```
company1 == company2
```

What should the answer be?

There are **two different questions** hiding here.

### Identity

Are these references pointing to the exact same object?

### Value

Do these objects represent the same conceptual value?

Those are not equivalent.

For ordinary classes, unless you implement different equality semantics, two separately constructed instances are distinct objects even if their properties happen to contain the same values.

That becomes important very quickly.

---

# 12. Reference equality

Consider:

```
var company1 = new Company("Microsoft", "microsoft.com");

var company2 = company1;
```

Now:

```
company1 ───────┐
                ▼
             Company
                ▲
company2 ───────┘
```

Both variables refer to the same object.

Compare that with:

```
var company1 = new Company("Microsoft", "microsoft.com");
var company2 = new Company("Microsoft", "microsoft.com");
```

Now:

```
company1 ─────→ Company #1

company2 ─────→ Company #2
```

Same data doesn't necessarily mean same identity.

---

# 13. Why equality matters to collections

Imagine:

```
var companies = new HashSet<Company>();

companies.Add(
    new Company("Microsoft", "microsoft.com")
);

companies.Add(
    new Company("Microsoft", "microsoft.com")
);
```

You might expect:

```
Count = 1
```

because:

> It's the same company.
> 

But how does `HashSet<Company>` know what **same company** means?

That's not a philosophical question. The collection needs an actual equality rule.

Hash-based collections rely on equality and hash codes to determine whether keys/elements are equal. A `Dictionary<TKey,TValue>` similarly requires its keys to have stable equality/hash behaviour. Microsoft specifically warns that dictionary keys must not change in a way that affects their hash value while they're being used as keys. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=netcore-2.2&utm_source=chatgpt.com)

This is one reason equality design matters.

---

# 14. `Equals()` and `GetHashCode()`

.NET objects have:

```
Equals(...)
GetHashCode()
```

These participate in equality semantics.

If you define custom equality, an important rule is:

> Objects that are equal must produce the same hash code.
> 

Otherwise hash-based structures such as:

```
Dictionary
HashSet
```

can behave incorrectly.

You *can* implement equality manually in a class, but modern C# gives us another interesting option that we'll study properly in Topic 6:

```
record
```

Records provide value-based equality by default.

For now, I don't want you converting everything into records before we've studied what that means.

---

# 15. Design problem: what makes two companies equal?

This is not primarily a C# question.

Consider:

```
Company A
Name: Microsoft
Website: microsoft.com

Company B
Name: Microsoft
Website: microsoft.com
```

Same company?

Probably.

Now:

```
Microsoft
microsoft.com
```

versus:

```
Microsoft Corporation
microsoft.com
```

Same?

Maybe.

What about:

```
Indra
indra.es
```

versus:

```
Indra
indracompany.com
```

The language cannot decide this for us.

**Equality is part of domain modelling.**

Sometimes identity should instead come from an explicit identifier:

```
Guid Id
```

Then:

```
same ID → same entity
```

regardless of whether some descriptive properties changed.

We'll revisit this when discussing records, entities and value objects.

---

# 16. Collection interfaces

Now we get to something more professionally important.

Suppose JobTracker owns:

```
private readonly List<JobApplication> _applications = [];
```

You could expose:

```
public List<JobApplication> Applications
{
    get { return _applications; }
}
```

External code can now do:

```
tracker.Applications.Clear();
```

or:

```
tracker.Applications.Add(someRandomApplication);
```

That means JobTracker doesn't actually control its own collection.

We spent Topic 2 protecting this:

```
public ApplicationStatus Status { get; private set; }
```

It would be strange to protect individual properties while handing callers unrestricted access to the entire internal collection.

---

# 17. Program to the capability you want to expose

Instead of:

```
public List<JobApplication> Applications
```

we might expose:

```
public IReadOnlyCollection<JobApplication> Applications
```

while internally maintaining:

```
private readonly List<JobApplication> _applications;
```

Now there's an important distinction:

```
Internal representation:
List<JobApplication>

Public contract:
IReadOnlyCollection<JobApplication>
```

The class needs mutation capabilities internally.

The caller doesn't.

`IReadOnlyCollection<T>` exposes enumeration and `Count`, but not mutating operations such as `Add`, `Remove`, or `Clear`. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1?view=netframework-4.7&utm_source=chatgpt.com)

This connects directly to encapsulation from Topic 2.

---

# 18. `IEnumerable<T>` vs `IReadOnlyCollection<T>` vs `IReadOnlyList<T>`

At a high level:

```
IEnumerable<T>
    "You can iterate through these."

IReadOnlyCollection<T>
    "You can iterate and know Count."

IReadOnlyList<T>
    "You can iterate, know Count, and access by index."

ICollection<T>
    "You have collection operations, including mutation."

IList<T>
    "You have mutable list semantics and indexed access."
```

Microsoft's generic collection APIs explicitly distinguish these contracts: `IReadOnlyCollection<T>` provides a read-only collection abstraction, while `IReadOnlyList<T>` adds indexed access. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic?view=netcore-2.2&utm_source=chatgpt.com)

This leads to a useful API-design principle:

> **Expose the least powerful abstraction your caller needs.**
> 

If callers only need iteration, why promise indexed access?

If they only need reading, why promise mutation?

---

# 19. Important subtlety: “read-only” doesn't mean immutable

Suppose:

```
public IReadOnlyCollection<JobApplication> Applications { get; }
```

The caller can't do:

```
Applications.Add(...)
```

But if `JobApplication` itself is mutable:

```
application.Notes = "Changed";
```

the object inside the collection can still change.

So:

```
read-only collection
```

means:

> The caller cannot mutate the **collection through that interface**.
> 

It does **not** necessarily mean:

> Everything inside it is immutable.
> 

Keep collection mutability and object mutability separate in your head.

---

# 20. Another subtlety: abstraction isn't necessarily a defensive copy

Consider:

```
private readonly List<JobApplication> _applications = [];

public IReadOnlyCollection<JobApplication> Applications
    => _applications;
```

The caller can't call `Add` through the `IReadOnlyCollection<T>` reference.

But we're still exposing a view of the same underlying collection.

That's often completely reasonable.

Other designs might return a copy or immutable collection when stronger isolation is required.

Don't cargo-cult defensive copies everywhere—they have costs too.

The requirement determines the protection needed.

---

# 21. `readonly` does not make a collection immutable

This is a common confusion:

```
private readonly List<JobApplication> _applications = [];
```

`readonly` means the field can't later be reassigned to another list after construction.

This is prevented:

```
_applications = new List<JobApplication>();
```

But this is still fine:

```
_applications.Add(application);
```

The **reference** is readonly.

The **object referenced by it** is still mutable.

This is analogous to:

```
const
readonly
immutable object
read-only interface
```

being related ideas but not interchangeable concepts.

---

# 22. Modern collection expressions

Historically you'd commonly write:

```
var numbers = new List<int>
{
    1,
    2,
    3
};
```

Modern C# supports:

```
List<int> numbers = [1, 2, 3];
```

Arrays:

```
int[] numbers = [1, 2, 3];
```

and other supported targets can use the same collection-expression syntax. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions?utm_source=chatgpt.com)

There's also a **spread element**:

```
int[] first = [1, 2, 3];
int[] second = [4, 5];

int[] combined = [.. first, .. second];
```

producing conceptually:

```
1 2 3 4 5
```

You should recognise and be comfortable writing this modern syntax.

But:

```
var numbers = [1, 2, 3];
```

is not generally sufficient by itself because the collection expression needs a target type.

That's another example of where `var` isn't always usable.

---

# 23. Micro-exercise — predict the behaviour

Without running it:

```
List<string> technologies = ["C#", "React"];

var anotherReference = technologies;

anotherReference.Add("Docker");

Console.WriteLine(technologies.Count);
```

Answer:

1. What prints?
2. Why?
3. How does this connect to the reference-type discussion from Topic 2?

---

# 24. Main exercise — introduce `JobTracker`

Until now, `JobApplication` has been doing domain work for one application.

We now need an object responsible for managing applications.

Create:

```
JobTracker
```

### Requirements

A tracker manages multiple `JobApplication` objects.

External code must **not** receive unrestricted mutation access to its internal collection.

It must support:

```
AddApplication
RemoveApplication
```

An application should have a unique:

```
Guid Id
```

assigned when it is created.

JobTracker must also support:

```
FindById
```

You decide what should happen when an ID doesn't exist.

---

### Additional requirement — skills

Each `JobApplication` can contain required skills:

```
C#
ASP.NET Core
React
Docker
SQL
...
```

A skill should not appear twice for the same application.

Choose an appropriate collection.

Think about whether external code should be able to freely modify that collection.

---

### Additional requirement — companies

JobTracker also needs to keep track of known companies.

A company should have an ID.

We frequently need to retrieve a company by ID.

Choose an appropriate collection.

Don't introduce a repository abstraction yet. We'll get there later.

---

# 25. Constraints

For this exercise:

**Do not use LINQ.**

I know you could immediately write:

```
_applications.FirstOrDefault(...)
```

That's Topic 5.

For now, use the collections themselves and ordinary control flow so we're clear about what the underlying structures provide.

Also:

**Do not automatically create an interface for `JobTracker`.**

Topic 3 taught interfaces. That doesn't mean every class now needs one.

Add abstractions because there's a reason.

---

# 26. Design decisions I want you to make

Before implementing, write short answers in comments:

```
1. What collection owns the applications, and why?

2. What collection stores companies, and why?

3. What collection stores skills, and why?

4. What type should JobTracker expose for its applications?

5. Should JobApplication expose its skills directly?

6. What happens when FindById receives an unknown ID?

7. What happens if the same application is added twice?

8. What defines "same application"?
```

There aren't predetermined answers to all of these.

For example, this requirement:

```
Find application by ID
```

might make you consider:

```
Dictionary<Guid, JobApplication>
```

But perhaps you decide a `List<JobApplication>` is sufficient at this scale and implement a search.

I care more about whether your decision follows from the requirements.

---

# 27. A trap I specifically want you to notice

Suppose you choose:

```
private readonly List<JobApplication> _applications = [];
```

and expose:

```
public IReadOnlyCollection<JobApplication> Applications
    => _applications;
```

Good.

But then implement:

```
public void AddApplication(JobApplication application)
{
    _applications.Add(application);
}
```

What happens if I do:

```
tracker.AddApplication(application);
tracker.AddApplication(application);
tracker.AddApplication(application);
```

Maybe that's allowed.

Maybe it isn't.

The collection type and your domain rules are separate questions.

A `List<T>` permits duplicates.

That doesn't mean your **domain** must permit duplicates.

---

# 28. Equality challenge

After your first JobTracker implementation works, experiment with this separately:

```
var first = new Company(
    Guid.NewGuid(),
    "Microsoft",
    "microsoft.com");

var second = new Company(
    Guid.NewGuid(),
    "Microsoft",
    "microsoft.com");
```

Consider these possible definitions:

```
A. Same object reference
B. Same Id
C. Same Name
D. Same Website
E. Same Name + Website
```

Which one should mean:

> **These represent the same company**
> 

in JobTracker?

Don't implement custom equality yet unless you want to experiment.

I want the reasoning first.

---

# 29. `HashSet<T>` experiment

Now perform one small experiment.

Create:

```
HashSet<Company>
```

Add two separately constructed `Company` objects containing identical descriptive information.

Inspect:

```
Count
```

Then add the **same object reference** twice.

Inspect `Count` again.

Explain the difference.

This experiment will set us up very nicely for records and value equality later.

---

# 30. Dictionary challenge

Create a:

```
Dictionary<Guid, Company>
```

Add several companies.

Then implement, without LINQ:

```
PrintCompany(Guid id)
```

Requirements:

- if the company exists, print its name;
- if it doesn't, print `"Company not found"`;
- don't use exceptions as normal lookup logic.

Use the dictionary API intended for this situation.

---

# 31. API-design challenge

Consider these three versions:

```
public List<JobApplication> Applications { get; }
```

```
public IReadOnlyCollection<JobApplication> Applications { get; }
```

```
public IEnumerable<JobApplication> Applications { get; }
```

For JobTracker, answer:

> Which would you expose **right now**, and why?
> 

There isn't necessarily one universally correct answer.

Your reasoning should involve what operations the caller actually needs.

---

# 32. Knowledge check

Answer these after completing the coding work.

**1.** When would you choose a `List<T>` over a `Dictionary<TKey,TValue>`?

**2.** What does a dictionary key represent?

**3.** Why must dictionary keys be unique?

**4.** What's the conceptual purpose of `HashSet<T>`?

**5.** Why does equality matter to `HashSet<T>`?

**6.** What's the difference between:

```
IEnumerable<T>
```

and:

```
IReadOnlyCollection<T>
```

?

**7.** Why might this be dangerous?

```
public List<JobApplication> Applications { get; }
```

**8.** Does:

```
readonly List<JobApplication>
```

make the list immutable?

Explain.

**9.** Does:

```
IReadOnlyCollection<JobApplication>
```

make each `JobApplication` immutable?

**10.** What's the difference between two references to the same object and two separate objects containing identical data?

**11.** What broad performance difference exists between searching a list and dictionary lookup by key?

**12.** Why can a mutable dictionary key be dangerous if the mutation changes its equality/hash behaviour?

**13.** What does `TryGetValue()` communicate differently from using the dictionary indexer?

**14.** When would `HashSet<T>` communicate intent better than `List<T>` even if both technically work?

---

# 33. Code-reading test

Predict everything before running it:

```
var company = new Company(
    Guid.NewGuid(),
    "Microsoft",
    "microsoft.com");

List<Company> companies = [company];

var copy = companies;

copy.Add(
    new Company(
        Guid.NewGuid(),
        "TUI",
        "tui.com"));

Console.WriteLine(companies.Count);

company.ChangeWebsite("careers.microsoft.com");

Console.WriteLine(companies[0].Website);
```

Questions:

1. What does the first `Console.WriteLine` print?
2. Why?
3. What does the second print?
4. Did putting `company` into a list copy the `Company` object?
5. How many `List<Company>` objects exist?
6. How many `Company` objects exist at the end?

Assume `ChangeWebsite()` performs the obvious mutation.

---

# 34. Exit drill

From memory, write only:

```
1. A List<int> containing three numbers.
2. A HashSet<string> containing three strings.
3. A Dictionary<int, string> containing two entries.
4. Add one element to each.
5. Retrieve a dictionary value safely.
6. Loop through the list.
7. Declare an IReadOnlyCollection<string>.
8. Initialize a collection using [...] syntax.
```

If that takes a few minutes, good. Don't turn it into another application.

---

# 35. What I expect your JobTracker to roughly become

Not code to copy—just the conceptual structure:

```
JobTracker
 │
 ├── applications
 │      │
 │      ├── JobApplication
 │      ├── JobApplication
 │      └── JobApplication
 │
 └── companies
        │
        ├── Company
        └── Company

JobApplication
 │
 ├── Company
 ├── SalaryRange
 └── required skills
        │
        ├── "C#"
        ├── "React"
        └── "Docker"
```

Notice how much Topic 3 is already influencing Topic 4.

We're increasingly building the application from **objects that own responsibilities and compose other objects**, rather than one giant class.

---

# 36. One thing we're deliberately postponing

You're probably going to find yourself wanting to write things like:

```
applications
    .Where(...)
    .OrderBy(...)
    .Select(...);
```

Don't yet.

Topic 5 is **LINQ**, and I want that lesson to go considerably deeper than remembering `Where()` and `Select()`:

```
delegates and lambdas
        ↓
IEnumerable<T>
        ↓
LINQ operators
        ↓
deferred execution
        ↓
materialisation
        ↓
multiple enumeration
        ↓
projection/grouping/aggregation
        ↓
IEnumerable<T> vs IQueryable<T>
```

The collection work in this topic gives us the data model necessary to make that lesson useful rather than artificial.

## Official references

[Microsoft Learn — Collections (C# reference)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/collections?utm_source=chatgpt.com) — overview of index-based, key/value and enumerable collections. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/collections?utm_source=chatgpt.com)

[Microsoft Learn — Generic collections namespace](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic?utm_source=chatgpt.com) — the generic collection types and interfaces. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic?view=netcore-2.2&utm_source=chatgpt.com)

[Microsoft Learn — Dictionary<TKey,TValue>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?utm_source=chatgpt.com) — dictionary behaviour, keys, hashing and equality. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=netcore-2.2&utm_source=chatgpt.com)

[Microsoft Learn — HashSet<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?utm_source=chatgpt.com) — uniqueness and set operations. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?view=net-5.0&utm_source=chatgpt.com)

[Microsoft Learn — Collection expressions](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions?utm_source=chatgpt.com) — modern `[...]` collection syntax and spread elements. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions?utm_source=chatgpt.com)

This should be about the same difficulty progression as Topic 3: the basic collection syntax should be easy for you; the actual work is **choosing structures, reasoning about equality, and designing what your objects expose**.