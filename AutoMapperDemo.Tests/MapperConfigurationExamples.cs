using AutoMapper;
using Xunit;

namespace AutoMapperDemo.Tests
{
    public class MapperConfigurationExamples
    {
        #region Example MapperConfiguration

        public void Example_auto_mapper_configuration()
        {
            MapperConfiguration autoMapperConfiguration = new(builder =>
            {
                builder.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                builder.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                builder.AddProfile<MyProfile>();
            });

            // For a few hundred mappings, this may take a couple of seconds.
            autoMapperConfiguration.CompileMappings();
        }

        private sealed class MyProfile : Profile
        {
            public MyProfile()
            {
                CreateMap<SourceClass, DestinationClass>();
            }
        }

        #endregion

        [Fact]
        public void Should_validate_mapper_destination()
        {
            MapperConfiguration autoMapperConfiguration = new(builder =>
            {
                builder.CreateMap<SourceClass, DestinationClass>()
                    .ForMember(x => x.City, x => x.Ignore());
            });

            autoMapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Should_validate_mapper_source()
        {
            MapperConfiguration autoMapperConfiguration = new(builder =>
            {
                builder.CreateMap<SourceClass, DestinationClass>(MemberList.Source)
                    .ForSourceMember(x => x.Coordinates, x => x.DoNotValidate());
            });

            autoMapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Should_validate_mapper_none()
        {
            MapperConfiguration autoMapperConfiguration = new(builder =>
            {
                builder.CreateMap<SourceClass, DestinationClass>(MemberList.None);
            });

            autoMapperConfiguration.AssertConfigurationIsValid();
        }

        public record SourceClass
        {
            public string? Location { get; init; }

            public string? Coordinates { get; init; }
        }

        public record DestinationClass
        {
            public string? City { get; init; }

            public string? Location { get; init; }
        }
    }
}