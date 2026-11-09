# 3. OOP Design: Inheritance, Composition & Polymorphism

**Project:** JobTracker — evolving the domain model

**Main objective:** Move beyond knowing *how* inheritance works in C# to deciding **when inheritance is the right model, when composition is better, and why**.

This lesson is deliberately more theoretical than Topics 1–2. Inheritance versus composition is a design decision worth understanding properly rather than reducing to “prefer composition.”

Microsoft describes inheritance as creating a specialized type from a base type: a derived class **is a** more specialized version of its base class. C# allows a class to have one direct base class, while a class can implement multiple interfaces. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/inheritance?utm_source=chatgpt.com)

---

## 1. Learning objectives

By the end of this topic, you should be able to:

- explain inheritance, composition and polymorphism;
- distinguish **is-a**, **has-a** and **can-do** relationships;
- implement inheritance using base and derived classes;
- understand `virtual`, `override`, `abstract`, `base`, `protected` and `sealed`;
- understand substitutability and why it matters;
- recognise inheritance used merely for code reuse;
- model relationships using composition;
- explain why composition generally creates looser coupling;
- understand the role of interfaces and how they differ from base classes;
- decide whether JobTracker actually benefits from inheritance;
- defend a design choice rather than merely make the code compile.

---

# 2. Retrieval warm-up

Keep this short.

### A — Write from memory

Create a class called `Person` with:

- a name that must be provided through its constructor;
- a nullable email;
- a method called `ChangeEmail`.

No need to run it.

### B — Reason about your Topic 2 model

Your current `JobApplication` contains:

```
public string Company { get; }
```

but you also created:

```
public class Company
{
    public string Name { get; }
    public string Website { get; }
}
```

Without writing code:

**Would it now make more sense for a `JobApplication` to contain a `Company` rather than a `string`? Why?**

Hold onto that thought.

---

# 3. Three relationships: is-a, has-a, can-do

A useful starting point for OO modelling is to ask what relationship exists between concepts.

### Is-a → inheritance

A `Manager` **is an** `Employee`.

```
Employee
   ▲
   │
Manager
```

Potentially:

```
public class Employee
{
}

public class Manager : Employee
{
}
```

A `Manager` can therefore be treated as an `Employee`.

Microsoft explicitly describes inheritance in terms of this **is-a relationship**: a derived class represents a specialization of its base class. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/inheritance?utm_source=chatgpt.com)

---

### Has-a → composition

A `Car` **has an** `Engine`.

```
Car
 │
 └──── Engine
```

That doesn't mean:

```
class Car : Engine
```

A car isn't an engine.

Instead:

```
public class Car
{
    public Engine Engine { get; }
}
```

One object **contains or references another object**.

That's composition.

---

### Can-do → interface/capability

Something might be:

```
printable
comparable
searchable
exportable
```

without those things describing what it fundamentally **is**.

Microsoft makes this distinction explicitly: interfaces generally express capabilities rather than the same kind of *is-a* relationship represented by class inheritance. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/inheritance?utm_source=chatgpt.com)

A crude but useful initial heuristic is therefore:

```
IS-A    → consider inheritance
HAS-A   → consider composition
CAN-DO  → consider an interface
```

**Consider** is important. This is not an algorithm.

---

# 4. Inheritance mechanics

Suppose JobTracker eventually has users:

```
public class User
{
    public string Name { get; }

    public User(string name)
    {
        Name = name;
    }
}
```

We might create:

```
public class Recruiter : User
{
    public string Company { get; }

    public Recruiter(string name, string company)
        : base(name)
    {
        Company = company;
    }
}
```

`Recruiter` inherits accessible members from `User`.

So:

```
var recruiter = new Recruiter("Sarah", "Microsoft");

Console.WriteLine(recruiter.Name);
Console.WriteLine(recruiter.Company);
```

works even though `Name` is declared by `User`.

Constructors themselves aren't inherited. Here, `base(name)` selects and invokes the base-class constructor. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/inheritance?utm_source=chatgpt.com)

The object isn't conceptually:

```
Recruiter object
    └── User object
```

There is one object whose type derives from `User`.

That's different from composition.

---

# 5. `protected`

You've already used:

```
private
public
```

Inheritance introduces another important accessibility level:

```
protected
```

A `protected` member is accessible within its containing type and derived types.

For example:

```
public class Employee
{
    protected decimal Salary { get; set; }
}
```

A derived class can access it:

