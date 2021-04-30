using AutoMapper;
using FluentAssertions;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class ConditionnalMapping
    {
        [Fact]
        public void Example_pre_condition()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.PreCondition(e => e.Value > 20));
                })
                .CreateMapper();

            Model model = new()
            {
                Value = 18
            };

            Dto dto = new()
            {
                Value = 5
            };

            mapper.Map<Model, Dto>(model, dto);

            dto.Value.Should().Be(5);
        }

        [Fact]
        public void Example_condition()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.Condition(e => e.Value > 20));
                })
                .CreateMapper();

            Model model = new()
            {
                Value = 18
            };

            Dto dto = new()
            {
                Value = 5
            };

            mapper.Map<Model, Dto>(model, dto);

            dto.Value.Should().Be(5);
        }

        private sealed record Model
        {
            public int Value { get; init; }
        }

        private sealed record Dto
        {
            public int Id { get; init; }

            public int Value { get; init; }
        }
    }
}