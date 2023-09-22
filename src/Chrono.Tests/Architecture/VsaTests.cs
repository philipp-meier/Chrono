using System.Reflection;
using ArchUnitNET.Fluent.Slices;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using NUnit.Framework;

namespace Chrono.Tests.Architecture;

[Category("Architecture")]
public class VsaTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(Assembly.Load("Chrono"))
        .Build();

    [Test]
    public void Ensure_Vertical_Slices_Not_Coupled()
    {
        // Chrono.Features.A types should not access Chrono.Features.B types etc.
        SliceRuleDefinition.Slices().Matching("Chrono.Features.(*)").Should()
            .NotDependOnEachOther().Check(Architecture);
    }
}
