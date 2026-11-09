# 1. C# Fundamentals Refresher

**Estimated study:** 1–2 sessions

**Project:** JobTracker — first console version

**Main objective:** Recover basic C# coding fluency before introducing classes and modern data modelling.

## 1. Learning objectives

By the end of this topic, **without AI and preferably without notes**, you should be able to:

- declare and initialize variables using appropriate types;
- explain what `var` does and does *not* do;
- use basic operators and expressions;
- write `if`/`else` and basic loops;
- declare and call methods;
- use parameters and return values;
- understand local-variable scope;
- read a small C# program and trace its execution;
- break a small problem into several sensible methods;
- compile and run a .NET console application.

The standard isn't *"I understand this when I see it."* It is **"I can write it."**

---

## 2. Retrieval warm-up — before reading

Do this first without searching.

**Task A — Variables**

Declare variables representing:

- your name;
- your age;
- a salary of €45,000.50;
- whether you're currently looking for work.

Don't worry if you can't remember everything.

**Task B — Method**

Write a method named `CalculateAnnualSalary` that:

- receives a monthly salary;
- multiplies it by 12;
- returns the result.

**Task C — Condition**

Given:

```
decimal salary = 45000m;
```

write code that prints `"High salary"` when the salary is at least 40,000 and `"Standard salary"` otherwise.

**Task D — Loop**

Print the numbers 1 through 10.

> **Do not correct these with AI yet.**
> 
> 
> Keep your first attempt. At the end of the topic, rewrite the same exercises and compare them.
> 

This gives us a baseline of what has actually become rusty.

---

# 3. Variables and the C# type system

C# is **strongly typed**. A variable has a type, and that determines which values and operations are valid for it. The compiler catches incompatible operations before the program executes. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/?utm_source=chatgpt.com)

The general form is:

```
type variableName = value;
```

For example:

```
int applicationCount = 5;
string companyName = "Microsoft";
decimal salary = 45000m;
bool isRemote = true;
```

Some types you will use constantly:

| Type | Typical use | Example |
| --- | --- | --- |
| `int` | Whole numbers | `int count = 5;` |
| `long` | Large whole numbers | `long population = 48000000;` |
| `double` | General floating-point calculations | `double average = 4.75;` |
| `decimal` | Money / decimal precision | `decimal salary = 45000m;` |
| `bool` | True/false | `bool isRemote = true;` |
| `char` | One character | `char grade = 'A';` |
| `string` | Text | `string company = "Microsoft";` |

