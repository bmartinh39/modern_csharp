using ExerciseWorkspace.Unit8;

namespace ExerciseWorkspace.Tests.Unit8;

public class InMemoryRepositoryTests
{
    private sealed record ApplicationEntry(Guid Id, string Title) : IEntity;
    private sealed record CompanyEntry(Guid Id, string Name) : IEntity;

    [Fact]
    public void Stored_entity_is_returned_with_its_type_and_identity_preserved()
    {
        IRepository<ApplicationEntry> repository = new InMemoryRepository<ApplicationEntry>();
        var application = new ApplicationEntry(Guid.NewGuid(), "Backend developer");

        repository.Add(application);

        ApplicationEntry? result = repository.GetById(application.Id);
        Assert.Same(application, result);
    }

    [Fact]
    public void Unknown_id_returns_null()
    {
        var repository = new InMemoryRepository<ApplicationEntry>();

        Assert.Null(repository.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void New_repository_has_no_entities()
    {
        Assert.Empty(new InMemoryRepository<ApplicationEntry>().GetAll());
    }

    [Fact]
    public void Null_add_is_rejected_without_changing_contents()
    {
        var repository = new InMemoryRepository<ApplicationEntry>();
        var application = new ApplicationEntry(Guid.NewGuid(), "Backend developer");
        repository.Add(application);

        var error = Assert.Throws<ArgumentNullException>(() => repository.Add(null!));

        Assert.Equal("item", error.ParamName);
        Assert.Same(application, Assert.Single(repository.GetAll()));
    }

    [Fact]
    public void Duplicate_id_does_not_replace_the_existing_entity()
    {
        var repository = new InMemoryRepository<ApplicationEntry>();
        var first = new ApplicationEntry(Guid.NewGuid(), "First");
        repository.Add(first);

        var error = Assert.Throws<ArgumentException>(() =>
            repository.Add(new ApplicationEntry(first.Id, "Replacement")));

        Assert.Equal("item", error.ParamName);
        Assert.Same(first, repository.GetById(first.Id));
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void Snapshot_does_not_include_later_additions()
    {
        var repository = new InMemoryRepository<ApplicationEntry>();
        var first = new ApplicationEntry(Guid.NewGuid(), "First");
        repository.Add(first);
        var snapshot = repository.GetAll();

        repository.Add(new ApplicationEntry(Guid.NewGuid(), "Second"));

        Assert.Same(first, Assert.Single(snapshot));
        Assert.Equal(2, repository.GetAll().Count());
    }

    [Fact]
    public void Changing_returned_collection_cannot_remove_entities_from_repository()
    {
        var repository = new InMemoryRepository<ApplicationEntry>();
        var first = new ApplicationEntry(Guid.NewGuid(), "First");
        repository.Add(first);
        var snapshot = repository.GetAll();

        if (snapshot is ICollection<ApplicationEntry> collection && !collection.IsReadOnly)
            collection.Clear();
        else if (snapshot is ApplicationEntry[] array)
            array[0] = new ApplicationEntry(Guid.NewGuid(), "Replacement");

        Assert.Same(first, repository.GetById(first.Id));
        Assert.Same(first, Assert.Single(repository.GetAll()));
    }

    [Fact]
    public void Same_implementation_supports_a_second_entity_type_and_empty_guid()
    {
        IRepository<CompanyEntry> repository = new InMemoryRepository<CompanyEntry>();
        var company = new CompanyEntry(Guid.Empty, "Acme");

        repository.Add(company);

        CompanyEntry? result = repository.GetById(Guid.Empty);
        Assert.Same(company, result);
    }
}
