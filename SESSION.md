# Unit 8 — Compact teaching session

Status: Prepared on 2026-11-10; teaching has not started. Start at U8-01.
Goal: One meaningful coding task plus brief reasoning checks. Aim for one short session;
skip redundant questions when competence is clear, not mandatory completion of every prompt.
Primary source: material/8 Generics & Abstractions 40dea85eec91802d8f45dbceeddf6286.md.
Prior reading is recorded, but all Unit 8 mastery remains unassessed.

## Teaching rules

- Ask one question at a time. No worksheet dump, setup or tool calls between theory answers.
- Explain unfamiliar syntax before testing it; remind directly when a familiar name is rusty.
- Keep explanations focused; do not read all material again during this prepared session.
- Code requirements and the filtered test command are in the source comments.
- Inspect implementation and run relevant tests when the learner requests review.
- Update progress at coding review/session end, retaining evidence and support needed.
- Preserve all Unit 7 code; TextPreview remains set aside, not a progression blocker.
- Unit 7 async-disposal uncertainty can be revisited with Unit 9; do not derail this compact unit.

## Core activities

### U8-01 — Retrieval (3 short prompts)

Unit/topics: 8 generic type arguments, 5 delegates, 7 exception scope.
Type/difficulty: Retrieval / Foundation. Status: Ready.
Sources: sections 1–6; Unit 7 batch-import review. Files/tests: Chat only.
Ask individually, moving quickly:
1. In Dictionary<Guid, Company>, what do the two type arguments specify?
2. What inputs and output does Func<Company, bool> represent?
3. If file reading and its consumer callback can both throw FileNotFoundException,
   why might one try around both operations hide the wrong failure?
Do not restart beginner exercises for concepts the learner already explains clearly.

### U8-02 — Constraints: explain, then diagnose

Unit/topics: 8 generic methods, inference, unconstrained T and combined constraints.
Type/difficulty: Explain + debugging / Application. Status: Ready.
Sources: sections 3–13. Files/tests: Chat only; prepares U8-03.
Briefly explain class versus method type parameters, inferred T, and where constraints.
Use the source's IHasName/GetName example; explain class/struct/base/interface/new()
constraints and combining only those actually needed (new() last). Keep it brief.
Then present:

```csharp
static Guid ReadId<T>(T entity) => entity.Id;
```

Ask: Why can this fail to compile even if every current caller passes an entity?
What compile-time guarantee is missing? Provide names/syntax directly if forgotten.

### U8-03 — One generic repository

Unit/topics: 8 constraints, generic type/interface, useful abstraction; 4 collection
encapsulation; 7 guards and expected absence.
Type/difficulty: Implementation / Application. Status: Ready; implementation pending.
Sources: sections 14–15 and 25.
Starter/spec: src/ExerciseWorkspace/Unit8/InMemoryRepository.cs (all requirements in comments).
Tests: tests/ExerciseWorkspace.Tests/Unit8/InMemoryRepositoryTests.cs, 8 cases.
Command: dotnet test --filter FullyQualifiedName~InMemoryRepositoryTests
Use the supplied entity contract and repository interface; learner chooses storage and
adds the entity constraint. Test fixture entities avoid changing Unit 7 learner types.
Review constraint correctness in code as well as runtime tests. Assess null/duplicate
handling, typed lookup, absence and snapshot behaviour. Do not rewrite cosmetic details.
One design check after review: Would this exercise justify adding generic repositories
around every EF Core entity, or what concrete need would justify that abstraction?

### U8-04 — Generic extension, delegate and tuple review

Unit/topics: 8 method inference, extension syntax, Func, tuples, deconstruction, nameof;
5 multiple enumeration interleave.
Type/difficulty: Worked-example review / Application. Status: Ready.
Sources: sections 16–17 and 22–24. Files/tests: Chat only; no extra coding assignment.
First explain this on the first parameter, named tuple return syntax and deconstruction
with a different tiny example. Then present this review snippet (not an assignment solution):