Microsoft recommends `decimal` when exact decimal precision matters, including financial calculations. [Microsoft Learn](https://learn.microsoft.com/en-us/%20dotnet/csharp/fundamentals/types/built-in-types?utm_source=chatgpt.com)

### Exercise 1 — Write it

Don't copy the examples.

Declare appropriate variables for a job application containing:

1. company name;
2. number of employees;
3. advertised salary;
4. whether remote work is available;
5. number of interview rounds.

Then print some of them.

---

# 4. `var`: type inference, not dynamic typing

Modern C# code uses `var` frequently:

```
var company = "Microsoft";
var applications = 5;
```

The compiler infers the types:

```
company      → string
applications → int
```

`var` does **not** mean that the variable has no type or that its type can change. The compiler determines the type at compile time. [Microsoft Learn](https://learn.microsoft.com/en-us/%20dotnet/csharp/fundamentals/types/built-in-types?utm_source=chatgpt.com)

So this won't work:

```
var applications = 5;

applications = "five"; // compile error
```

We'll develop conventions around when explicit types and `var` improve readability later.

### Exercise 2 — Predict before running

What types will the compiler infer?

```
var a = 10;
var b = 10.5;
var c = 10.5m;
var d = "10";
var e = true;
```

Then verify your answers in the IDE.

---

# 5. Expressions, operators and decisions

Refresh the basic operators:

```
+  -  *  /  %
```

Comparisons:

```
==  !=  >  <  >=  <=
```

Boolean operators:

```
&&  ||  !
```

For example:

```
if (salary >= 50000 && isRemote)
{
    Console.WriteLine("Interesting opportunity");
}
else
{
    Console.WriteLine("Doesn't meet criteria");
}
```

### Exercise 3 — Eligibility

Write a small program that has:

```
decimal salary = 55000m;
bool isRemote = true;
int requiredOfficeDays = 1;
```

An application is interesting when:

- salary is at least €50,000; **and**
- it is remote **or** requires at most one office day.

Print whether the job is interesting.

Don't turn this into a method yet.

---

# 6. Repetition

You should still be comfortable reading and writing the three fundamental loop forms:

```
for (...)
{
}
```

```
while (...)
{
}
```

```
foreach (...)
{
}
```

We'll use `foreach` extensively once we introduce collections.

For now:

### Exercise 4 — Write from memory

Write a `for` loop that prints:

```
Application 1
Application 2
Application 3
Application 4
Application 5
```

Then modify it so that it only prints the even application numbers.

---

# 7. Methods — major coding drill

This is one of the skills I want to become automatic again.

Microsoft describes a method as a block of statements executed when the method is called. A method can receive typed parameters and return a value. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/methods?utm_source=chatgpt.com)

The basic anatomy is:

```
returnType MethodName(parameterType parameter)
{
    // code

    return value;
}
```

For example:

```
decimal CalculateAnnualSalary(decimal monthlySalary)
{
    return monthlySalary * 12;
}
```

Call it:

```
decimal annualSalary = CalculateAnnualSalary(4000m);
```

A method that doesn't return a value uses `void`:

```
void PrintWelcomeMessage(string name)
{
    Console.WriteLine($"Welcome {name}");
}
```

and:

```
PrintWelcomeMessage("Borja");
```

### Coding drill 1 — Method signatures

Write these **from scratch**:

**A.** `Add` receives two integers and returns their sum.

**B.** `IsHighSalary` receives a salary and returns `true` when it exceeds €50,000.

**C.** `PrintCompany` receives a company name and prints it. It returns nothing.

**D.** `CalculateMonthlySalary` receives an annual salary and returns the monthly salary.

Don't run them until you've written all four.

---

# 8. Parameters, arguments and return values

In:

```
decimal CalculateBonus(decimal salary, decimal percentage)
{
    return salary * percentage;
}
```

`salary` and `percentage` are **parameters**.

In:

```
var bonus = CalculateBonus(50000m, 0.10m);
```

`50000m` and `0.10m` are **arguments** supplied to the method.

By default, C# passes arguments by value. The implications become particularly important once we revisit value types versus reference types. [Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters?utm_source=chatgpt.com)

For now, concentrate on being able to design simple method signatures.

### Coding drill 2 — Design the signature

Don't implement these immediately.

First decide the **return type, method name, parameter types and parameter names** for functionality that:

- determines whether a job meets a minimum salary;
- calculates the salary increase between two jobs;
- prints information about an application;
- converts an annual salary into a monthly salary.

Then implement them.

---

# 9. Scope

Variables exist within a particular scope.

```
void CheckSalary(decimal salary)
{
    if (salary > 50000)
    {
        string message = "Good salary";
        Console.WriteLine(message);
    }

    // message isn't accessible here.
}
```

This will become much more important when we introduce classes, fields and object state in Topic 2.

### Exercise 5 — Find the problem

Without compiling it, explain why this doesn't work:

```
if (salary > 50000)
{
    string category = "High";
}

Console.WriteLine(category);
```

Then fix it.

---

# 10. First JobTracker challenge

Now stop following examples.

We don't have classes yet. That's deliberate.

Create a **console application** representing one job application.

It needs to know:

- company;
- position;
- salary;
- whether it's remote;
- number of interview stages.

The program must decide whether the opportunity is interesting.

**You decide the rule.**

Break the program into at least **two methods** where you think methods make sense.

Print a small summary such as:

```
Company: ...
Position: ...
Salary: ...
Remote: ...

Worth pursuing: ...
```

### Restrictions

Don't use:

- classes of your own;
- records;
- LINQ;
- AI-generated code.

You **can** consult the lesson for syntax.

The point is to make you decide:

> What data do I need?
> 
> 
> What type represents it?
> 
> What behaviour deserves a method?
> 
> What should that method receive?
> 
> What should it return?
> 

Those decisions are more important than the resulting program.

---

# 11. Code review questions

Once it works, answer these about your own implementation:

1. Why did you choose `decimal` or another type for salary?
2. Where did you use `var`, if anywhere? Why?
3. Could any variable have a more descriptive name?
4. Which pieces of behaviour did you extract into methods?
5. Could either method be made smaller?
6. Does each method have one clear responsibility?
7. Did you duplicate any logic?
8. Can someone understand the program without comments explaining every line?

This is our first connection to **clean code and responsibility**. We aren't going to postpone good design until an "architecture" module.

---

# 12. Retrieval test — close the notes

Write from memory:

```
1. Declare an integer.
2. Declare a decimal.
3. Declare a string.
4. Declare a boolean.
5. Declare a variable using var.
6. Write an if/else.
7. Write a for loop.
8. Write a method receiving two ints and returning an int.
9. Write a method receiving a string and returning nothing.
10. Call both methods.
```

Then answer:

**Q1.** Is C# statically or dynamically typed?

**Q2.** Does `var` make a variable dynamically typed?

**Q3.** What's the difference between a parameter and an argument?

**Q4.** What does `void` mean?

**Q5.** What does `return` do?

**Q6.** What's the difference between `=` and `==`?

**Q7.** When would `decimal` generally be preferable to `double`?

**Q8.** Why can't a local variable declared inside an `if` block normally be accessed outside that block?

Don't check the answers until you've committed to yours.

---

# 13. Exit test

Create a completely new console project.

Without copying JobTracker, implement this requirement:

> A training course has a name, number of completed lessons and total number of lessons. Calculate the completion percentage and print whether the course is complete.
> 

Requirements:

- choose appropriate variable types;
- write at least one method returning a value;
- write at least one method returning `bool`;
- use a conditional;
- produce readable console output.

No starter code.

If you can do that comfortably, **Topic 1 has done its job**.

---

# 14. Quick reference

```
// Variable
int count = 5;

// Inferred local variable
var name = "JobTracker";

// Constant
const int MonthsPerYear = 12;

// Condition
if (condition)
{
}
else
{
}

// Loop
for (int i = 0; i < 10; i++)
{
}

// Method returning a value
int Add(int a, int b)
{
    return a + b;
}

// Method returning nothing
void Print(string message)
{
    Console.WriteLine(message);
}

// Call
var result = Add(2, 3);
Print("Hello");
```

### Official references

[Microsoft Learn — C# type system](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/?utm_source=chatgpt.com)

[Microsoft Learn — Built-in types and literals](https://learn.microsoft.com/en-us/%20dotnet/csharp/fundamentals/types/built-in-types?utm_source=chatgpt.com)

[Microsoft Learn — Methods in C#](https://learn.microsoft.com/en-us/dotnet/csharp/methods?utm_source=chatgpt.com)

[Microsoft Learn — Method parameters](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters?utm_source=chatgpt.com)

---