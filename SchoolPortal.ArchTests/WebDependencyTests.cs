using NetArchTest.Rules;
using SchoolPortal.Web;
using Xunit;

namespace SchoolPortal.ArchTests;

public class WebDependencyTests
{
    [Fact]
    public void Web_application_code_should_not_depend_directly_on_modules_infrastructure()
    {
        var infrastructureAssemblyNames = ModuleMap.All.Select(m => m.Infrastructure.GetName().Name!).ToArray();

        var result = Types.InAssembly(ModuleMap.Web).ShouldNot().HaveDependencyOnAny(infrastructureAssemblyNames)
            .GetResult();

        // Program.cs is the composition root and is intentionally allowed
        // to reference module Infrastructure assemblies.
        var failingTypes = result.FailingTypes?.Where(type => type != typeof(Program)).ToArray() ??
                           Enumerable.Empty<Type>().ToArray();

        var isSuccessful = failingTypes.Length == 0;

        Assert.True(isSuccessful,
            "SchoolPortal.Web application code depends directly on a module's Infrastructure layer. " +
            "Only Program.cs is allowed to reference Infrastructure. " + "Offending types: " +
            string.Join(", ", failingTypes.Select(type => type.Name)));
    }
}