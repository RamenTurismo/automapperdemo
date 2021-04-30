using AutoMapper;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class CustomTypeConversion
    {
        [Fact]
        public void Example_convert_using()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    // Expression based.
                    builder.CreateMap<Model, Dto>()
                        .ConvertUsing(m => new Dto
                        {
                            Value = m.Values.Sum()
                        });
                })
                .CreateMapper();

            Model model = new()
            {
                Values = new[] { 2, 2 }
            };

            Dto dto = mapper.Map<Dto>(model);

            dto.Value.Should().Be(4);
            dto.Id.Should().Be(default);
        }

        [Fact]
        public void Example_convert_using_with_converter()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<int[], int>()
                        .ConvertUsing<ArrayIntIntConverter>();

                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.MapFrom(e => e.Values));
                })
                .CreateMapper();

            Model model = new()
            {
                Values = new[] { 2, 2 }
            };

            Dto dto = mapper.Map<Dto>(model);

            dto.Value.Should().Be(4);
            dto.Id.Should().Be(default);
        }

        private sealed record Model
        {
            public Model()
            {
                Values = Array.Empty<int>();
            }

            public int[] Values { get; init; }
        }

        private sealed record Dto
        {
            public int Id { get; init; }

            public int Value { get; init; }
        }

        public class ArrayIntIntConverter : ITypeConverter<int[], int>
        {
            public int Convert(int[] source, int destination, ResolutionContext context)
            {
                return source.Sum();
            }
        }
    }
}