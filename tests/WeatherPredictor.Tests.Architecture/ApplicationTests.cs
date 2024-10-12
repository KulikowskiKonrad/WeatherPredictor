using NetArchTest.Rules;
using WeatherPredictor.Application.Abstractions.Commands;
using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Infrastructure;
using Xunit;

namespace WeatherPredictor.Tests.Architecture;

public class ApplicationTests : ArchTestBase
{
    [Fact]
    public void Command_Should_Be_Immutable()
    {
        var types = Types.InAssembly(WeatherPredictorInfrastructureAssembly.Assembly)
            .That()
            .ImplementInterface(typeof(ICommand))
            .GetTypes();

        AssertAreImmutable(types);
    }

    [Fact]
    public void Query_Should_Be_Immutable()
    {
        var types = Types.InAssembly(WeatherPredictorInfrastructureAssembly.Assembly)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .GetTypes();

        AssertAreImmutable(types);
    }

    [Fact]
    public void CommandHandler_Should_Have_Name_EndingWith_CommandHandler()
    {
        var result = Types.InAssembly(WeatherPredictorInfrastructureAssembly.Assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        AssertArchTestResult(result);
    }

    [Fact]
    public void QueryHandler_Should_Have_Name_EndingWith_QueryHandler()
    {
        var result = Types.InAssembly(WeatherPredictorInfrastructureAssembly.Assembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();

        AssertArchTestResult(result);
    }
}