using System.Reflection;
using Chronicles.Application;
using Chronicles.Domain;
using Chronicles.Infrastructure.Database;
using Domain.Auth.Users;
using GameRuntime.Common.World;
using GameRuntime.Logic.User.Api;
using Infrastructure.Database.Auth;
using IntegrationContracts;
using NetArchTest.Rules;
using SharedKernel;
using Shouldly;
using GameRuntimeDependencyInjection = GameRuntime.DependencyInjection;
using RootApplicationDependencyInjection = Application.DependencyInjection;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly SharedKernelAssembly = typeof(Result).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(RootApplicationDependencyInjection).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(AuthDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
    protected static readonly Assembly GameRuntimeCommonAssembly = typeof(ArenaWorld).Assembly;
    protected static readonly Assembly GameRuntimeAssembly = typeof(GameRuntimeDependencyInjection).Assembly;
    protected static readonly Assembly UserCodeApiAssembly = typeof(UserAction).Assembly;
    protected static readonly Assembly ChroniclesDomainAssembly = typeof(ChronicleNomination).Assembly;
    protected static readonly Assembly ChroniclesApplicationAssembly = typeof(DependencyInjection).Assembly;
    protected static readonly Assembly ChroniclesInfrastructureAssembly = typeof(ChroniclesDbContext).Assembly;
    protected static readonly Assembly IntegrationContractsAssembly = typeof(IntegrationEventJson).Assembly;

    protected static void ShouldBeSuccessful(TestResult result)
    {
        string[] failingTypes = (result.FailingTypes ?? [])
            .Select(type => type.FullName ?? type.Name)
            .ToArray();

        result.IsSuccessful.ShouldBeTrue(string.Join(Environment.NewLine, failingTypes));
    }
}
