# 2. Classes, Objects & Encapsulation

**Project:** JobTracker — moving from loose variables to a domain model

**Main objective:** Recover everyday object-oriented C# fluency while learning how modern C# lets us control object initialization and state more precisely.

This topic deliberately has **one main JobTracker exercise** rather than several versions of the same program.

Microsoft's current guidance describes classes as reference types particularly suited to objects with behaviour, mutable state, and identity. Properties provide controlled access to data, while constructors establish an object's initial state. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/classes?utm_source=chatgpt.com)

---

## 1. Learning objectives

By the end you should be able to:

- distinguish a **class**, an **object** and a **reference**;
- declare and instantiate classes without needing an example;
- use fields, properties, constructors and methods appropriately;
- use `public`, `private`, `protected` and `internal`;
- explain **encapsulation** and apply it rather than merely define it;
- distinguish instance members from `static` members;
- reason about which parts of an object's state callers should be allowed to modify;
- understand nullable reference types;
- use `init` and `required`;
- design a small class whose state cannot easily become invalid.

---

# 2. Retrieval warm-up

No need to write another application. Do these without Topic 1 open.

### A — Write from memory

Write a method that receives an annual salary and a percentage increase and returns the new salary.

### B — Read the code

```
decimal salary = 50000m;
var newSalary = IncreaseSalary(salary, 10);
```

What types do you expect `salary` and `newSalary` to have?

### C — Think

We currently represent a job like this:

```
string company = "Microsoft";
string position = ".NET Developer";
decimal salary = 55000m;
bool remote = true;
```

Imagine tracking 50 applications.

What problem are we beginning to encounter?

Don't worry about terminology yet. Think about how you'd want the code organised.

---

# 3. From variables to objects

In Topic 1, the job application was represented by unrelated variables.

Conceptually, however:

```
company
position
salary
remote
```

aren't four unrelated pieces of information.

They describe **one thing**.

Object-oriented programming lets us create a type that represents that concept:

```
public class JobApplication
{
}
```

A **class** defines a type. An **object** is an instance of that type.

```
var application = new JobApplication();
```

Here:

- `JobApplication` → class/type
- `new JobApplication()` → creates an object
- `application` → variable containing a reference to that object

Classes are **reference types**. The variable doesn't contain the complete object itself; it contains a reference to it. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/classes?utm_source=chatgpt.com)

That distinction has consequences.

### Predict before running

```
var application1 = new JobApplication();
var application2 = application1;
```

How many `JobApplication` objects exist?

**One.**

Both variables refer to the same object.

We'll return to value versus reference semantics later because it becomes important with records, structs, method arguments and equality.

---

# 4. Fields and properties

A class contains **members**. These include fields, properties, methods, constructors, events and several other constructs. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/index?utm_source=chatgpt.com)

A field is a variable declared directly inside a class:

```
public class JobApplication
{
    private decimal _salary;
}
```

The `_salary` field belongs to each `JobApplication` instance.

But exposing fields directly is generally discouraged. Microsoft's current guidance recommends keeping fields `private` or `protected` and exposing data through properties or methods. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/fields?utm_source=chatgpt.com)

## Properties

You've certainly seen this syntax:

```
public string Company { get; set; }
```

This is an **auto-implemented property**.

Conceptually:

- `get` controls reading;
- `set` controls writing.

Properties give us field-like syntax while retaining the ability to control access and behaviour. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties?utm_source=chatgpt.com)

For example:

```
application.Company = "Microsoft";

Console.WriteLine(application.Company);
```

### Quick drill

Without copying the syntax above, write properties for:

- `Position` → `string`
- `Salary` → `decimal`
- `Remote` → `bool`

If the syntax came back immediately, move on.

---

# 5. Encapsulation: the important part

Consider:

```
public class BankAccount
{
    public decimal Balance { get; set; }
}
```

External code can now do this:

```
account.Balance = -1000000;
```

or:

