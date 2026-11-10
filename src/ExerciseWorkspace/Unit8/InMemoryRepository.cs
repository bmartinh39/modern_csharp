namespace ExerciseWorkspace.Unit8;

public interface IEntity
{
    Guid Id { get; }
}

public interface IRepository<T> where T : class
{
    void Add(T item);
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
}

// Unit 8: Implement one typed repository usable for different entity classes.
// - Add the constraint that guarantees T exposes IEntity.Id (retain class).
// - Add rejects null (ArgumentNullException, item) and duplicate IDs
//   (ArgumentException, item), leaving existing contents unchanged.
// - GetById returns the stored instance, or null when absent.
// - GetAll returns a shallow snapshot: later additions or changes to the returned
//   collection must not change each other's membership. Do not clone entities.
// - Keep types intact: no object/dynamic casts, databases or extra abstraction layers.
// Choose the internal collection and helpers. All Guid values are allowed.
// Reminder: ToArray()/ToList() create shallow collection copies; IEnumerable alone
// does not protect a mutable backing collection. No variance is needed in this class.
// Test: dotnet test --filter FullyQualifiedName~InMemoryRepositoryTests
public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{

    private readonly Dictionary<Guid, T> _items = [];

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (GetById(item.Id) != null)
            throw new ArgumentException("Item already exists", nameof(item));
        
        _items.Add(item.Id, item);        
    }

    public T? GetById(Guid id)
    {
        _items.TryGetValue(id, out T? value);

        return value;
    }

    public IEnumerable<T> GetAll()
    {
        var list = _items.Values.ToList();
        return list;
    }
}
