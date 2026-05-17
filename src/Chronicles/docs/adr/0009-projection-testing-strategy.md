# ADR 0009: Тестирование проекций и номинаций Chronicles

## Статус

Принято

## Контекст

`Chronicles` строит read-model из завершённых игровых сессий. Ошибка в этой зоне может:

- исказить Зал славы;
- сломать первичную догонку старых сессий;
- привести к повторной обработке inbox-сообщений;
- нарушить независимость `Chronicles.Application` от транспортного контракта.

При этом текущая production-модель намеренно простая: один экземпляр worker, `Outbox -> Inbox -> Projection`, без распределённых lease/claim механизмов.

## Решение

Быстрыми unit-тестами покрываются:

- `tests/Chronicles.Domain.UnitTests` — доменные методы `CharacterChronicleStats` и `CharacterNominationScore`;
- `tests/Chronicles.Application.UnitTests` — стратегии `IChronicleNominationProjector`;
- `tests/Chronicles.Infrastructure.UnitTests` — преобразование `GameSessionCompletedIntegrationEvent` в `CompletedGameSessionData`;
- `tests/IntegrationContracts.UnitTests` — JSON-настройки интеграционных событий с polymorphic log entries.

Architecture-тестами фиксируется:

- `Chronicles.Application` не зависит от `IntegrationContracts`;
- `Chronicles.Infrastructure` остаётся единственным местом, где transport payload превращается в application data;
- `Chronicles.Domain` не знает про EF Core и внешние слои.

Тесты для projectors обращаются к internal типам через `InternalsVisibleTo` на `Chronicles.Application.UnitTests`.
Это осознанный компромисс: сами стратегии остаются скрытыми от production API, но их правила можно проверять напрямую без БД и worker host.

## Последствия

### Плюсы

- новые номинации можно добавлять через отдельную стратегию и тест на неё;
- parser интеграционного события проверяется отдельно от EF/inbox;
- архитектурная граница между transport contract и application model защищена тестом.

### Минусы

- unit-тесты не заменяют будущие integration-тесты миграций и реального PostgreSQL;
- при изменении набора номинаций нужно обновлять тесты стратегий и визуальные пресеты фронта.
