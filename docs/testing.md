# Тестирование

## Тестовый стек

- `xUnit` — test runner и базовая модель тестов.
- `Shouldly` — читаемые assertion'ы без зависимости от FluentAssertions.
- `NetArchTest.Rules` — architecture-тесты границ слоёв и bounded context.
- Ручные fakes/stubs вместо Moq-подобных библиотек, если зависимость легко заменить в тесте.

## Раскладка проектов

Unit-тесты лежат в отдельном тестовом проекте рядом с именем production-проекта:

- `tests/SharedKernel.UnitTests`
- `tests/Domain.UnitTests`
- `tests/Application.UnitTests`
- `tests/Infrastructure.UnitTests`
- `tests/Web.Api.UnitTests`
- `tests/IntegrationContracts.UnitTests`
- `tests/GameRuntime.Common.UnitTests`
- `tests/GameRuntime.UnitTests`
- `tests/Chronicles.Domain.UnitTests`
- `tests/Chronicles.Application.UnitTests`
- `tests/Chronicles.Infrastructure.UnitTests`

Solution-level архитектурные правила находятся в `tests/ArchitectureTests`.

Host-проекты (`Aspire.AppHost`, workers, migrator) покрываются через тесты нижележащей логики и architecture-тесты. Для сценариев с реальной БД нужен отдельный будущий слой integration-тестов.

## Локальная проверка

```powershell
dotnet restore ByteFight.sln
dotnet build ByteFight.sln --no-restore

$testProjects = @(
  "tests\SharedKernel.UnitTests\SharedKernel.UnitTests.csproj",
  "tests\Domain.UnitTests\Domain.UnitTests.csproj",
  "tests\Application.UnitTests\Application.UnitTests.csproj",
  "tests\Infrastructure.UnitTests\Infrastructure.UnitTests.csproj",
  "tests\Web.Api.UnitTests\Web.Api.UnitTests.csproj",
  "tests\IntegrationContracts.UnitTests\IntegrationContracts.UnitTests.csproj",
  "tests\GameRuntime.Common.UnitTests\GameRuntime.Common.UnitTests.csproj",
  "tests\GameRuntime.UnitTests\GameRuntime.UnitTests.csproj",
  "tests\Chronicles.Domain.UnitTests\Chronicles.Domain.UnitTests.csproj",
  "tests\Chronicles.Application.UnitTests\Chronicles.Application.UnitTests.csproj",
  "tests\Chronicles.Infrastructure.UnitTests\Chronicles.Infrastructure.UnitTests.csproj",
  "tests\ArchitectureTests\ArchitectureTests.csproj"
)

foreach ($project in $testProjects) {
  dotnet test $project --no-build
}
```

`dotnet test ByteFight.sln` допустим, но в локальной среде он оценивает все проекты solution, включая `Aspire.AppHost`, и может требовать установленный Aspire SDK/workload и доступ к NuGet даже при `--no-build`.

## Что считать критичной логикой

- доменные инварианты и value objects;
- авторизация, permissions и security-sensitive код;
- правила runtime-боя: движение, урон, лечение, завершение сессии;
- компиляция и ограничения пользовательского кода;
- integration contracts и JSON polymorphism;
- projection pipeline и nomination projectors в `Chronicles`;
- архитектурные границы между слоями и bounded context.
