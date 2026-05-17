using NetArchTest.Rules;

namespace ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void SharedKernel_ShouldNotDependOnFeatureAssemblies()
    {
        TestResult result = Types.InAssembly(SharedKernelAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                DomainAssembly.GetName().Name,
                ApplicationAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name,
                GameRuntimeCommonAssembly.GetName().Name,
                UserCodeApiAssembly.GetName().Name,
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                IntegrationContractsAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void Domain_ShouldNotHaveDependencyOnApplication()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void ApplicationLayer_ShouldNotDependOnRuntimeOrChroniclesContexts()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                GameRuntimeCommonAssembly.GetName().Name,
                GameRuntimeAssembly.GetName().Name,
                UserCodeApiAssembly.GetName().Name,
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                IntegrationContractsAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotDependOnRuntimeOrChroniclesContexts()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                GameRuntimeCommonAssembly.GetName().Name,
                GameRuntimeAssembly.GetName().Name,
                UserCodeApiAssembly.GetName().Name,
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                IntegrationContractsAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void GameRuntimeCommon_ShouldRemainIndependentFromRuntimeHostsAndInfrastructure()
    {
        TestResult result = Types.InAssembly(GameRuntimeCommonAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                ApplicationAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name,
                UserCodeApiAssembly.GetName().Name,
                "GameRuntime.Hosting",
                "GameRuntime.Persistence",
                "GameRuntime.Realtime",
                "GameRuntime.Logic.Actions",
                "GameRuntime.Logic.NPC",
                "GameRuntime.Logic.Turns",
                "GameRuntime.Logic.User.Compilation",
                "GameRuntime.Logic.User.Execution",
                "GameRuntime.Logic.User.Intellisense",
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                IntegrationContractsAssembly.GetName().Name,
                "Microsoft.CodeAnalysis")
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void UserCodeApi_ShouldRemainIndependentFromRuntimeHostsAndInfrastructure()
    {
        TestResult result = Types.InAssembly(UserCodeApiAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                ApplicationAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name,
                "GameRuntime.Hosting",
                "GameRuntime.Persistence",
                "GameRuntime.Realtime",
                "GameRuntime.Logic.Actions",
                "GameRuntime.Logic.NPC",
                "GameRuntime.Logic.Turns",
                "GameRuntime.Logic.User.Compilation",
                "GameRuntime.Logic.User.Execution",
                "GameRuntime.Logic.User.Intellisense",
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name,
                IntegrationContractsAssembly.GetName().Name,
                "Microsoft.CodeAnalysis")
            .GetResult();

        ShouldBeSuccessful(result);
    }

    [Fact]
    public void GameRuntime_ShouldNotDependOnPresentationOrChroniclesContexts()
    {
        TestResult result = Types.InAssembly(GameRuntimeAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                InfrastructureAssembly.GetName().Name,
                PresentationAssembly.GetName().Name,
                ChroniclesDomainAssembly.GetName().Name,
                ChroniclesApplicationAssembly.GetName().Name,
                ChroniclesInfrastructureAssembly.GetName().Name)
            .GetResult();

        ShouldBeSuccessful(result);
    }
}