```
public class Manager : Employee
{
    public void IncreaseSalary(decimal amount)
    {
        Salary += amount;
    }
}
```

but arbitrary external code can't.

Be careful with `protected`, though. Making state `protected` means subclasses become dependent on the internal representation of the base class.

That increases coupling.

So:

> “A subclass needs this” does not automatically imply “make the field protected.”
> 

Sometimes a protected method or property exposing a controlled operation is safer than exposing the state itself.

---

# 6. Inheritance gives you substitutability

This is more important than avoiding duplicated properties.

Given:

```
public class Employee
{
}

public class Manager : Employee
{
}
```

this is valid:

```
Employee employee = new Manager();
```

because a `Manager` **is an `Employee`**.

Likewise:

```
void Pay(Employee employee)
{
}
```

can receive:

```
Pay(new Manager());
```

This is where inheritance becomes powerful.

Code written against the general abstraction can operate on specialized implementations.

That leads directly to **polymorphism**.

---

# 7. Polymorphism: `virtual` and `override`

Suppose:

```
public class Employee
{
    public virtual decimal CalculateBonus()
    {
        return 500m;
    }
}
```

A specialized type can change that behaviour:

```
public class Manager : Employee
{
    public override decimal CalculateBonus()
    {
        return 1500m;
    }
}
```

Now consider:

```
Employee employee = new Manager();

Console.WriteLine(employee.CalculateBonus());
```

Which implementation runs?

`Manager.CalculateBonus()`.

Even though the variable is declared as:

```
Employee
```

the actual object is:

```
Manager
```

and virtual dispatch selects the overridden implementation.

That's **runtime polymorphism**.

Microsoft describes this as one of the central capabilities of inheritance: derived types can override virtual members while still being treated through the base type. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/polymorphism?utm_source=chatgpt.com)

---

# 8. `abstract`: behaviour that subclasses must supply

Sometimes the base class cannot provide a sensible implementation.

Consider:

```
public abstract class Shape
{
    public abstract double CalculateArea();
}
```

There is no universal formula for the area of “a shape.”

Derived classes provide it:

```
public class Circle : Shape
{
    public double Radius { get; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }
}
```

An abstract class cannot itself be instantiated:

```
new Shape(); // error
```

but it can contain concrete implementation as well as abstract members. If a class contains an abstract member, the class itself must be abstract. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/inheritance?utm_source=chatgpt.com)

This makes an abstract base class capable of saying:

> Every subtype shares **this state and behaviour**, but each subtype must provide **this other behaviour**.
> 

---

# 9. `virtual` vs `abstract`

This distinction should become automatic.

```
public virtual void DoSomething()
{
    // default implementation
}
```

means:

> Here is the normal behaviour. A derived class **may replace it**.
> 

Whereas:

```
public abstract void DoSomething();
```

means:

> There is no implementation here. A concrete derived class **must provide one**.
> 

So:

|  | Base implementation | Derived override |
| --- | --- | --- |
| ordinary method | Yes | No |
| `virtual` | Yes | Optional |
| `abstract` | No | Required |

---

# 10. `base`

An override can still use its base implementation.

```
public class Employee
{
    public virtual void PrintDetails()
    {
        Console.WriteLine("Employee");
    }
}

public class Manager : Employee
{
    public override void PrintDetails()
    {
        base.PrintDetails();
        Console.WriteLine("Manager");
    }
}
```

`base` explicitly accesses the base-class implementation. It's also what you saw earlier when selecting a base constructor. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/base?utm_source=chatgpt.com)

---

# 11. A trap: method hiding is not overriding

Consider:

```
public class Animal
{
    public void Speak()
    {
        Console.WriteLine("Animal");
    }
}

public class Dog : Animal
{
    public void Speak()
    {
        Console.WriteLine("Dog");
    }
}
```

This is **not polymorphic overriding**.

C# requires the base member to be `virtual`/`abstract` and the derived member to explicitly use `override` for overriding.

A derived member can instead **hide** an inherited member, optionally explicitly using `new`, but that has different dispatch semantics. Microsoft deliberately distinguishes hiding from overriding. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/versioning-with-the-override-and-new-keywords?utm_source=chatgpt.com)

For the sort of application code we're writing, if you find yourself unintentionally hiding members, stop and examine the design.

---

# 12. So why not use inheritance everywhere?

Inheritance looks attractive:

```
Common properties?
        ↓
Put them in BaseClass

Common methods?
        ↓
Put them in BaseClass

Slight variation?
        ↓
Override something
```

