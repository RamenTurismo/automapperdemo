using AutoMapper;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class MappingInheritance
    {
        [Fact]
        public void Should_map()
        {
            MapperConfiguration configuration = new(cfg =>
            {
                cfg.CreateMap<ModelBase, DtoBase>()
                    .Include<MyModel, MyDto>()
                    .ForMember(x => x.ValueBase, x => x.MapFrom(e => e.SomeValue));

                cfg.CreateMap<MyModel, MyDto>();

                // OR :
                //cfg.CreateMap<MyModel, MyDto>()
                //    .IncludeBase<ModelBase, DtoBase>()
                //    .ForMember(x => x.ValueBase, x => x.MapFrom(e => e.SomeValue));

                //cfg.CreateMap<ModelBase, DtoBase>();

                // OR :
                //cfg.CreateMap<ModelBase, DtoBase>()
                //    .IncludeAllDerived()
                //    .ForMember(x => x.ValueBase, x => x.MapFrom(e => e.SomeValue));

                //cfg.CreateMap<MyModel, MyDto>();
            });

            configuration.AssertConfigurationIsValid();
        }

        private record ModelBase
        {
            public int SomeValue { get; init; }
        }

        private sealed record MyModel : ModelBase
        {
            public int Value { get; init; }
        }

        private abstract record DtoBase
        {
            public int ValueBase { get; init; }
        }

        private sealed record MyDto : DtoBase
        {
            public int Value { get; init; }
        }
    }
}