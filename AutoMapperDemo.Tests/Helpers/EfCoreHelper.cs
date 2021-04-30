using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace AutoMapperDemo.Tests.Helpers
{
    public static class EfCoreHelper
    {
        public static T CreateDbContext<T>(Action<DbContextOptionsBuilder>? options = null)
            where T : DbContext
        {
            SqliteConnection connection = new("Data Source=:memory:");
            connection.Open();

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                .UseSqlite(connection);

            options?.Invoke(optionsBuilder);

            T db = (T)Activator.CreateInstance(typeof(T), new object[] { optionsBuilder.Options })!;
            db.Database.EnsureCreated();

            return db;
        }
    }
}
