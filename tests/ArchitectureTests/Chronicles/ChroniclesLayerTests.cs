using NetArchTest.Rules;

namespace ArchitectureTests.Chronicles;

public sealed class ChroniclesLayerTests : BaseTest
{
    [Fact]
    public void ChroniclesDomain_ShouldNotDependOnApplicationOrInfrastructure()
    {
        TestResult result = Types.InAssembly(ChroniclesDomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                ApplicationAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void ChroniclesDomain_ShouldNotDependOnEfCore()
    {
        TestResult result = Types.InAssembly(ChroniclesDomainAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void ChroniclesApplication_ShouldNotDependOnInfrastructureOrTransportContracts()
    {
        TestResult result = Types.InAssembly(ChroniclesApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                ChroniclesInfrastructureAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name,
                IntegrationContractsAssembly.GetName().Name,
                "Chronicles.Worker",
                "Migrator")
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void ChroniclesInfrastructure_ShouldNotDependOnRuntimeHosts()
    {
        TestResult result = Types.InAssembly(ChroniclesInfrastructureAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                PresentationAssembly.GetName().Name,
                "Chronicles.Worker",
                "Migrator")
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void IntegrationContracts_ShouldRemainIndependentFromApplicationAndInfrastructure()
    {
        TestResult result = Types.InAssembly(IntegrationContractsAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                DomainAssembly.GetName().Name,
                ApplicationAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name,
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }
}
