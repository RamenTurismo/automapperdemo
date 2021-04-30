using AutoMapper;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class MappingArrays
    {
        #region Map Enumerables

        [Fact]
        public void Should_map_enumerables()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<SourceClass, DestinationClass>()
                        .ForMember(x => x.Value, x => x.MapFrom(e => e.Sniped));
                })
                .CreateMapper();

            var sources = new SourceClass[]
            {
                new()
                {
                    Moves = new[]
                    {
                        1, 4, 8
                    },
                    Sniped = 391
                },
                new()
                {
                    Moves = new[]
                    {
                        7, 5, 6
                    },
                    Sniped = 8714
                },
                new()
                {
                    Moves = new[]
                    {
                        9, 6, 5, 4, 7, 53, 1, 3
                    },
                    Sniped = 471
                }
            };

            IEnumerable<DestinationClass> ienumerableDest = mapper.Map<SourceClass[], IEnumerable<DestinationClass>>(sources);
            ICollection<DestinationClass> icollectionDest = mapper.Map<SourceClass[], ICollection<DestinationClass>>(sources);
            IList<DestinationClass> ilistDest = mapper.Map<SourceClass[], IList<DestinationClass>>(sources);
            List<DestinationClass> listDest = mapper.Map<SourceClass[], List<DestinationClass>>(sources);
            DestinationClass[] arrayDest = mapper.Map<SourceClass[], DestinationClass[]>(sources);
        }

        private record SourceClass
        {
            public SourceClass()
            {
                Moves = Array.Empty<int>();
            }

            public int[] Moves { get; init; }

            public int Sniped { get; init; }
        }

        private record DestinationClass
        {
            public DestinationClass()
            {
                Moves = new List<int>();
            }

            public List<int> Moves { get; init; }

            public int Value { get; init; }
        }

        #endregion

        #region Map Arrays and Polymorphism

        [Fact]
        public void Should_convert_polymorph_class_to_array()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<ChildSource, ChildDestination>();

                    builder.CreateMap<ParentSource, ParentDestination>()
                        .Include<ChildSource, ChildDestination>();
                })
                .CreateMapper();

            ParentSource[] sources =
            {
                new ParentSource(),
                new ChildSource(),
                new ParentSource()
            };

            ParentDestination[] destinations = mapper.Map<ParentSource[], ParentDestination[]>(sources);

            destinations[0].Should().BeAssignableTo<ParentDestination>();
            destinations[1].Should().BeAssignableTo<ChildDestination>();
            destinations[2].Should().BeAssignableTo<ParentDestination>();
        }

        public record ParentSource
        {
            public int Value1 { get; init; }
        }

        public record ChildSource : ParentSource
        {
            public int Value2 { get; init; }
        }

        public record ParentDestination
        {
            public int Value1 { get; init; }
        }

        public record ChildDestination : ParentDestination
        {
            public int Value2 { get; init; }
        }

        #endregion
    }
}