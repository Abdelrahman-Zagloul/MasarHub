using FluentAssertions;
using MasarHub.API;
using MasarHub.Application;
using MasarHub.Domain;
using MasarHub.Infrastructure;
using MasarHub.Infrastructure.Persistence;
using NetArchTest.Rules;
using System.Reflection;

namespace MasarHub.ArchitectureTests;

[Trait("ArchitectureTests", "LayerDependency")]
public sealed class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(IDomainAssemblyMarker).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(IApplicationAssemblyMarker).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(IInfrastructureAssemblyMarker).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(IPersistenceAssemblyMarker).Assembly;
    private static readonly Assembly ApiAssembly = typeof(IApiAssemblyMarker).Assembly;

    private static readonly string DomainNamespace = DomainAssembly.GetName().Name!;
    private static readonly string ApplicationNamespace = ApplicationAssembly.GetName().Name!;
    private static readonly string InfrastructureNamespace = InfrastructureAssembly.GetName().Name!;
    private static readonly string PersistenceNamespace = PersistenceAssembly.GetName().Name!;
    private static readonly string ApiNamespace = ApiAssembly.GetName().Name!;

    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        var result = Types
            .InAssemblies([DomainAssembly])
            .ShouldNot()
            .HaveDependencyOnAny(
                ApplicationNamespace,
                InfrastructureNamespace,
                PersistenceNamespace,
                ApiNamespace)
            .GetResult();

        AssertArchitecture(result);
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructurePersistenceOrApi()
    {
        var result = Types
            .InAssemblies([ApplicationAssembly])
            .ShouldNot()
            .HaveDependencyOnAny(
                InfrastructureNamespace,
                PersistenceNamespace,
                ApiNamespace)
            .GetResult();

        AssertArchitecture(result);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types
            .InAssemblies([InfrastructureAssembly])
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace)
            .GetResult();

        AssertArchitecture(result);
    }

    [Fact]
    public void Persistence_ShouldNotDependOnApi()
    {
        var result = Types
            .InAssemblies([PersistenceAssembly])
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace)
            .GetResult();

        AssertArchitecture(result);
    }


    private static void AssertArchitecture(TestResult result)
    {
        var constraint = result.FailingTypeNames is not null
                ? $"Violations: {string.Join(", ", result.FailingTypeNames)}"
                : string.Empty;


        result.IsSuccessful.Should().BeTrue(constraint);
    }
}