```
account.Balance = 999999999;
```

The object has no control over its own state.

That's the problem **encapsulation** helps solve.

Encapsulation isn't simply:

> Make fields private.
> 

A more useful definition is:

> An object controls access to its internal state and exposes deliberate operations through which other code interacts with it.
> 

Consider:

```
public class BankAccount
{
    public decimal Balance { get; private set; }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit must be positive.");
        }

        Balance += amount;
    }
}
```

Now:

```
account.Balance = 5000;
```

isn't allowed externally.

But:

```
account.Deposit(5000);
```

is.

The difference is important.

`Deposit()` represents a **meaningful operation**, and the object can enforce its rules when that operation happens.

That's object-oriented design, rather than merely putting variables inside classes.

---

# 6. Access modifiers

Accessibility determines which code can use a type or member. C# provides six accessibility forms; the four you should have immediately available in everyday work are: [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers?utm_source=chatgpt.com)

| Modifier | Accessible from |
| --- | --- |
| `public` | Anywhere that can access the containing type |
| `private` | Only inside the same class/struct |
| `protected` | Same class and derived classes |
| `internal` | Same assembly |

There are also:

```
protected internal
private protected
```

Know that they exist; memorising every combination isn't important yet.

A class member without an explicit access modifier defaults to `private`. A top-level class without one defaults to `internal`. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers?utm_source=chatgpt.com)

### Think rather than code

Consider:

```
public class JobApplication
{
    public string Company { get; set; }
    public decimal Salary { get; set; }
    public string Status { get; set; }
}
```

Ask yourself:

> Should *any* code in our application really be able to change *all three* of these values whenever it wants?
> 

Keep your answer. We'll use it later.

---

# 7. Constructors

An object often requires certain information before it can meaningfully exist.

A constructor initializes a new instance and has the same name as its type. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-constructors?utm_source=chatgpt.com)

```
public class JobApplication
{
    public string Company { get; }
    public string Position { get; }

    public JobApplication(string company, string position)
    {
        Company = company;
        Position = position;
    }
}
```

Now:

```
var application =
    new JobApplication("Microsoft", ".NET Developer");
```

The caller **cannot create a `JobApplication` without supplying those values**.

That's already an improvement to our model.

### Code-writing drill

Without looking at the previous example:

Create:

```
class: Company

required constructor information:
- Name
- Website
```

Both values should be readable externally but not changeable after construction.

Instantiate one `Company`.

That's enough. Don't build anything else around it.

---

# 8. Object state and invariants

This is where the topic becomes more interesting.

Suppose JobTracker allows these statuses:

```
Interested
Applied
Interview
Offer
Rejected
```

This design:

```
public string Status { get; set; }
```

allows:

```
application.Status = "Banana";
```

That's clearly undesirable.

An enum improves things:

```
public enum ApplicationStatus
{
    Interested,
    Applied,
    Interview,
    Offer,
    Rejected
}
```

Now:

```
public ApplicationStatus Status { get; set; }
```

prevents arbitrary strings.

But there's still a design question.

Should external code be allowed to do this?

```
application.Status = ApplicationStatus.Offer;
```

What if the application hasn't even been submitted?

An **invariant** is a condition that should remain true for an object to be considered valid.

Examples might be:

- salary can't be negative;
- company can't be missing;
- an application can't receive an offer before being submitted.

Encapsulation gives the class somewhere to enforce such rules.

This idea will recur constantly as we move toward domain modelling and architecture.

---

# 9. Nullable reference types

Now we reach an important modern C# feature.

Historically:

```
string notes = null;
```

was legal, even though code could later do:

```
Console.WriteLine(notes.Length);
```

and fail at runtime with a `NullReferenceException`.

Modern C# can perform compile-time **null-state analysis**.

In a nullable-enabled project:

```
string company;
```

means that `company` is intended **not to be null**.

Whereas:

```
string? notes;
```

explicitly means:

