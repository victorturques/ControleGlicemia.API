using AutoMapper;
using ControleGlicemia.API.Mappers;
using Xunit;

namespace ControleGlicemia.API.Tests.Mappers;

public class MappingProfileTests
{
    [Fact]
    public void MappingProfile_DeveSerValido()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        config.AssertConfigurationIsValid();
    }
}
