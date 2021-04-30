using AutoMapper;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AutoMapperDemo.Tests
{
    /// <summary>
    /// https://docs.automapper.org/en/stable/Value-converters.html
    /// </summary>
    public class ValueConverter
    {
        [Fact]
        public void Example_convert_using_with_converter()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.ConvertUsing<ArrayIntSumConverter, int[]>(e => e.Values));
                })
                .CreateMapper();

            Model model = new()
            {
                Values = new[] { 2, 2 }
            };

            Dto dto = mapper.Map<Dto>(model);

            dto.Value.Should().Be(4);
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
            public int Value { get; init; }
        }

        public class ArrayIntSumConverter : IValueConverter<int[], int>
        {
            public int Convert(int[] source, ResolutionContext context)
                => source.Sum();
        }
    }
}