# Заметки для кодинг-агентов

Этот файл фиксирует короткие рабочие правила, которые помогают безопасно дорабатывать проект автоматизированными агентами.

## Быстрые проверки

```powershell
dotnet build ByteFight.sln --no-restore
dotnet test tests\SharedKernel.UnitTests\SharedKernel.UnitTests.csproj --no-build
dotnet test tests\Domain.UnitTests\Domain.UnitTests.csproj --no-build
dotnet test tests\Application.UnitTests\Application.UnitTests.csproj --no-build
dotnet test tests\Infrastructure.UnitTests\Infrastructure.UnitTests.csproj --no-build
dotnet test tests\Web.Api.UnitTests\Web.Api.UnitTests.csproj --no-build
dotnet test tests\IntegrationContracts.UnitTests\IntegrationContracts.UnitTests.csproj --no-build
dotnet test tests\GameRuntime.Common.UnitTests\GameRuntime.Common.UnitTests.csproj --no-build
dotnet test tests\GameRuntime.UnitTests\GameRuntime.UnitTests.csproj --no-build
dotnet test tests\Chronicles.Domain.UnitTests\Chronicles.Domain.UnitTests.csproj --no-build
dotnet test tests\Chronicles.Application.UnitTests\Chronicles.Application.UnitTests.csproj --no-build
dotnet test tests\Chronicles.Infrastructure.UnitTests\Chronicles.Infrastructure.UnitTests.csproj --no-build
dotnet test tests\ArchitectureTests\ArchitectureTests.csproj --no-build

cd src/ClientApp
pnpm.cmd lint
pnpm.cmd lint:strict
pnpm.cmd format
pnpm.cmd build
```

Полный список test projects и PowerShell-цикл лежат в `docs/testing.md`.
`dotnet test ByteFight.sln` может дополнительно оценивать `Aspire.AppHost` и требовать установленный Aspire SDK/workload. Если нужно проверить только тесты, запускайте тестовые проекты напрямую.

## Тесты

- Для новой бизнес-логики предпочитайте отдельный test project с именем production-проекта: `tests/<ProjectName>.UnitTests`.
- Используйте `xUnit`, `Shouldly` и ручные fakes/stubs; не добавляйте Moq/FluentAssertions без отдельного ADR.
- Architecture-ограничения добавляйте в `tests/ArchitectureTests`, если изменение касается границы слоя, bounded context или transport contract.

## Chronicles

- `Chronicles.Application` не должен зависеть от `IntegrationContracts`: transport payload разбирается в `Chronicles.Infrastructure`.
- Новые номинации добавляются через стратегию `IChronicleNominationProjector`, а не через условную логику в reader'ах.
- Read-model пересобирается только явно через `--rebuild-chronicles` или `Migrator:RebuildChroniclesProjections=true`.
- Обычный startup migrator должен выполнять инкрементальную догонку через `Outbox -> Inbox -> Projection`, а не полный пересчёт.
- При изменении кодов номинаций нужно синхронно проверить миграции, frontend visual presets и unit-тесты.

## Документация

- Глобальные ADR лежат в `docs/adr`.
- ADR, относящиеся только к `Chronicles`, лежат в `src/Chronicles/docs/adr`.
- В публичные классы, интерфейсы и важные public members добавляйте русскоязычные XML summary, если они помогают будущему сопровождению.