> `null` is a valid value here.
> 

The compiler tracks whether references are *not-null* or *maybe-null* and warns about potentially unsafe dereferences. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types?utm_source=chatgpt.com)

For JobTracker this distinction is meaningful:

```
string Company
string Position
string? Notes
```

A job application without a company probably doesn't make sense.

A job application without notes does.

Nullable annotations therefore aren't merely compiler bureaucracy. They let your type communicate part of its contract.

### Quick reasoning exercise

Which would you make nullable?

```
Company
Position
RecruiterName
RecruiterEmail
Notes
ApplicationDate
```

There isn't necessarily one universal answer. Decide based on the domain assumptions you want JobTracker to enforce.

---

# 10. `init`: mutable during initialization

Traditional writable property:

```
public string Company { get; set; }
```

can be changed later:

```
application.Company = "Microsoft";
application.Company = "Google";
application.Company = "OpenAI";
```

Sometimes we want object-initializer syntax without allowing arbitrary later mutation.

Modern C# provides `init`:

```
public string Company { get; init; }
```

This permits:

```
var application = new JobApplication
{
    Company = "Microsoft"
};
```

but not a later assignment:

```
application.Company = "Google"; // not allowed
```

An `init` accessor can be assigned during object initialization/construction but not through normal assignment afterwards. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-properties?utm_source=chatgpt.com)

---

# 11. `required`

There's still a problem:

```
public string Company { get; init; }
```

What stops us writing:

```
var application = new JobApplication();
```

when `Company` is something every application should have?

One modern option is:

```
public required string Company { get; init; }
```

Now callers are required to initialize that member.

For example:

```
var application = new JobApplication
{
    Company = "Microsoft"
};
```

Microsoft specifically documents `required` together with initialization mechanisms such as `init` as a way of enforcing initialization requirements. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties?utm_source=chatgpt.com)

This gives us several design tools:

```
public string Name { get; set; }

public string Name { get; private set; }

public string Name { get; }

public string Name { get; init; }

public required string Name { get; init; }
```

They don't mean the same thing.

---

# 12. Design exercise — choose deliberately

Don't code yet.

For each piece of `JobApplication` state, decide which access pattern seems most appropriate.

| Data | Your decision |
| --- | --- |
| Company | required get; init; |
| Position | required get; init; |
| Minimum salary | required get; init; |
| Maximum salary | required get; init; |
| Application status | get; set; |
| Date discovered | get; init; |
| Date applied | get; init; |
| Notes | nullable |

Possible choices include:

```
get; set;
get; private set;
get;
get; init;
required ... get; init;
nullable
non-nullable
```

Also decide whether some state should change **only through a method**.

This isn't a syntax question. It's a modelling question.

---

# 13. Instance versus `static`

Normally:

```
application.Status
application.Apply()
```

operate on one particular `JobApplication`.

Those are **instance members**.

A `static` member belongs to the type itself rather than to an individual object. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes?utm_source=chatgpt.com)

For example:

```
public static int Something { get; }
```

is accessed through the type:

```
JobApplication.Something
```

rather than:

```
application.Something
```

We won't force static members into JobTracker merely to practise them.

### Quick question

Which sounds conceptually more likely to be static?

**A.** The salary of one particular job application.

**B.** A utility operation that converts an annual salary to monthly salary without needing any application state.

Explain why.

---

# 14. Main exercise — JobTracker domain model

Now build the first real version of our model.

### Requirement

JobTracker needs to represent an employment opportunity.

Every application has:

- company;
- position;
- salary range;
- date discovered;
- current status.

It may additionally have:

- recruiter name;
- recruiter email;
- notes;
- application date.

A newly discovered opportunity starts as:

```
Interested
```

The user can later mark it as **applied**.

When that happens:

- status becomes `Applied`;
- application date is recorded.

External code should **not** be able to arbitrarily overwrite application status or application date.

The model shouldn't allow an obviously invalid salary range where the minimum exceeds the maximum.

