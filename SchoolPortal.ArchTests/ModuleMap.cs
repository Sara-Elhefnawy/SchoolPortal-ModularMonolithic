using System.Reflection;

namespace SchoolPortal.ArchTests;

/// <summary>
/// One record per module, holding its three layer assemblies.
/// Assembly.Load(name) works here (instead of needing a marker type in each
/// project) because every module assembly is already a ProjectReference of
/// this test project, so it's guaranteed to be sitting in the test's own
/// output folder at test-run time.
/// </summary>
public sealed record ModuleAssemblies(
    string ModuleName,
    Assembly Domain,
    Assembly Application,
    Assembly Infrastructure);

internal static class ModuleMap
{
    // Add a line here when a new module is created — every test in this
    // project iterates this list, so nothing else needs to change.
    public static readonly IReadOnlyList<ModuleAssemblies> All =
    [
        Load("Students"),
        Load("Grades"),
        Load("Attendances"),
        Load("Notifications"),
    ];

    private static ModuleAssemblies Load(string moduleName) =>
        new(moduleName, Assembly.Load($"SchoolPortal.{moduleName}.Domain"),
            Assembly.Load($"SchoolPortal.{moduleName}.Application"),
            Assembly.Load($"SchoolPortal.{moduleName}.Infrastructure"));

    public static readonly Assembly Web = Assembly.Load("SchoolPortal.Web");
    public static readonly Assembly SharedKernel = Assembly.Load("SchoolPortal.SharedKernel");
}