And soon you have:

```
                 Application
                     │
          ┌──────────┴──────────┐
      JobApplication       InternshipApplication
            │
      RemoteJobApplication
            │
 SeniorRemoteJobApplication
```

The problem is that inheritance creates a **strong structural relationship** between types.

A derived class:

- depends on the base class's public/protected contract;
- inherits behaviour whether it needs all of it or not;
- can be affected when base behaviour changes;
- is constrained by C#'s single class inheritance;
- promises that it can sensibly be used wherever the base type is expected.

So inheritance shouldn't merely mean:

> These classes happen to contain similar code.
> 

It should represent a meaningful abstraction.

---

# 13. The crucial question: is it really an IS-A relationship?

Imagine:

```
public class Company
{
    public string Name { get; }
}
```

and:

```
public class JobApplication : Company
{
}
```

Why might someone do this?

Perhaps because a job application needs:

```
Name
Website
Address
```

and `Company` already contains them.

We've avoided duplication.

But the model now claims:

> A JobApplication **is a Company**.
> 

Which is nonsense.

And because inheritance gives us substitutability, this would compile:

```
void DeleteCompany(Company company)
{
}

DeleteCompany(jobApplication);
```

The type system now believes something that our domain doesn't.

**Code reuse alone is not enough justification for inheritance.**

---

# 14. Composition

Your Topic 2 model currently contains:

```
public string Company { get; }
```

But you've already created a `Company` type.

We could instead have:

```
public class JobApplication
{
    public Company Company { get; }
}
```

and:

```
public class Company
{
    public string Name { get; }
    public string Website { get; }
}
```

Now:

```
JobApplication
      │
      │ has a
      ▼
   Company
```

That's composition.

The model says:

> A job application **has a company**.
> 

That matches the domain.

---

# 15. Composition can include behaviour too

Composition isn't just replacing strings with objects.

Imagine:

```
public class SalaryRange
{
    public decimal Minimum { get; }
    public decimal Maximum { get; }

    // validation...
}
```

Then:

```
public class JobApplication
{
    public SalaryRange Salary { get; }
}
```

Now `JobApplication` doesn't have to know every rule governing salary ranges.

It delegates that responsibility to `SalaryRange`.

Conceptually:

```
JobApplication
    │
    ├── Company
    │
    └── SalaryRange
```

Each object owns a smaller coherent responsibility.

This is one of the major advantages of composition.

---

# 16. Composition tends to reduce coupling

Suppose `JobApplication` inherits:

```
JobApplication : Opportunity
```

`JobApplication` is now permanently part of that type hierarchy.

By contrast:

```
public class JobApplication
{
    private readonly SalaryRange _salaryRange;
}
```

means JobApplication **uses** another object.

That relationship is generally easier to change.

For example, later:

```
SalaryRange
    ↓
CompensationPackage
```

may require changing a member rather than restructuring an inheritance hierarchy.

Composition lets behaviour be assembled from collaborating objects rather than fixed primarily by a class hierarchy.

This is the motivation behind the common design principle:

> **Favor composition over inheritance.**
> 

But I don't want you to turn that into:

> Composition good. Inheritance bad.
> 

That's equally simplistic.

---

# 17. When inheritance is genuinely useful

Inheritance is strong when all of these are reasonably true:

**1. There is a real is-a relationship.**

```
SavingsAccount IS AN Account
```

**2. The base type represents a meaningful abstraction.**

`Account` makes sense independently as a concept.

**3. Derived types can genuinely substitute for the base type.**

Code expecting an `Account` can sensibly operate on a `SavingsAccount`.

**4. Derived types share important behaviour/state, not merely a few coincidentally identical properties.**

**5. The hierarchy is reasonably stable.**

If you're constantly reshaping the hierarchy as requirements change, inheritance may be fighting your domain.

Microsoft's current type-selection tutorial similarly presents inheritance as appropriate when a new type is a specialized version of the base and needs to reuse its state/behaviour while adding or tightening rules. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/choosing-types?utm_source=chatgpt.com)

---

# 18. Substitutability: the deeper test

Consider:

```
public class Bird
{
    public virtual void Fly()
    {
    }
}
```

Then:

```
public class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotSupportedException();
    }
}
```

At first glance:

> Penguin is a bird.
> 

Biologically, yes.

But our **software abstraction** says something stronger:

> Every `Bird` supports `Fly()`.
> 

Now:

```
void MakeBirdFly(Bird bird)
{
    bird.Fly();
}
```

