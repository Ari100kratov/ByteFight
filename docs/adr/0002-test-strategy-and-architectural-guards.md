# ADR 0002: Стратегия тестирования и архитектурные ограничения

## Статус

Принято

## Контекст

В проекте есть несколько зон, где регрессия дорого стоит:

- расчёт результатов боя и производных read-model;
- integration contracts между `GameRuntime` и `Chronicles`;
- авторизация доступа к пользовательским данным;
- безопасность компиляции и выполнения пользовательского кода;
- границы слоёв `SharedKernel`, `Domain`, `Application`, `Infrastructure`, `Web.Api`;
- границы `GameRuntime` и bounded context `Chronicles`.

Нужны тесты, которые ловят не только ошибки в конкретных значениях, но и случайное нарушение архитектурных зависимостей.

## Решение

Используем проектную раскладку unit-тестов:

- `tests/<Project>.UnitTests` — быстрые unit-тесты логики конкретного production-проекта.
- `tests/ArchitectureTests` — solution-level правила зависимостей между слоями и bounded context.

Тестовый стек:

- `xUnit` для тестов;
- `Shouldly` для assertion'ов вместо FluentAssertions;
- `NetArchTest.Rules` для architecture-тестов;
- ручные fakes/stubs вместо Moq-подобных библиотек, если зависимость легко заменить в тесте.

Критические unit-тесты покрывают:

- доменные методы и value objects, которые поддерживают инварианты;
- validation decorators, permissions и security-sensitive инфраструктуру;
- правила движения, урона, лечения, завершения боя и turn loop;
- компиляцию пользовательского кода и запрет опасных API;
- JSON-сериализацию и десериализацию интеграционных событий;
- расчёт nominations/read-model в `Chronicles`.

Критические architecture-тесты фиксируют:

- `SharedKernel` не зависит от feature-assemblies;
- `Domain` не зависит от application/infrastructure/presentation;
- `Application` не зависит от infrastructure/presentation/runtime/Chronicles/transport contracts;
- `GameRuntime.Common` и user-code API не зависят от runtime hosts, infrastructure и Roslyn;
- `GameRuntime` не зависит от `Web.Api`, `Infrastructure` и `Chronicles`;
- `Chronicles.Application` не зависит от `IntegrationContracts`;
- `IntegrationContracts` остаётся независимым пакетом контрактов.

Команды и список test projects вынесены в `docs/testing.md`.

`dotnet test ByteFight.sln` допустим, но может требовать корректно установленный Aspire SDK/workload и доступ к NuGet из-за оценки `Aspire.AppHost`. Для проверки только тестов нужно запускать test projects напрямую.

## Последствия

### Плюсы

- регрессии в боевой логике, хрониках и контрактах ловятся без поднятия БД;
- тесты проще искать: имя test project совпадает с production project;
- архитектурные тесты не дают случайно протащить зависимости в неправильный слой;
- coding agent и разработчик получают одинаковый набор команд проверки.

### Минусы

- для тестирования internal-стратегий используются friend assemblies через `InternalsVisibleTo`;
- список test projects длиннее, чем один монолитный тестовый проект;
- сценарии с реальной БД и миграциями всё ещё требуют отдельного слоя integration-тестов в будущем.
