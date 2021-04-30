using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace AutoMapperDemo.Tests.Helpers
{
    internal static class LoggingHelper
    {
        public static ILoggerFactory CreateLoggerFactory(ITestOutputHelper testOutputHelper)
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new TestOutputProvider(testOutputHelper));
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });
        }
    }

    internal class TestOutputProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestOutputProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new TestOutputLogger(_testOutputHelper);
        }
    }

    internal class TestOutputLogger : ILogger, IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestOutputLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _testOutputHelper.WriteLine($"{logLevel}: {formatter(state, exception)}");
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
