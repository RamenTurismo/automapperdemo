using AutoMapper;
using FluentAssertions;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class NullSubstition
    {
        [Fact]
        public void Example_condition()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.NullSubstitute(1));
                })
                .CreateMapper();

            Model model = new()
            {
                Value = null
            };

            Dto dto = new()
            {
                Value = 5
            };

            mapper.Map<Model, Dto>(model, dto);

            dto.Value.Should().Be(1);
        }

        private sealed record Model
        {
            public int? Value { get; init; }
        }

        private sealed record Dto
        {
            public int Value { get; init; }
        }
    }
}
