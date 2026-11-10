using System.Text;
using ExerciseWorkspace;

namespace ExerciseWorkspace.Tests;

public class TextPreviewTests
{
    [Fact]
    public void Borrowed_stream_remains_open_after_success()
    {
        using var source = new ObservedStream("First line\nSecond line");

        Assert.Equal("First line", TextPreview.ReadFirstLine(source));

        Assert.False(source.WasDisposed);
        Assert.True(source.CanRead);
    }

    [Fact]
    public void Borrowed_stream_remains_open_when_input_is_empty()
    {
        using var source = new ObservedStream("");

        Assert.Throws<InvalidDataException>(() => TextPreview.ReadFirstLine(source));

        Assert.False(source.WasDisposed);
    }

    [Fact]
    public void Owned_stream_is_disposed_after_success()
    {
        using var source = new ObservedStream("First line\nSecond line");

        Assert.Equal("First line", TextPreview.LoadFirstLine(() => source));

        Assert.True(source.WasDisposed);
    }

    [Fact]
    public void Owned_stream_is_disposed_when_input_is_empty()
    {
        using var source = new ObservedStream("");

        Assert.Throws<InvalidDataException>(() => TextPreview.LoadFirstLine(() => source));

        Assert.True(source.WasDisposed);
    }

    [Fact]
    public void Borrowed_stream_remains_open_and_read_failure_propagates()
    {
        var failure = new IOException("Simulated read failure.");
        using var source = new ObservedStream("content", failure);

        var actual = Assert.Throws<IOException>(() => TextPreview.ReadFirstLine(source));

        Assert.Same(failure, actual);
        Assert.False(source.WasDisposed);
    }

    [Fact]
    public void Owned_stream_is_disposed_and_read_failure_propagates()
    {
        var failure = new IOException("Simulated read failure.");
        using var source = new ObservedStream("content", failure);

        var actual = Assert.Throws<IOException>(() => TextPreview.LoadFirstLine(() => source));

        Assert.Same(failure, actual);
        Assert.True(source.WasDisposed);
    }

    // Test fixture only: observes lifetime and can simulate an I/O failure.
    private sealed class ObservedStream(string text, IOException? failure = null)
        : MemoryStream(Encoding.UTF8.GetBytes(text))
    {
        public bool WasDisposed { get; private set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (failure is not null)
                throw failure;

            return base.Read(buffer, offset, count);
        }

        public override int Read(Span<byte> buffer)
        {
            if (failure is not null)
                throw failure;

            return base.Read(buffer);
        }

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
    }
}
