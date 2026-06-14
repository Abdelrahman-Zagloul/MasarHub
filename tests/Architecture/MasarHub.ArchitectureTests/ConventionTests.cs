using FluentAssertions;
using MasarHub.API;
using MasarHub.Application;
using NetArchTest.Rules;
using System.Reflection;

namespace MasarHub.ArchitectureTests
{
    [Trait("ArchitectureTests", "Conventions")]
    public sealed class ConventionTests
    {
        private static readonly Assembly ApplicationAssembly = typeof(IApplicationAssemblyMarker).Assembly;
        private static readonly Assembly ApiAssembly = typeof(IApiAssemblyMarker).Assembly;

        private static readonly string ApplicationNamespace = ApplicationAssembly.GetName().Name!;
        private static readonly string ApiNamespace = ApiAssembly.GetName().Name!;
        private static readonly string FeatureApplicationNamespace = $"{ApplicationNamespace}.Features";
        [Fact]
        public void Commands_ShouldBeSealed()
        {
            var result = Types
                .InAssemblies([ApplicationAssembly])
                .That().ResideInNamespace(FeatureApplicationNamespace)
                .And().AreClasses()
                .And().HaveNameEndingWith("Command")
                .Should().BeSealed()
                .GetResult();

            AssertArchitecture(result);
        }

        [Fact]
        public void Queries_ShouldBeSealed()
        {
            var result = Types
                .InAssemblies([ApplicationAssembly])
                .That().ResideInNamespace(FeatureApplicationNamespace)
                .And().AreClasses()
                .And().HaveNameEndingWith("Query")
                .Should().BeSealed()
                .GetResult();

            AssertArchitecture(result);
        }

        [Fact]
        public void Handlers_ShouldBeSealed()
        {
            var result = Types
                .InAssemblies([ApplicationAssembly])
                .That().ResideInNamespace(FeatureApplicationNamespace)
                .And().AreClasses()
                .And().HaveNameEndingWith("Handler")
                .Should().BeSealed()
                .GetResult();

            AssertArchitecture(result);
        }

        [Fact]
        public void FeatureValidators_ShouldBeSealed()
        {
            var result = Types
                .InAssemblies([ApplicationAssembly])
                .That().ResideInNamespace(FeatureApplicationNamespace)
                .And().AreClasses()
                .And().HaveNameEndingWith("Validator")
                .Should().BeSealed()
                .GetResult();

            AssertArchitecture(result);
        }

        [Fact]
        public void ConcreteControllers_ShouldBeSealed()
        {
            var controllerTypes = Types
                .InAssemblies([ApiAssembly])
                .That().ResideInNamespace($"{ApiNamespace}.Controllers")
                .And().AreClasses()
                .And().HaveNameEndingWith("Controller")
                .Should().BeSealed()
                .GetTypes().Where(t => !t.IsAbstract)
                .ToArray();

            controllerTypes.Should().OnlyContain(t => t.IsSealed);
        }

        private static void AssertArchitecture(TestResult result)
        {
            var constraint = $"Violations: {string.Join(", ", result.FailingTypeNames ?? [])}";

            result.IsSuccessful.Should().BeTrue(constraint);
        }
    }
}