```csharp
public static (int Total, int Matches) Inspect<T>(
    this IEnumerable<T> source, Func<T, bool> predicate)
{
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(predicate);
    return (source.Count(), source.Count(predicate));
}
```

Given IEnumerable<Company> companies and var (total, matches) = companies.Inspect(c => c.Name != ""),
ask one at a time as needed: What is T inferred as? What makes this an extension method?
What risk does enumerating the source twice introduce, and what alternative would you choose?
Mention tuple versus named domain type briefly, not a modelling rabbit hole.
Use the null guards to ask why nameof(source) is preferable to a literal parameter name;
skip if the learner already demonstrates it. No solutions to pending prediction questions.

### U8-05 — Variance through safety

Unit/topics: 8 invariance, covariance, contravariance.
Type/difficulty: Explain + prediction / Application. Status: Ready.
Sources: sections 18–21. Files/tests: Chat only.
Explain producers/out and consumers/in using the source's Animal/Dog examples.
State that these generic variance conversions concern reference types; no advanced API design.
Then show assignments separately and ask which compile and why, assuming Dog : Animal:

```csharp
List<Animal> a = new List<Dog>();
IEnumerable<Animal> b = new List<Dog>();
Action<Animal> handleAnimal = animal => Console.WriteLine(animal);
Action<Dog> handleDog = handleAnimal;
```

For a rejected conversion, ask what unsafe operation it would otherwise permit.
Do not demand memorisation of every variance rule.

### U8-06 — Exit and stop

Unit/topics: 8 constraints and abstraction judgement.
Type/difficulty: Integrated reasoning / Application. Status: Ready.
Sources: sections 15, 25–27. Files/tests: Reviewed repository, no new scaffolding.
Ask one compact transfer question: If the repository were rewritten around object,
which compile-time guarantees would callers lose and where would casts/failures move?
If answers plus coding review show adequate fluency, finish initial Unit 8 practice.
Record specific unsupported/weak topics for later review; do not infer mastery from exposure.
No extra exercises just to fill the queue. Proceed toward Unit 9 when ready.

## Optional alternatives — only for demonstrated gaps

### U8-A1 — Constraint discrimination

Unit/topics: 8 class/struct/new() versus interface capability.
Type/difficulty: Debugging / Foundation. Status: Reserve, not mandatory.
Sources: sections 8–12. Files/tests: Chat only.
Prompt: Adding where T : class does not make value.Id compile. Why not?
Would new() guarantee Id? Ask for the capability actually needed, without adding another project.

### U8-A2 — Variance safety alternative

Unit/topics: 8 covariance/contravariance.
Type/difficulty: Prediction / Foundation. Status: Reserve, not mandatory.
Sources: sections 18–21. Files/tests: Chat only.
Prompt: A handler accepts every Animal. What promise does it already make about Dogs?
Conversely, can a Dog-only handler safely process every Animal? Ask one direction at a time.

## Coverage and evidence

U8-01/02/03 cover generic types, multiple parameters, methods/inference, object versus T,
constraints and typed interfaces. U8-04 covers generic extensions/delegates, tuples,
deconstruction and nameof. U8-05 covers invariance and both variance directions;
U8-03/06 assess whether abstraction is justified. This is a coverage map, not mastery evidence.
Unit 7 record: sessions/Unit7-2026-11-10.md. Repository code and test fixtures are preserved.

## Next chat

Start Unit 8 in teaching mode. Read AGENTS.md, PROGRESS.md and this file. Present U8-01
one prompt at a time, skip redundant checks, and use the prepared repository task.
Do not regenerate scaffolding, reveal the implementation or prolong basic questions.

## Preparation verification

One solution build/test pass on 2026-11-10: 49 existing cases pass; eight new cases
fail on NotImplementedException as expected. No learner code changed, no packages
added, no solutions supplied, and no mastery inferred from preparation.