### Your task

Design and implement it.

You decide:

- which types to create;
- constructors versus object initializers;
- which properties are nullable;
- access modifiers;
- `set` versus `private set` versus `init`;
- where `required` is useful;
- what methods the object exposes;
- how salary validation works.

You **don't have to use every feature from this topic**.

In fact, forcing every feature into the solution would be poor design.

### One restriction

Don't ask me for the implementation before attempting it.

Syntax questions are fine:

> “I can't remember how to declare an enum.”
> 

Design questions should initially come back to you:

> “Should Status have a private setter?”
> 

I'll help you reason through that rather than simply deciding it for you.

---

# 15. Code review

Once your implementation works, we'll review **your actual code together**.

These are the questions we'll use:

1. Can an invalid `JobApplication` be constructed?
2. Can client code put it into an invalid state after construction?
3. Which properties genuinely need to change?
4. Which properties can be `null`, and does each nullable choice reflect the domain?
5. Does the class expose implementation details unnecessarily?
6. Does `Apply()` represent behaviour better than exposing `Status` and `ApplicationDate` setters?
7. Is salary validation located where invalid salary data enters the model?
8. Is each public member something a consumer of `JobApplication` genuinely needs?
9. Are we using modern features because they improve the model, or merely because we've learned them?

We'll refactor only **after** answering those.

---

# 16. Knowledge check

### Concepts

**1.** What's the difference between a class and an object?

**2.** What does a variable of a class type contain?

**3.** What's the practical difference between a field and a property?

**4.** Why are public fields generally discouraged?

**5.** What problem does a constructor solve?

**6.** Explain encapsulation without using the phrase *“making fields private.”*

**7.** What's the difference between:

```
get; set;
```

and:

```
get; private set;
```

**8.** What does this communicate?

```
string? RecruiterName
```

**9.** Contrast `set` and `init`.

**10.** What does `required` enforce?

**11.** What's the difference between an instance member and a static member?

### Code reasoning

Without running this:

```
var first = new JobApplication(/* ... */);
var second = first;

second.Apply();
```

If `Apply()` changes the object's state, should that change be visible through `first`?

Why?

---

# 17. Exit coding drill

No second application.

Close the lesson and write from memory:

```
1. A class.
2. One private field.
3. One normal read/write property.
4. One property with a private setter.
5. A constructor receiving two parameters.
6. A nullable string property.
7. A required init-only property.
8. An instance method.
9. Instantiate the class.
10. Declare an enum with three values.
```

This should take minutes, not become another project.

Anything you can't produce is simply something we mark for later retrieval.

---

# 18. Quick reference

```
public class Example
{
    private int _value;

    public string Mutable { get; set; }

    public string Controlled { get; private set; }

    public string ReadOnly { get; }

    public string InitOnly { get; init; }

    public required string Required { get; init; }

    public string? Optional { get; set; }

    public Example(string readOnly)
    {
        ReadOnly = readOnly;
    }

    public void ChangeValue(int value)
    {
        _value = value;
    }
}
```

Treat this as a **syntax reference**, not a model of a well-designed real class.

### Official references

[Microsoft Learn — C# classes](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/classes?utm_source=chatgpt.com)

[Microsoft Learn — Properties](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties?utm_source=chatgpt.com)

[Microsoft Learn — Constructors](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-constructors?utm_source=chatgpt.com)

[Microsoft Learn — Access modifiers](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers?utm_source=chatgpt.com)

[Microsoft Learn — Nullable reference types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types?utm_source=chatgpt.com)

[Microsoft Learn — `init` properties](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init?utm_source=chatgpt.com)

This version deliberately shifts the difficulty upward: the syntax refresher is still there, but **the substantial work is the JobTracker modelling problem**, where I don't prescribe the class design. Topic 3 can then use the decisions you make here to introduce deeper OOP, composition, interfaces, inheritance where appropriate, and records without restarting the project.