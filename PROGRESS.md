# C# Retraining Progress

## Overview

- Current unit: Unit 8 — Generics & Abstractions (initial practice complete).
- Current topic: Compact session complete; targeted variance and snapshot review pending.
- Record initialized: 2026-11-09.
- Last study session: 2026-11-10 — Unit 8 compact session; repository passes 8/8 targeted tests after guidance.
- Next recommended activity: Prepare Unit 9 with brief Unit 8 retrieval; retain Unit 7 async-disposal review. Do not restart Unit 8.
- Git checkpoint: Unit 7 code, tests, progress and workflow committed as 9154aae and pushed to origin/main on 2026-11-10. Detailed session archived in sessions/Unit7-2026-11-10.md.
- Session preference: Ask theory questions one at a time so the learner can focus.
- Difficulty feedback: Learner finds coding exercises too easy. Prefer realistic integrated implementation, debugging and design decisions. Specify observable requirements clearly without prescribing every language construct; give API/syntax reminders directly when needed. Avoid extending practice with trivial tasks already demonstrated.
- Exercise format preference: Concise requirements, essential reminders and the test command belong in the C# file's comments; SESSION.md indexes activities rather than duplicating coding specifications.
- Curriculum preference: Questions and exercises should test the actual source material. Do not make incidental APIs outside the units (such as StreamReader.leaveOpen) the assessed concept; provide any necessary API details directly.
- Prior practice clarification (2026-11-09): Learner reports Units 7–9 were read only, with no practical exercises completed. Prior coverage remains recorded; mastery remains unassessed. Practice history for Units 1–6 is not established here.

## Recording conventions

- Every checked topic below means **previously covered**, based on the learner's report that Units 1–9 have been studied. It does not mean independently demonstrated or mastered.
- Initial status is **Introduced** (previously covered; not yet assessed in this sandbox). Other allowed statuses: Not started, Practising, Needs review, Mastered.
- `—` in mastery means **unassessed**, not zero. Do not assign a numerical score until performance supplies evidence.
- `—` in dates means no assessment or review date exists yet. The setup date is not an assessment date.
- Evidence `P` on every initial topic means: learner-reported prior coverage; no independent sandbox evidence. It applies to that row only until replaced with observed evidence.
- Coverage completion = checked topic rows / total topic rows. Current percentages record reported prior study, not completed sandbox exercises.
- Unit mastery = average of assessed topic scores only; assessment coverage is recorded separately. With no assessments the average is undefined.

| Mastery | Meaning |
|---------|---------|
| 0 | Assessed as not understood (unassessed is recorded separately as —) |
| 1 | Recognises the concept |
| 2 | Explains correctly with some support |
| 3 | Applies independently in a focused exercise |
| 4 | Applies correctly in unfamiliar or integrated problems |

## Unit Summary

| Unit | Status | Completion: prior coverage | Mastery | Assessment coverage | Last reviewed |
|------|--------|----------------------------|---------|---------------------|---------------|
| 1 — C# Fundamentals Refresher | Introduced; previously covered | 10/10 (100%) | — | 0/10 | — |
| 2 — Classes & Encapsulation | Introduced; previously covered | 11/11 (100%) | — | 0/11 | — |
| 3 — OOP Design | Introduced; previously covered | 13/13 (100%) | — | 0/13 | — |
| 4 — Collections, Equality & Protecting State | Introduced; previously covered | 15/15 (100%) | — | 0/15 | — |
| 5 — LINQ, Lambdas, Delegates & IEnumerable | Introduced; previously covered | 21/21 (100%) | — | 0/21 | — |
| 6 — Modern Modelling, Equality & Pattern Matching | Introduced; previously covered | 18/18 (100%) | — | 0/18 | — |
| 7 — Defensive C# & Resource Management | Needs review; initial practice complete | 16/16 (100%) | 2.44/4 | 16/16 | 2026-11-10 |
| 8 — Generics & Abstractions | Needs review; initial practice complete | 16/16 (100%) | 2.00/4 | 5/16 | 2026-11-10 |
| 9 — Async C# | Introduced; previously covered | 27/27 (100%) | — | 0/27 | — |

## Unit 1 — C# Fundamentals Refresher

