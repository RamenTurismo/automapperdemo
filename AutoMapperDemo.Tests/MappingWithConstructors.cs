using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapperDemo.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class MappingWithConstructors
    {
        [Fact]
        public void Example_ctor_param()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<MyModel, MyDto>();

                    builder.CreateMap<MyDto, MyModel>()
                        .ForCtorParam("identifier", x => x.MapFrom(e => e.Id));
                })
                .CreateMapper();

            const int value = 78;

            MyDto destination = mapper.Map<MyDto>(new MyModel(1, value));
            MyModel source = mapper.Map<MyModel>(destination);

            source.Value.Should().Be(value);
            destination.Value.Should().Be(value);
        }
        
        [Fact]
        public void Example_construct_using()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<MyModel, MyDto>();

                    builder.CreateMap<MyDto, MyModel>()
                        .ConstructUsing(x => new MyModel(x.Id, x.Value));
                })
                .CreateMapper();

            const int value = 5;

            MyDto destination = mapper.Map<MyDto>(new MyModel(1, value));
            MyModel source = mapper.Map<MyModel>(destination);

            source.Value.Should().Be(value);
            destination.Value.Should().Be(value);
        }

        [Fact]
        public void Example_ctor_param_efcore()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<MyEntity, MyModel>()
                        .ForCtorParam("identifier", x => x.MapFrom(e => e.Id));

                    //builder.CreateMap<MyEntity, SourceClass>()
                    //    .ConstructUsing(x => new SourceClass(x.Id, x.Value));

                    // Doesn't work.
                    // ConstructUsing (Func<>): Could be ignore if the constructor's parameter names have the same naming.
                    
                    //builder.CreateMap<MyEntity, SourceClass>()
                    //    .ConstructUsing((x, context) =>
                    //    {
                    //        return new SourceClass(x.Id, x.Value);
                    //    });
                })
                .CreateMapper();
            
            using EfContext db = EfCoreHelper.CreateDbContext<EfContext>();
     
            List<MyModel> source = db.Entities
                .ProjectTo<MyModel>(mapper.ConfigurationProvider)
                .ToList();

            source.Should().OnlyHaveUniqueItems(e => e.Identifier);
            source.Should().OnlyHaveUniqueItems(e => e.Value);
            source.Should().HaveCount(2);
        }

        private sealed record MyModel
        {
            public MyModel(int identifier, int value)
            {
                Identifier = identifier;
                Value = value;
            }

            public int Identifier { get; init; }

            public int Value { get; }
        }

        private sealed record MyDto
        {
            public int Id { get; init; }

            public int Value { get; init; }
        }

        [Table("entity")]
        private sealed record MyEntity
        {
            [Key]
            public int Id { get; init; }
            
            public int Value { get; init; }
        }

        private class EfContext : DbContext
        {
            public DbSet<MyEntity> Entities => Set<MyEntity>();

            /// <inheritdoc />
            public EfContext(DbContextOptions options) : base(options)
            {
            }
            
            /// <inheritdoc />
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<MyEntity>()
                    .HasData(
                        new MyEntity
                        {
                            Id = 1,
                            Value = 23
                        },
                        new MyEntity
                        {
                            Id = 2,
                            Value = 87
                        });
            }
        }
    }
}