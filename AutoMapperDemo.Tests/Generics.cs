using AutoMapper;
using FluentAssertions;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class Generics
    {
        [Fact]
        public void Should_map()
        {
            IMapper configuration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap(typeof(Source<>), typeof(Destination<>));
                })
                .CreateMapper();

            var source = new Source<int> { Value = 10 };

            Destination<int> dest = configuration.Map<Source<int>, Destination<int>>(source);

            dest.Value.Should().Be(10);
        }

        private sealed record Source<T>
        {
            public T? Value { get; init; }
        }

        private sealed record Destination<T>
        {
            public T? Value { get; init; }
        }
    }
}