Source: [1 C# Fundamentals Refresher 3faea85eec918089a418dd9c7cedd3eb](material/1%20C%23%20Fundamentals%20Refresher%203faea85eec918089a418dd9c7cedd3eb.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Types, variable declarations and initialization | Introduced | — | — | — | P |
| [x] | `var`: compile-time inference versus dynamic typing | Introduced | — | — | — | P |
| [x] | Expressions, arithmetic, comparisons and Boolean operators | Introduced | — | — | — | P |
| [x] | Decisions: `if` / `else` and compound conditions | Introduced | — | — | — | P |
| [x] | Repetition: `for`, `foreach`, `while` and loop selection | Introduced | — | — | — | P |
| [x] | Method declarations, calls and decomposition | Introduced | — | — | — | P |
| [x] | Parameters, arguments, return types and return values | Introduced | — | — | — | P |
| [x] | Local-variable and block scope | Introduced | — | — | — | P |
| [x] | Tracing control flow and debugging basic errors | Introduced | — | — | — | P |
| [x] | Naming, formatting and modern console build/run conventions | Introduced | — | — | — | P |

## Unit 2 — Classes & Encapsulation

Source: [2 Classes, Objects & Encapsulation 3faea85eec9180b8844dd3439ac46724](material/2%20Classes%2C%20Objects%20%26%20Encapsulation%203faea85eec9180b8844dd3439ac46724.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Classes, objects, references and instantiation | Introduced | — | — | — | P |
| [x] | Fields versus properties | Introduced | — | — | — | P |
| [x] | Constructors and establishing valid initial state | Introduced | — | — | — | P |
| [x] | Access modifiers: `public`, `private`, `protected`, `internal` | Introduced | — | — | — | P |
| [x] | Encapsulation and controlling mutation | Introduced | — | — | — | P |
| [x] | State, invariants and domain methods | Introduced | — | — | — | P |
| [x] | Nullable reference types and compiler warnings | Introduced | — | — | — | P |
| [x] | Getter-only properties and `private set` | Introduced | — | — | — | P |
| [x] | `init` and object initialisers | Introduced | — | — | — | P |
| [x] | `required`: initialization requirements versus validation | Introduced | — | — | — | P |
| [x] | Instance versus `static` members | Introduced | — | — | — | P |

## Unit 3 — OOP Design

Source: [3 OOP Design Inheritance, Composition & Polymorphi 3fbea85eec9180edade8e4739f8f65cf](material/3%20OOP%20Design%20Inheritance%2C%20Composition%20%26%20Polymorphi%203fbea85eec9180edade8e4739f8f65cf.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | IS-A / HAS-A / CAN-DO relationship reasoning | Introduced | — | — | — | P |
| [x] | Base/derived classes and single class inheritance | Introduced | — | — | — | P |
| [x] | `protected` and access within a hierarchy | Introduced | — | — | — | P |
| [x] | Substitutability and behavioural contracts | Introduced | — | — | — | P |
| [x] | Polymorphism with `virtual` and `override` | Introduced | — | — | — | P |
| [x] | Abstract classes and abstract versus virtual members | Introduced | — | — | — | P |
| [x] | `base`: constructors and base behaviour | Introduced | — | — | — | P |
| [x] | Method hiding versus overriding | Introduced | — | — | — | P |
| [x] | `sealed` and limiting inheritance | Introduced | — | — | — | P |
| [x] | Composition of state and behaviour; coupling trade-offs | Introduced | — | — | — | P |
| [x] | Interfaces and multiple capabilities | Introduced | — | — | — | P |
| [x] | Abstract class versus interface decisions | Introduced | — | — | — | P |
| [x] | Composition plus interfaces; choosing useful abstractions over reuse alone | Introduced | — | — | — | P |

## Unit 4 — Collections, Equality & Protecting State

Source: [4 Collections, Equality & Protecting Object State 3fcea85eec918087b445dd61cf0e6aca](material/4%20Collections%2C%20Equality%20%26%20Protecting%20Object%20State%203fcea85eec918087b445dd61cf0e6aca.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Arrays and `List<T>`: indexing, ordering and mutation | Introduced | — | — | — | P |
| [x] | Choosing collections by access, ordering and uniqueness semantics | Introduced | — | — | — | P |
| [x] | `Dictionary<TKey,TValue>` and key-based lookup | Introduced | — | — | — | P |
| [x] | Safe dictionary lookup with `TryGetValue` | Introduced | — | — | — | P |
| [x] | `HashSet<T>` and uniqueness | Introduced | — | — | — | P |
| [x] | Reference equality versus value equality in collections | Introduced | — | — | — | P |
| [x] | `Equals` / `GetHashCode` consistency and hash-based collections | Introduced | — | — | — | P |
| [x] | Entity identity decisions for collection membership | Introduced | — | — | — | P |
| [x] | `IEnumerable<T>` and `ICollection<T>` capabilities | Introduced | — | — | — | P |
| [x] | `IReadOnlyCollection<T>` versus `IReadOnlyList<T>` | Introduced | — | — | — | P |
| [x] | Protecting internal collections through domain methods | Introduced | — | — | — | P |
| [x] | Read-only interfaces versus immutability; casts and shared references | Introduced | — | — | — | P |
| [x] | Defensive copies, live views and mutable elements | Introduced | — | — | — | P |
| [x] | `readonly` fields versus collection mutation | Introduced | — | — | — | P |
| [x] | Collection expressions and target typing | Introduced | — | — | — | P |

## Unit 5 — LINQ, Lambdas, Delegates & IEnumerable

Source: [5 LINQ, Lambdas, Delegates & IEnumerable T 406ea85eec9180a399b1de527e75322a](material/5%20LINQ%2C%20Lambdas%2C%20Delegates%20%26%20IEnumerable%20T%20406ea85eec9180a399b1de527e75322a.md).
Unit 5 introduces `yield return` through its delegate exercise rather than a worked explanation; Unit 8 supplies a generic iterator example and Unit 9 covers async iterators. Explain iterator semantics before requiring independent use in a future exercise.

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Delegates: methods as values and behaviour parameters | Introduced | — | — | — | P |
| [x] | `Func<>` and `Action<>` signatures | Introduced | — | — | — | P |
| [x] | Expression and statement lambdas | Introduced | — | — | — | P |
| [x] | Extension methods and their static-method syntax | Introduced | — | — | — | P |
| [x] | `IEnumerable<T>` and enumeration semantics | Introduced | — | — | — | P |
| [x] | `Where`: filtering | Introduced | — | — | — | P |
| [x] | `Select`: projection, anonymous types and filtering distinctions | Introduced | — | — | — | P |
| [x] | `OrderBy`, descending order and `ThenBy` | Introduced | — | — | — | P |
| [x] | `Any` and `All` quantifiers | Introduced | — | — | — | P |
| [x] | `First` / `Single` and their `OrDefault` variants | Introduced | — | — | — | P |
| [x] | `Count`, `Sum`, `Average`, `Min`, `Max` | Introduced | — | — | — | P |
| [x] | `Distinct`, `DistinctBy` and equality semantics | Introduced | — | — | — | P |
| [x] | `GroupBy` and group projections | Introduced | — | — | — | P |
| [x] | `SelectMany` and flattening | Introduced | — | — | — | P |
| [x] | `ToList`, `ToArray`, `ToDictionary` and materialisation | Introduced | — | — | — | P |
| [x] | Deferred execution and observing source mutations | Introduced | — | — | — | P |
| [x] | Multiple enumeration, repeated work and side effects | Introduced | — | — | — | P |
| [x] | Query syntax versus method syntax | Introduced | — | — | — | P |
| [x] | Introductory `IQueryable<T>` and provider execution versus in-memory LINQ | Introduced | — | — | — | P |
| [x] | Choosing loops versus LINQ and composing maintainable queries | Introduced | — | — | — | P |
| [x] | Iterator concepts and `yield return` (cross-reference Unit 9 async iterators) | Introduced | — | — | — | P |

## Unit 6 — Modern Modelling, Equality & Pattern Matching

Source: [6 Modern Modelling, Equality & Pattern Matching 407ea85eec918092ac64d261375cf2ec](material/6%20Modern%20Modelling%2C%20Equality%20%26%20Pattern%20Matching%20407ea85eec918092ac64d261375cf2ec.md).
Source: [6 Modern Modelling, Equality & Pattern Matching_v2 407ea85eec918061bc60e30ec92c465f](material/6%20Modern%20Modelling%2C%20Equality%20%26%20Pattern%20Matching_v2%20407ea85eec918061bc60e30ec92c465f.md).
Both versions preserved; expanded v2 is the primary coverage map, with original checked for additional topics.

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Reference versus value types and assignment semantics | Introduced | — | — | — | P |
| [x] | `struct` copying semantics, including reference-valued members | Introduced | — | — | — | P |
| [x] | Reference identity versus value equality | Introduced | — | — | — | P |
| [x] | Entities versus value objects | Introduced | — | — | — | P |
| [x] | Ordinary class equality and equality-dependent operations | Introduced | — | — | — | P |
| [x] | Record classes and generated value equality | Introduced | — | — | — | P |
| [x] | Positional versus non-positional records and behaviour | Introduced | — | — | — | P |
| [x] | Constructors, validation and invariants in records | Introduced | — | — | — | P |
| [x] | Immutability, `init` and shallow immutability | Introduced | — | — | — | P |
| [x] | `with` expressions, shallow copies and invariant risks | Introduced | — | — | — | P |
| [x] | `record struct` and `readonly record struct` | Introduced | — | — | — | P |
| [x] | Choosing class / record / struct for domain models | Introduced | — | — | — | P |
| [x] | Equality in sets, dictionaries and `Distinct` | Introduced | — | — | — | P |
| [x] | Switch expressions, arm order and discard `_` | Introduced | — | — | — | P |
| [x] | Relational and logical patterns: `and`, `or`, `not` | Introduced | — | — | — | P |
| [x] | Property and nested property patterns | Introduced | — | — | — | P |
| [x] | Type, combined type/property and null patterns | Introduced | — | — | — | P |
| [x] | List patterns and choosing clear domain classifications | Introduced | — | — | — | P |

## Unit 7 — Defensive C# & Resource Management

Source: [7 Defensive C# & Resource Management 408ea85eec91805b9b74ec5544ce90a1](material/7%20Defensive%20C%23%20%26%20Resource%20Management%20408ea85eec91805b9b74ec5544ce90a1.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Validation versus exception handling and boundary decisions | Practising | 2 | 2026-11-09 | Next study session | Identified that returning null from a broad catch conflates not-found with lookup/storage failure and hides information from the caller. |
| [x] | Guard clauses and preserving invariants | Practising | 3 | 2026-11-09 | Next study session | Independently placed all validation before assignments in constructor and Update, preserving state on rejection. Corrected exception precedence after feedback; shared helper extracted with guidance. |
| [x] | ArgumentException, ArgumentNullException, ArgumentOutOfRangeException | Practising | 2 | 2026-11-09 | Next study session | Negative-bound/order checks implemented with precedence feedback in SalaryRange; now independently uses ThrowIfNull in ApplicationInbox, including correct parameter name. Retain 2 pending independent mixed exception-selection review. |
| [x] | InvalidOperationException for invalid object state | Practising | 3 | 2026-11-09 | Next study session | Independently implemented Apply/Reject preconditions with InvalidOperationException and no state changes on invalid transitions; all workflow tests pass. |
| [x] | `try` / `catch`, propagation and specific catch ordering | Practising | 3 | 2026-11-09 | Next study session | Implemented ImportAttempt: handles selected IOException cases and lets original unhandled exceptions propagate; tests verify exception identity. Specific catch ordering remains unassessed. |
| [x] | Catch only when meaningful handling is possible | Practising | 2 | 2026-11-09 | Next study session | Explained handling at a layer with a meaningful response, otherwise propagation; cleanup versus handling distinction clarified. |
| [x] | `throw;` versus `throw ex;` and preserving stack traces | Practising | 2 | 2026-11-09 | Next study session | Correctly recalled that bare throw preserves the stack trace; rethrow-location detail clarified. No debugging task yet. |
| [x] | `finally` and cleanup control flow | Practising | 3 | 2026-11-09 | Next study session | Implemented onFinished in finally; four tests verify exactly-once execution after success, handled failure and propagated failures, including return paths. |
| [x] | Exception filters with `when` | Practising | 3 | 2026-11-09 | Next study session | Implemented type-plus-flag filter correctly after a syntax reminder using a different exception type; all four ImportAttempt cases pass. No solution supplied. |
| [x] | Expected absence/validation outcomes versus exceptional failure | Practising | 2 | 2026-11-09 | Next study session | Identified business-contract dependence; after a concrete scenario, correctly treated missing arbitrary ID as an expected outcome. |
| [x] | Custom exceptions when they add domain meaning | Practising | 2 | 2026-11-09 | Next study session | Correctly explained that a custom type adds little if callers handle all transition failures identically; it needs useful meaning/handling differentiation. Custom exception implementation not assessed. |
| [x] | Null guards, ThrowIfNull, `nameof` and explicit guard helpers | Practising | 3 | 2026-11-09 | Next study session | Explained compile-time annotations versus runtime input and implemented ThrowIfNull before adding to ApplicationInbox. Null input preserves existing count and names application in the exception. |
| [x] | Resource ownership and lifetime | Practising | 3 | 2026-11-09 | Next study session | Correctly implemented owned StreamReader lifetime in LoadText; prior borrowing/ownership-transfer predictions correct. Four file tests pass; code review confirms cleanup on read failure. |
| [x] | `IDisposable`, `using` statements and using declarations | Practising | 3 | 2026-11-09 | Next study session | Implemented using declaration around owned reader and returned ReadToEnd without swallowing exceptions. Tests verify content/file release and missing-file propagation; reviewed exceptional-exit cleanup. |
| [x] | Disposal versus garbage collection | Practising | 2 | 2026-11-09 | Next study session | Correctly stated disposal does not immediately reclaim managed memory; GC/unreachability distinction clarified. |
| [x] | `IAsyncDisposable` and `await using` | Needs review | 1 | 2026-11-09 | Next study session | Recognises await using; questioned synchronous disposal being immediate and what async cleanup waits for. Explained that both await cleanup completion, while genuine async I/O can avoid blocking. No independent transfer check after clarification; revisit with Unit 9. |

## Unit 8 — Generics & Abstractions

Source: [8 Generics & Abstractions 40dea85eec91802d8f45dbceeddf6286](material/8%20Generics%20%26%20Abstractions%2040dea85eec91802d8f45dbceeddf6286.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Generic types and multiple type parameters | Introduced | — | — | — | P |
| [x] | Generic methods and type inference | Introduced | — | — | — | P |
| [x] | Type safety versus `object` and casts | Introduced | — | — | — | P |
| [x] | Unconstrained `T` and the purpose of constraints | Practising | 3 | 2026-11-10 | 2026-11-13 | Diagnosed missing Id guarantee and correctly implemented class, IEntity constraint. |
| [x] | `where`: class, struct, base class, interface, new() | Introduced | — | — | — | P |
| [x] | Combining constraints | Introduced | — | — | — | P |
| [x] | Generic interfaces and reusable abstractions | Practising | 3 | 2026-11-10 | 2026-11-13 | Implemented supplied IRepository<T> with typed lookup for two entity types; dictionary/snapshot guidance needed, so no independent integrated mastery claim. |
| [x] | Useful generic abstractions versus premature generalisation | Introduced | — | — | — | P |
| [x] | Generic extension methods | Introduced | — | — | — | P |
| [x] | Delegates with generics: Func and Action | Introduced | — | — | — | P |
| [x] | Generic invariance and why mutable collections cannot freely convert | Needs review | 1 | 2026-11-10 | Next study session | Predicted List conversion compiles while identifying mixed-type insertion risk; compiler protection explained. |
| [x] | Covariance (`out`) and producer interfaces | Practising | 2 | 2026-11-10 | Next study session | Reasoned enumeration-only conversion is safe after explanation; purpose required a reuse example. |
| [x] | Contravariance (`in`) and consumer interfaces | Needs review | 1 | 2026-11-10 | Next study session | Tentatively accepted Animal handler as Dog handler after explanation; independent safety reasoning pending. |
| [x] | Tuples and named tuple elements | Introduced | — | — | — | P |
| [x] | Deconstruction | Introduced | — | — | — | P |
| [x] | `nameof` for parameter names and refactoring | Introduced | — | — | — | P |

## Unit 9 — Async C#

Source: [9 Async C# 40dea85eec9180be9557cd6855bc21cd](material/9%20Async%20C%23%2040dea85eec9180be9557cd6855bc21cd.md).

### Topics

| Previously covered | Topic | Status | Mastery /4 | Last assessed | Next review | Evidence |
|--------------------|-------|--------|------------|---------------|-------------|----------|
| [x] | Asynchronous I/O and blocking versus non-blocking waiting | Introduced | — | — | — | P |
| [x] | `Task`, `Task<T>` and completion/result semantics | Introduced | — | — | — | P |
| [x] | `async`, return types and Async naming conventions | Introduced | — | — | — | P |
| [x] | `await`, control flow and synchronous execution before suspension | Introduced | — | — | — | P |
| [x] | Awaiting does not inherently create a thread | Introduced | — | — | — | P |
| [x] | Sequential operations and dependency ordering | Introduced | — | — | — | P |
| [x] | Starting independent operations before awaiting | Introduced | — | — | — | P |
| [x] | `Task.WhenAll`, results and concurrent completion | Introduced | — | — | — | P |
| [x] | Concurrency versus CPU parallelism | Introduced | — | — | — | P |
| [x] | CPU-bound versus I/O-bound work and appropriate `Task.Run` | Introduced | — | — | — | P |
| [x] | Async call chains, `.Result` / `.Wait()` and blocking scalability costs | Introduced | — | — | — | P |
| [x] | Exception propagation through await and faulted task state | Introduced | — | — | — | P |
| [x] | Failures in `Task.WhenAll` and observing exceptions | Introduced | — | — | — | P |
| [x] | Cooperative cancellation and `CancellationToken` | Introduced | — | — | — | P |
| [x] | Token propagation and ThrowIfCancellationRequested | Introduced | — | — | — | P |
| [x] | CancellationTokenSource, requesting cancellation and disposal | Introduced | — | — | — | P |
| [x] | Cancellation versus faults and task states | Introduced | — | — | — | P |
| [x] | `async void` and event-handler exceptions to task-returning conventions | Introduced | — | — | — | P |
| [x] | Missing awaits and accidental fire-and-forget | Introduced | — | — | — | P |
| [x] | Sequential versus concurrent async loops and concurrency limits | Introduced | — | — | — | P |
| [x] | Async LINQ delegates produce sequences of tasks | Introduced | — | — | — | P |
| [x] | `IAsyncEnumerable<T>` and `await foreach` | Introduced | — | — | — | P |
| [x] | Async iterators with `yield return` | Introduced | — | — | — | P |
| [x] | Task of a collection versus asynchronous streaming | Introduced | — | — | — | P |
| [x] | ConfigureAwait recognition (introductory only) | Introduced | — | — | — | P |
| [x] | Fake async and unnecessary Task.Run around asynchronous I/O | Introduced | — | — | — | P |
| [x] | Resource lifetime across awaits and premature disposal | Introduced | — | — | — | P |

## Spaced Repetition Queue

Unit 7 focused implementation and Unit 8 compact practice reviewed; retain supported concepts for fresh transfer checks.

| Topic | Priority | Last assessed | Next review | Evidence / action |
|-------|----------|---------------|-------------|-------------------|
| Unit 8 variance safety | High | 2026-11-10 | Next study session | Fresh invariant-list and consumer-direction checks; then approximately 2026-11-13, 2026-11-17 and 2026-12-01 as performance warrants. |
| Unit 8 dictionary values, snapshots and as conversions | Medium | 2026-11-10 | Next study session | Distinguish entries from entities, shallow membership copies and conversion from transformation; implementation needed guidance. |
| Unit 8 constraints and typed abstractions | Low | 2026-11-10 | 2026-11-13 | Fresh short transfer check, followed by approximately +7/+21-day reviews. |
| Unit 7 exception selection | Medium | 2026-11-09 | Next study session | Distinguish bad arguments from invalid state using fresh scenarios; exact API names needed reminders. |
| Unit 7 async disposal: cleanup completion versus thread blocking | High | 2026-11-09 | Next study session | Distinguish synchronous from immediate; await does not guarantee thread release. Recognised syntax but conceptual understanding after explanation is not yet verified. |
| Unit 7 propagation, filters and finally | Low | 2026-11-09 | Next study session | Correct focused implementation; revisit with a different failure-handling scenario, then approximately +3/+7/+21 days as appropriate. |
| Unit 7 guard clauses and invariant preservation | Low | 2026-11-09 | Next study session | Use a different state-changing operation; learner placed validation before mutation independently. |
| Unit 7 rethrowing and disposal semantics | Low | 2026-11-09 | Next study session | Correct conceptual retrieval; verify transfer through later debugging/resource tasks. |

After observed learning, schedule first review for the next study session, then approximately +3, +7 and +21 days. Adapt to results. Priorities: high for misconceptions or inability to apply; medium for supported performance; low for correct independent application. Use fresh examples and occasional cross-unit combinations.

## Persistent Weaknesses

No persistent conceptual weakness established. Some standard exception names needed reminders; validation precedence needed one correction. Focused invariant protection demonstrated; broader practical fluency remains to be assessed.

## Exercise History

### 2026-11-09 — Unit 7 opening retrieval

- Explained why constructor validation plus public setters cannot protect lifetime invariants.
- Justified specific exceptions and identified invalid-state failures; exact standard names needed minor reminders.
- Correctly recalled stack-trace preservation, disposal on exceptional scope exit and disposal versus GC.
- Unit 7 scores above reflect conceptual explanations only (2/4), with no independent coding evidence yet.
- Follow-up: Apply validation through a state-preserving update, then revisit resource ownership.

### 2026-11-09 — SalaryRange validation and safe updates

- Status: Completed with targeted guidance; functional review passed and diagnostic wording corrected.
- Starter: src/ExerciseWorkspace/SalaryRange.cs. Requirements: EXERCISE.md.
- Initial starter: 12 requirement cases discovered and executed; all failed as expected.
- First learner implementation: 10/12 cases pass. Constructor and Update both validate before assignments, protecting existing state on rejection. Both fail for (0, -1): the ordering check throws ArgumentException before the negative maximum check can throw ArgumentOutOfRangeException. Feedback targets validation precedence; no code edited by Codex.
- Follow-up review: 12/12 requirement cases and the infrastructure test pass (13 total). Learner corrected validation precedence and extracted private static ValidateBounds, shared by both callers, following targeted guidance. No learner code edited by Codex.
- Follow-up inspection confirms both negative-bound messages now say cannot be negative. Learner correctly explained type-level static membership; tutor clarified that this helper depends only on parameters.
- Guard clauses/invariant preservation: 3/4 from independently validating before mutation. Exception-selection score remains 2/4 because precedence needed support. No broad mastery claim.
- Infrastructure checks are not learner assessment evidence.

### 2026-11-09 — Resource ownership predictions

- Correctly explained why a borrowed stream should normally remain open for its owner.
- Correctly predicted using-declaration lifetime on normal return and exception propagation; needed confirmation about enclosing scope.
- Diagnosed returning a disposed stream and proposed caller-owned disposal with using around the returned resource.
- Assessment: ownership 2/4; using semantics remain 2/4. These are chat reasoning results, not independent implementation evidence.
- Follow-up: Practise ownership and cleanup in runnable code; revisit with a fresh example next study session.

### 2026-11-09 — TextPreview resource-lifetime debugging

- Status: First learner implementation reviewed; incomplete and set aside after curriculum-alignment feedback.
- Files: src/ExerciseWorkspace/TextPreview.cs, RESOURCE-EXERCISE.md, tests/ExerciseWorkspace.Tests/TextPreviewTests.cs.
- Starter deliberately closes borrowed streams and relies on that mistake to mask faulty owned-stream cleanup.
- Baseline: Compiles; 3/6 tests pass and 3/6 fail as expected. Passing starter tests are not learner evidence.
- API reminders supplied before implementation: StreamReader ownership and leaveOpen, ReadLine, Func<Stream> factory, null-coalescing throw and buffered reads.
- Next action: Learner corrects ownership/cleanup; review stream and reader lifetimes on normal and exceptional exits.
- First learner review: All 19 repository tests pass (6 TextPreview cases). LoadFirstLine now uses using to guarantee owned-stream cleanup on failure. ReadFirstLine removed using entirely, leaving its StreamReader undisposed; requirement 2 remains unmet despite passing stream-lifetime tests. LoadFirstLine also retains redundant explicit Dispose. Feedback asks how to dispose the reader without closing the borrowed stream. No learner code or tests edited, no mastery increase or completion recorded.

### 2026-11-09 — Exception handling retrieval and ImportAttempt task

- Correctly reasoned about meaningful handling, expected absence, rethrow/finally output, false filters and absence of automatic retry.
- Scores reflect supported conceptual checks only; no independent implementation of these topics yet.
- Learner requested tighter alignment with source material; TextPreview set aside without marking complete. Latest inspection shows redundant explicit stream disposal removed, but reader remains undisposed.
- New task: src/ExerciseWorkspace/ImportAttempt.cs, EXCEPTION-EXERCISE.md and ImportAttemptTests.cs; based on Unit 7 sections 12–19. Action reminder supplied (Unit 5).
- Baseline builds; all four new tests fail on NotImplementedException as expected. No solution supplied.
- Follow-up: Review learner implementation, then revisit these concepts next study session with a fresh example.

### 2026-11-09 — ImportAttempt final review and session pause

- Result: All four relevant tests pass. Code review found no functional defects under the stated assumptions (non-null actions; onFinished does not throw).
- Calls import once, returns true on success, filters IOException by handleIoFailure and returns false only for handled failures. Other exceptions propagate without unnecessary catch/rethrow.
- finally invokes onFinished on success and all failure paths, including before returning a result.
- Learner requested a when syntax reminder; implementation remained learner-authored. No code or tests changed by Codex during review.
- Assessment: propagation, finally and filters each 3/4 for focused application; no integrated mastery claimed.
- Lesson paused at learner's request. No new activities assigned during pause. Next review is the next study session, followed by approximately +3/+7/+21-day reviews as appropriate.

### 2026-11-09 — U7-F01–F03 retrieval and application workflow

- Retrieval: Correctly recalled validation precedence, false filters and finally before return; other catch blocks clarified. Correctly distinguished nullable annotations from runtime guarantees.
- Exercise: JobApplication.Apply/Reject and ApplicationInbox.Add. Learner implementation passes all 9 targeted tests; no functional review findings.
- Correctly protects the specified transitions, validates before mutation, retains an externally read-only status and guards null before storing input. Code and tests left unchanged.
- Assessment: invalid-state exceptions 3/4, null guards 3/4; invariant protection remains 3/4. Standard argument-exception group remains 2/4 because earlier precedence required support.
- Next: U7-F04 catch ordering. Revisit null guards and state preconditions next study session; extend intervals according to later performance.

### 2026-11-09 — U7-F04–F07 review and LoadText

- Catch ordering: Recognised that a broader catch makes the specific one unreachable, but initially predicted compilation success; clarified that C# rejects this ordering. Revisit once with a fresh example.
- Custom exceptions: Correctly explained need for meaningful caller distinctions rather than a new name alone.
- Boundary review: Identified loss of information when a broad catch turns storage failures into the same null result as expected absence.
- LoadText: Four targeted tests pass; code review finds correct owned-reader disposal on normal and exceptional exits and unchanged failure propagation. No learner code edited.
- Assessment: validation-boundary reasoning 2/4; custom-exception judgement 2/4; ownership and synchronous disposal 3/4 each. No unit-completion claim yet; async disposal remains unassessed.
- Learner found the coding task too easy and reports a broader pattern. Adapt future practical work toward integrated reasoning and realistic debugging, without introducing unprepared API puzzles.

### 2026-11-09 — Async disposal discussion and integrated task preparation

- Correctly identified throw ex as losing stack-trace information in F08.
- Requested a deeper explanation after the initial await-using mapping check was too obvious; discussion exposed uncertainty about synchronous cleanup duration versus asynchronous waiting. Record recognition only (1/4), not mastery after explanation.
- Prepared F10 integrated batch importer to address difficulty feedback: source comments specify observable behaviour; learner chooses validation/control-flow design and exception scope. Reuses the existing LoadText implementation.
- Baseline: 13 new tests discovered; build succeeds and starter cases fail as expected. No learner implementation or existing tests changed.
- Status: Assigned; implementation and practical review pending. Completion cannot be inferred from preparation.

### 2026-11-10 — ApplicationBatchImporter first implementation review

- Result: 9/13 targeted tests pass; four fail for two distinct issues.
- Correct: Argument null guards, input order/duplicates, successful-import counting, ordinary missing-file reporting, propagation of unrelated I/O and reporting failures, reuse of owned-reader helper.
- Validation timing: Path entries are checked during processing; a later invalid entry is discovered only after an earlier missing-file callback has run. Three invalid-entry cases fail the before-any-side-effects contract.
- Exception scope: The same try encloses LoadText and importText. FileNotFoundException from importText is misclassified as a missing source, reported and swallowed, so the batch continues. One callback-failure case fails.
- Redundant catch/rethrow adds no behaviour; diagnostic observation, not another functional defect.
- No learner code or tests changed; targeted feedback supplied. Exercise incomplete, no mastery upgrade; focused earlier scores do not imply integrated mastery.
- Follow-up: Correct validation timing and read/callback exception boundaries, then re-review. Include these concepts in next-session retrieval.

### 2026-11-10 — ApplicationBatchImporter corrected implementation

- Result: 13/13 targeted tests pass; review found no functional defects against the stated contract.
- Validates all entries before reads/callbacks. Try/catch now encloses only LoadText; a missing source is reported and skipped, while callback failures propagate. Successful count is incremented only after callback completion; redundant catch/rethrow removed.
- Learner implemented corrections after feedback and explicit clarification about exception origin. Exercise complete with guidance; no score-4 or independent integrated mastery claim.
- Minor conventions only: use PascalCase for ValidatePaths and correct successful spelling. No learner code or tests edited by Codex.
- Initial Unit 7 practice is complete; retain targeted review for validation timing, exception scope, catch-order compiler rejection and async disposal. TextPreview stays set aside, not marked complete and not a progression blocker.
- Next study session: fresh transfer examples; repeat around +3, +7 and +21 days as performance warrants. Async disposal remains Needs review at 1/4 until independently explained or applied.

### 2026-11-10 — U8-01–03 retrieval, constraints and repository review

- Correctly identified dictionary key/value type arguments and Func<Company, bool> signature. Recognised ambiguous exception origin when reading and a callback share a catch scope.
- Explained that unconstrained T lacks an Id guarantee and proposed an interface constraint; correctly implemented class, IEntity.
- Repository: 8/8 targeted tests pass after correction. Null and duplicate guards, typed lookup, expected absence and stored-instance identity are correct.
- Support: Dictionary storage and relevant APIs suggested; shallow snapshot explained with ToList hint. First GetAll enumerated key/value pairs and attempted an incompatible as conversion, returning null (3/8 tests passed). After explanation of Values and conversion versus transformation, learner corrected it to Values.ToList().
- No learner code or tests edited by Codex. At this checkpoint the implementation was complete with guidance and the design check was pending; it was subsequently discussed below. Passing tests do not imply independent snapshot mastery.
- Follow-up: Revisit dictionary entries versus values, shallow snapshots and failed as conversions next study session; extend to approximately +3/+7/+21 days according to performance.

### 2026-11-10 — U8-03 design check and U8-04–06 session finish

- Repository design: proposed business-driven boundaries and grouping related entities; concrete justification versus automatic EF wrappers clarified.
- Correctly inferred Company, identified changing contents between enumerations and proposed one foreach with two counters. Repeated work/side effects explained.
- Invariance: initially predicted List<Dog> can convert to List<Animal>, despite recognising non-Dog insertion risk. Compiler protection explained. Subsequently recognised IEnumerable conversion safety; practical purpose explained with a reuse example.
- Contravariance: tentative correct acceptance only, without independent explanation. Recognition-level assessment retained.
- Exit: correctly explained loss of specific typing with object and need for casts, versus constrained generics. Runtime-failure consequence clarified.
- Extension syntax, tuples and deconstruction explained but not directly assessed. Other correct retrieval (type arguments, Func) and nameof use recorded as evidence without claiming broad topic mastery. Remaining topic scores stay unassessed pending fuller checks.
- Initial practice complete. Repository passes 8/8 tests after guidance. No learner code or tests changed by Codex. Next: prepare Unit 9 and carry forward targeted review; no extra Unit 8 assignment.

## Next Session

- Prepare Unit 9 from its source using the existing solution and projects. Unit 8 U8-01–06 are complete; do not restart them.
- Retain short variance/snapshot retrieval and Unit 7 async-disposal review. Use fresh examples and ask one question at a time.
- Unit 8 repository passes 8/8 tests after guidance. Preserve learner implementations; TextPreview remains set aside with its reader-disposal issue documented.
- SESSION.md and sessions/Unit8-2026-11-10.md preserve the completed activities. Unit 9 scaffolding has not been prepared.
