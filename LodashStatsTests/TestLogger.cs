using Microsoft.Extensions.Logging;

namespace LodashStatsTests;

public class TestLogger<T> : ILogger<T>
{
    private class NoopDisposable : IDisposable
    {
        public static NoopDisposable Instance { get; } = new NoopDisposable();

        private NoopDisposable() { }

        public void Dispose()
        {
            // No operation performed
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return NoopDisposable.Instance;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine(formatter(state, exception!));
    }
}