using NetArchTest.Rules;
using Xunit;

namespace SchoolPortal.ArchTests;

public class LayerDependencyTests
{
    public static IEnumerable<object[]> Modules => ModuleMap.All.Select(m => new object[] { m });

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_should_not_depend_on_outer_layers(ModuleAssemblies module)
    {
        // Domain is the one layer every other layer depends on, directly or
        // transitively — if it starts depending back on Application/Infra/Web,
        // you've built a cycle, and cycles are what make "modular" monoliths
        // stop being modular. This is the single most important rule in the file.
        var result = Types.InAssembly(module.Domain).ShouldNot().HaveDependencyOnAny(module.Application.GetName().Name!,
            module.Infrastructure.GetName().Name!, "SchoolPortal.Web").GetResult();

        Assert.True(result.IsSuccessful, Format(result, module.ModuleName, "Domain"));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_should_not_depend_on_persistence_or_web_frameworks(ModuleAssemblies module)
    {
        // Domain should be testable and reusable without spinning up EF Core,
        // Dapper, or ASP.NET Core. If a domain entity needs one of these to
        // compile, persistence/web concerns have leaked into business rules.
        var result = Types.InAssembly(module.Domain).ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Dapper", "Microsoft.AspNetCore").GetResult();

        Assert.True(result.IsSuccessful, Format(result, module.ModuleName, "Domain (framework leak)"));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Application_should_not_depend_on_infrastructure_or_web(ModuleAssemblies module)
    {
        // Application defines the interfaces (repositories, Directory-pattern
        // ports, etc.); Infrastructure implements them. If Application starts
        // referencing Infrastructure, dependency inversion is broken and you
        // can no longer swap/test the implementation independently.
        var result = Types.InAssembly(module.Application).ShouldNot()
            .HaveDependencyOnAny(module.Infrastructure.GetName().Name!, "SchoolPortal.Web").GetResult();

        Assert.True(result.IsSuccessful, Format(result, module.ModuleName, "Application"));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Application_should_not_take_a_direct_dependency_on_ef_core_or_dapper(ModuleAssemblies module)
    {
        // Application should only see abstractions (e.g. the Directory-pattern
        // interfaces), never EF Core's DbContext or a raw Dapper connection —
        // those are Infrastructure's job to wire up behind the interface.
        var result = Types.InAssembly(module.Application)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Dapper")
            .GetResult();

        Assert.True(result.IsSuccessful, Format(result, module.ModuleName, "Application (persistence leak)"));
    }

    private static string Format(TestResult result, string module, string layer) =>
        $"{module}.{layer} has a forbidden dependency. Offending types: " +
        string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? []);
}