works for most birds and explodes for:

```
MakeBirdFly(new Penguin());
```

So the inheritance hierarchy has made a promise that `Penguin` can't honour.

This connects to the **Liskov Substitution Principle (LSP)**:

> A subtype should be usable where its base type is expected without breaking the expectations established by that base type.
> 

You don't need to memorise a formal definition yet.

Remember the practical test:

> **If I replace the base object with this derived object, does the calling code still make sense?**
> 

If the subclass constantly has to disable, reject or contradict inherited behaviour, the abstraction is suspicious.

---

# 19. Composition solves the bird problem differently

Instead of saying every bird flies:

```
Bird
 ├── Eagle
 ├── Sparrow
 └── Penguin
       ↑
   inherits Fly()
```

we could model flying as a capability.

Conceptually:

```
Bird

Flying capability
   ▲
   │
Eagle
Sparrow
```

A penguin doesn't receive behaviour it cannot support.

We'll shortly see how interfaces help express that.

---

# 20. Interfaces

An interface defines a **contract**.

```
public interface IExportable
{
    string Export();
}
```

A class promises to satisfy it:

```
public class JobApplication : IExportable
{
    public string Export()
    {
        // ...
    }
}
```

Other code can depend on the contract:

```
void SaveExport(IExportable item)
{
    var content = item.Export();
}
```

That method doesn't need to know whether `item` is:

```
JobApplication
Invoice
Report
CV
```

It only cares that it **can export itself**.

Interfaces can contain methods, properties, indexers and events, and modern C# interfaces can also contain default implementations and static abstract/virtual members. But their fundamental role remains defining a contract. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface?utm_source=chatgpt.com)

---

# 21. Abstract class versus interface

This distinction matters.

Suppose we have:

```
public abstract class Application
{
    public DateTime Created { get; }

    protected Application()
    {
        Created = DateTime.Now;
    }

    public abstract void Submit();
}
```

This can provide:

- state;
- constructors;
- implemented behaviour;
- abstract behaviour;
- protected implementation details.

An interface:

```
public interface ISubmittable
{
    void Submit();
}
```

primarily expresses:

> Objects of this type support this operation.
> 

Modern interfaces are more capable than old C# interfaces, including default implementations, but they still don't represent per-instance state in the way a class does. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface?utm_source=chatgpt.com)

Also:

```
class X : BaseClass
```

only allows **one direct base class**.

But:

```
class X : IOne, ITwo, IThree
```

can implement multiple interfaces. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/inheritance?utm_source=chatgpt.com)

A useful initial distinction:

```
Abstract base class
    → shared identity/state/implementation
    → IS-A hierarchy

Interface
    → shared contract/capability
    → CAN-DO
```

Again, not an absolute rule.

---

# 22. Composition + interfaces: where things get powerful

Suppose JobTracker eventually sends notifications.

A poor first instinct might be:

```
NotificationService
       ▲
       │
EmailNotificationService
       ▲
       │
GmailNotificationService
```

Perhaps inheritance is appropriate. Perhaps not.

Another design is:

```
public interface INotificationSender
{
    void Send(string message);
}
```

with:

```
public class EmailNotificationSender : INotificationSender
{
    // ...
}
```

and:

```
public class JobTracker
{
    private readonly INotificationSender _notificationSender;

    public JobTracker(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }
}
```

Now JobTracker **has an** `INotificationSender`.

That's:

**composition + abstraction + polymorphism.**

Later we can supply:

```
EmailNotificationSender
TeamsNotificationSender
FakeNotificationSender
```

without making `JobTracker` inherit from any of them.

This pattern will become extremely important when we reach ASP.NET Core dependency injection and testing.

---

# 23. Inheritance and composition are not mutually exclusive

Real designs often use both.

For example:

```
                 Employee
                    ▲
                    │
                 Manager
                    │
                    │ has a
                    ▼
              Address
```

`Manager` inherits from `Employee`.

`Manager` contains an `Address`.

And perhaps:

```
Manager CAN approve expenses
```

through an interface.

A mature OO model isn't:

> Pick inheritance or composition.
> 

It's:

> **Represent each relationship using the mechanism that matches what that relationship means.**
> 

---

# 24. A decision framework

When considering inheritance, ask in roughly this order:

**Is B genuinely a specialized kind of A?**

If no → don't use inheritance.

**Can B safely be used anywhere A is expected?**

If no → don't use inheritance.

**Does B want essentially the complete contract of A?**

