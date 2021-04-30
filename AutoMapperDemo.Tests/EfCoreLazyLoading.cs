using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapperDemo.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AutoMapperDemo.Tests
{
    /// <summary>
    /// https://docs.automapper.org/en/stable/Queryable-Extensions.html
    /// </summary>
    public class EfCoreLazyLoading : IDisposable
    {
        private static readonly Random Random = new();
        private readonly MyDbContext _demoDbContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMapper _mapper;

        public EfCoreLazyLoading(ITestOutputHelper testOutputHelper)
        {
            _loggerFactory = LoggingHelper.CreateLoggerFactory(testOutputHelper);

            _demoDbContext = EfCoreHelper.CreateDbContext<MyDbContext>(configure =>
            {
                configure.UseLoggerFactory(_loggerFactory);
                configure.UseLazyLoadingProxies();
            });

            _mapper = new MapperConfiguration(
                    builder =>
                    {
                        builder.CreateMap<Domaine, DomaineDto>()
                            .ForMember(x => x.TotalArticles, x => x.MapFrom(e => e.Articles.Count()));

                        builder.CreateMap<Article, ArticleDto>();
                    })
                .CreateMapper();
        }

        [Fact]
        public void Example_with_select()
        {
            List<DomaineDto> result = _demoDbContext.Domaines
                .Select(e => new DomaineDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Articles = e.Articles.Select(a => 
                        new ArticleDto
                        {
                            Id = a.Id,
                            Price = a.Price
                        }).ToList()
                })
                .ToList();

            result.Should().HaveCount(3);
            result.First(e => e.Id == 1).Articles.Should().HaveCount(2);
            result.First(e => e.Id == 2).Articles.Should().HaveCount(3);
        }

        [Fact]
        public void Example_with_automapper_map()
        {
            // Queries N+1 : A query is performed for each Domaine element and then for each Articles in the Domaine element.
            DomaineDto[] result = _demoDbContext.Domaines
                .Select(e => _mapper.Map<DomaineDto>(e))
                .ToArray();

            result.Should().HaveCount(3);
            result.First(e => e.Id == 1).Articles.Should().HaveCount(2);
            result.First(e => e.Id == 2).Articles.Should().HaveCount(3);
        }

        [Fact]
        public void Example_with_automapper_projecto()
        {
            DomaineDto[] result = _demoDbContext.Domaines
                .ProjectTo<DomaineDto>(_mapper.ConfigurationProvider)
                .ToArray();

            result.Should().HaveCount(3);
            result.First(e => e.Id == 1).Articles.Should().HaveCount(2);
            result.First(e => e.Id == 2).Articles.Should().HaveCount(3);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _demoDbContext.Dispose();
            _loggerFactory.Dispose();
        }

        public class MyDbContext : DbContext
        {
            public virtual DbSet<Domaine> Domaines => Set<Domaine>();

            public virtual DbSet<Article> Articles => Set<Article>();

            /// <inheritdoc />
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Domaine>()
                    .HasData(
                        new Domaine
                        {
                            Id = 1,
                            Name = "Tabac"
                        },
                        new Domaine
                        {
                            Id = 2,
                            Name = "Presse"
                        },
                        new Domaine
                        {
                            Id = 3,
                            Name = "Librairie"
                        });

                modelBuilder.Entity<Article>()
                    .HasData(
                        new Article
                        {
                            Id = 1,
                            Price = Random.NextDouble(),
                            DomaineId = 1
                        },
                        new Article
                        {
                            Id = 2,
                            Price = Random.NextDouble(),
                            DomaineId = 1
                        },
                        new Article
                        {
                            Id = 3,
                            Price = Random.NextDouble(),
                            DomaineId = 2
                        },
                        new Article
                        {
                            Id = 4,
                            Price = Random.NextDouble(),
                            DomaineId = 2
                        },
                        new Article
                        {
                            Id = 5,
                            Price = Random.NextDouble(),
                            DomaineId = 2
                        });
            }

            /// <inheritdoc />
            public MyDbContext(DbContextOptions options) : base(options)
            {
            }
        }

        public record Domaine
        {
            public Domaine()
            {
                Articles = new HashSet<Article>();
            }

            public int Id { get; set; }
            public string? Name { get; set; }
            public virtual ICollection<Article> Articles { get; set; }
        }

        public record Article
        {
            public int Id { get; init; }
            public double Price { get; init; }
            public virtual Domaine? Domaine { get; init; }
            public virtual int DomaineId { get; init; }
        }

        public record DomaineDto
        {
            public DomaineDto()
            {
                Articles = new List<ArticleDto>();
            }

            public int Id { get; init; }
            public string? Name { get; init; }
            public List<ArticleDto> Articles { get; init; }
            public int TotalArticles { get; init; }
        }

        public record ArticleDto
        {
            public int Id { get; init; }
            public double Price { get; init; }
        }
    }
}
