using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapperDemo.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AutoMapperDemo.Tests
{
    public class Parameterization : TestBase
    {
        private readonly ParameterizationDbContext _db;

        /// <inheritdoc />
        public Parameterization(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _db = CreateDbContext<ParameterizationDbContext>();
        }

        [Fact]
        public void Example_parameterization()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    int id = default;

                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Id, x => x.MapFrom(_ => id));
                })
                .CreateMapper();
            
            Dto dto = _db.Models
                .ProjectTo<Dto>(mapper.ConfigurationProvider, new { id = 92 })
                .Single();

            dto.Id.Should().Be(92);
            dto.ValuesLength.Should().Be(2);
        }

        private class ParameterizationDbContext : DbContext
        {
            public DbSet<Model> Models => Set<Model>();

            /// <inheritdoc />
            public ParameterizationDbContext(DbContextOptions options) : base(options)
            {
            }

            /// <inheritdoc />
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Model>(entityTypeBuilder =>
                {
                    entityTypeBuilder.Property(e => e.Values)
                        .HasColumnType("TEXT")
                        .HasConversion(
                            e => string.Join("|", e),
                            e => e.Split("|", StringSplitOptions.None).Select(s => Convert.ToInt32(s)).ToArray());

                        entityTypeBuilder.HasData(
                            new Model
                            {
                                LocalId = 220,
                                Values = new[] { 4, 8 }
                            });
                });
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _db.Dispose();
        }

        [Table("model")]
        private sealed record Model
        {
            public Model()
            {
                Values = Array.Empty<int>();
            }

            [Key]
            public int LocalId { get; init; }

            public int[] Values { get; init; }
        }

        private sealed record Dto
        {
            public int Id { get; init; }
            public int ValuesLength { get; init; }
        }
    }
}