If it needs to disable significant inherited behaviour → probably don't use inheritance.

**Am I doing this primarily to reuse some code?**

If yes → investigate composition first.

**Will callers benefit from treating multiple derived types uniformly through A?**

If yes → inheritance becomes substantially more useful.

Then ask:

> Could A instead be something B **has** or **uses**?
> 

Sometimes the answer immediately reveals the better design.

---

# 25. Design diagnosis — no coding

For each relationship, choose:

**Inheritance / Composition / Interface / None yet**

and justify it.

### A

A job application belongs to a company.

```
JobApplication ? Company
```

### B

A `RemoteJobApplication` is identical to `JobApplication` except that it contains:

```
Remote = true
```

Should it inherit from `JobApplication`?

### C

Both `JobApplication` and `CV` can be exported to PDF.

Should:

```
CV : JobApplication
```

?

If not, how might you model their common capability?

### D

JobTracker supports:

```
LinkedInOpportunity
IndeedOpportunity
CompanyWebsiteOpportunity
```

All have different import logic but eventually produce job opportunities.

Would inheritance, interfaces or composition help?

Don't code these yet.

The explanations matter more than the answers.

---

# 26. Main exercise — evolve JobTracker

Now modify your actual Topic 2 code.

Requirements have changed.

### Requirement 1 — Companies are entities

A company now has:

```
Name
Website
```

Multiple job applications can belong to the same company.

Your `JobApplication` should no longer represent its company merely as a string.

---

### Requirement 2 — Compensation deserves modelling

Salary information is becoming more complicated.

For now it still consists of:

```
Minimum
Maximum
```

but salary validation should no longer be the responsibility of `JobApplication`.

Move that responsibility somewhere more appropriate.

Think about:

> Is this inheritance or composition?
> 

---

### Requirement 3 — Different opportunity sources

Job opportunities may be discovered through:

```
LinkedIn
Indeed
Company website
Recruiter
Other
```

For now, don't assume this requires inheritance.

Choose the simplest appropriate representation.

---

### Requirement 4 — Exporting

We anticipate that multiple unrelated types may eventually be exportable.

For this exercise:

```
JobApplication
Company
```

should both be capable of producing a simple text export.

The caller should be able to operate on either without knowing its concrete type.

This is intentionally asking you to introduce **polymorphism**.

Decide what abstraction fits.

---

### Requirement 5 — Don't force inheritance

Your solution **does not have to contain a base class**.

That's deliberate.

If you believe JobTracker currently has no genuine class-inheritance relationship, don't invent one just because this is the inheritance lesson.

That's an important part of the exercise.

---

# 27. The inheritance challenge

Separately from JobTracker, I do want you to implement inheritance once so we can verify the mechanics.

Model:

```
PaymentMethod
    ▲
    │
 ┌──┴──────────┐
 │             │
CardPayment   BankTransfer
```

Requirements:

`PaymentMethod`:

- cannot be instantiated directly;
- has a `Name`;
- requires derived types to implement:

```
ProcessPayment(decimal amount)
```

`CardPayment`:

- adds `LastFourDigits`;
- implements payment processing differently.

`BankTransfer`:

- adds `AccountNumber`;
- implements payment processing differently.

Then write:

```
List<PaymentMethod>
```

containing both concrete types and process the same amount through every item.

This is small, but unlike another CRUD-style toy program, it exists to demonstrate one specific thing:

> **polymorphic dispatch through a base type.**
> 

---

# 28. Then modify it

Once it works, change `PaymentMethod.ProcessPayment()` from:

```
abstract
```

to:

```
virtual
```

Give it a default implementation.

Make one subtype override it and one inherit the default behaviour.

Run the same loop.

You should be able to explain exactly why the resulting behaviour changes.

That's enough inheritance syntax practice.

---

# 29. Design challenge — inheritance or composition?

Now consider a new requirement.

A payment can optionally:

- log transactions;
- send receipts;
- perform fraud checking.

One tempting design would be:

```
PaymentMethod
    ▲
LoggedPaymentMethod
    ▲
ReceiptSendingPaymentMethod
    ▲
FraudCheckedPaymentMethod
```

Before writing anything, consider what happens when you need:

```
Card + logging
Card + fraud check
Card + logging + receipt
Bank transfer + logging
Bank transfer + receipt
Bank transfer + fraud + logging
...
```

This is sometimes called a **combinatorial explosion** of subclasses.

Think about whether these behaviours are better represented as things a payment processor **has/uses** rather than things it **is**.

