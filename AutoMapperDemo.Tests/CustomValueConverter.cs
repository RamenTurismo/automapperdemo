using AutoMapper;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class CustomValueConverter
    {
        [Fact]
        public void Example_map_member_level()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.MapFrom<ModelDtoArrayIntConverter>());
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
        public void Example_map_member_level_global_converter()
        {
            IMapper mapper = new MapperConfiguration(builder =>
                {
                    builder.CreateMap<Model, Dto>()
                        .ForMember(x => x.Value, x => x.MapFrom<PriceConverter>());
                })
                .CreateMapper();

            Model model = new()
            {
                Values = new[] { 2, 2 }
            };

            Dto dto = new()
            {
                Value = 5
            };

            mapper.Map<Model, Dto>(model, dto);

            dto.Value.Should().Be(25);
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

        private class PriceConverter : IValueResolver<object, object, int>
        {
            public int Resolve(object source, object destination, int destMember, ResolutionContext context)
            {
                return destMember * 5;
            }
        }

        private class ModelDtoArrayIntConverter : IValueResolver<Model, Dto, int>
        {
            public int Resolve(Model source, Dto destination, int destMember, ResolutionContext context)
            {
                return source.Values.Sum();
            }
        }
    }
}