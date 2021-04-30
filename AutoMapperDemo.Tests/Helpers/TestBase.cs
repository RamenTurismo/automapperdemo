using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace AutoMapperDemo.Tests.Helpers
{
    public abstract class TestBase : IDisposable
    {
        protected static readonly Random Random = new();

        private readonly ITestOutputHelper _testOutputHelper;

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            LoggerFactory = LoggingHelper.CreateLoggerFactory(_testOutputHelper);
        }

        /// <summary>
        /// This shouldn't be instanciated by tests but it's just a demo.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        protected T CreateDbContext<T>()
            where T : DbContext
        {
            return EfCoreHelper.CreateDbContext<T>(options => options.UseLoggerFactory(LoggerFactory));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoggerFactory.Dispose();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