You don't need to implement the full alternative yet.

Sketch the types you'd use.

---

# 30. Knowledge check

Don't answer from the wording alone. Use examples.

**1.** What promise does:

```
class Manager : Employee
```

make beyond simply allowing `Manager` to reuse `Employee` code?

**2.** Why is code reuse alone a weak justification for inheritance?

**3.** Explain the difference between:

```
IS-A
HAS-A
CAN-DO
```

and give one example of each.

**4.** What's the difference between `virtual` and `abstract`?

**5.** What does `override` tell the compiler?

**6.** What does `base` do?

**7.** Why might excessive `protected` state make a hierarchy fragile?

**8.** Why can this be problematic?

```
class Penguin : Bird
```

if `Bird` promises `Fly()`?

**9.** What does substitutability mean in practical terms?

**10.** Why might:

```
JobApplication
    has a Company
```

be preferable to:

```
JobApplication : Company
```

**11.** What's the major conceptual difference between an abstract base class and an interface?

**12.** Why can composition make changing behaviour easier?

**13.** Can a C# class inherit from multiple classes?

**14.** Can it implement multiple interfaces?

**15.** If two classes both have `Name` and `CreatedDate`, is that enough reason to introduce a common base class?

---

# 31. Code-reading test

Predict the output without running it:

```
public abstract class Notification
{
    public virtual void Send()
    {
        Console.WriteLine("Sending notification");
    }
}

public class EmailNotification : Notification
{
    public override void Send()
    {
        Console.WriteLine("Sending email");
    }
}

public class SmsNotification : Notification
{
}

Notification first = new EmailNotification();
Notification second = new SmsNotification();

first.Send();
second.Send();
```

Then answer:

1. Why can both objects be stored in variables of type `Notification`?
2. Why does `first.Send()` not execute the base implementation?
3. Why does `second.Send()` execute it?
4. What would change if `Send()` were `abstract`?

---

# 32. Exit check

No second project.

From memory, write only enough code to demonstrate:

```
1. An abstract base class.
2. A constructor in that base class.
3. One abstract method.
4. One virtual method.
5. A derived class calling the base constructor.
6. An override of the abstract method.
7. An override of the virtual method.
8. A variable typed as the base class containing the derived object.
9. An interface.
10. A class implementing that interface.
```

Then write, in your own words:

> **When would I choose inheritance over composition?**
> 

That final answer is more important than remembering the precise syntax of `base(...)`.

---

# 33. Quick reference

```
// Inheritance
public class Derived : Base
{
}

// Base constructor
public Derived(string name)
    : base(name)
{
}

// Overridable implementation
public virtual void Execute()
{
}

// Override
public override void Execute()
{
}

// Required derived implementation
public abstract void Execute();

// Abstract class
public abstract class Base
{
}

// Prevent further inheritance
public sealed class FinalType
{
}

// Interface
public interface IExportable
{
    string Export();
}

// Interface implementation
public class Report : IExportable
{
    public string Export()
    {
        return "...";
    }
}

// Composition
public class Order
{
    public Customer Customer { get; }

    public Order(Customer customer)
    {
        Customer = customer;
    }
}

// Polymorphism
Base item = new Derived(...);
item.Execute();
```

## Official references

[Microsoft Learn — Inheritance in C#](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/inheritance?utm_source=chatgpt.com) — base/derived classes, abstract and virtual members, interfaces and sealing. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/inheritance?utm_source=chatgpt.com)

[Microsoft Learn — Introduction to inheritance](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/inheritance?utm_source=chatgpt.com) — particularly useful for the **is-a** relationship and its distinction from interface capabilities. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/inheritance?utm_source=chatgpt.com)

[Microsoft Learn — Polymorphism](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/polymorphism?utm_source=chatgpt.com) — `virtual`, `override`, runtime dispatch and base behaviour. [Microsoft Learn](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/polymorphism?utm_source=chatgpt.com)

[Microsoft Learn — Interfaces](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface?utm_source=chatgpt.com) — interface contracts and modern interface capabilities. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface?utm_source=chatgpt.com)

[Microsoft Learn — Choosing between C# types](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/choosing-types?utm_source=chatgpt.com) — useful current guidance on classes, records, structs and inheritance. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/choosing-types?utm_source=chatgpt.com)

This is the level I'd use from here: the syntax is still recorded so the Notion course remains reusable, but the main learning burden has moved to **modelling, trade-offs and explaining why a design is appropriate**.