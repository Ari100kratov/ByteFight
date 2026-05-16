# ADR 0007: Отдельный Migrator и первичная догонка завершённых сессий

## Статус

Принято

## Контекст

`Web.Api` и `Chronicles.Worker` не должны отвечать за подготовку схемы данных во время runtime-старта.
Иначе появляются гонки:

- API может стартовать раньше схемы `Chronicles`;
- worker может начать polling до создания таблиц;
- runtime-сервисы вынуждены иметь DDL-права.

Также при первом запуске `Chronicles` нужно обработать уже завершённые игровые сессии, которые появились до включения outbox-публикации.

## Решение

Вводится отдельный host `Migrator`.

Он выполняет последовательно:

1. миграции `AuthDbContext`;
2. миграции `GameDbContext`;
3. миграции `GameRuntimeDbContext`;
4. миграции `ChroniclesDbContext`;
5. seed базовых данных;
6. first-run backfill завершённых `GameSession` в `integration.outbox_messages`;
7. догонку `Chronicles` через обычный pipeline `Outbox -> Inbox -> Projection`;
8. опциональный ручной пересбор read-model `Chronicles`;
9. дозаполнение отсутствующих отображаемых данных участников в read-model номинаций.

Backfill не пишет напрямую в таблицы `Chronicles`.
Он создаёт недостающие `GameSessionCompletedIntegrationEvent` в outbox `GameRuntime`.
После этого migrator запускает тот же application pipeline, что и worker: импортирует новые outbox-сообщения в локальный inbox и обрабатывает необработанные inbox-сообщения.

Catch-up идемпотентен:

- повторный запуск не создаёт outbox-дубликаты, потому что проверяется `AggregateId + Type`;
- импорт не создаёт inbox-дубликаты, потому что использует `OutboxMessage.Id`;
- projection не трогает уже обработанные inbox-сообщения.

Reprojection в migrator не выполняется каждый раз.
Полный пересбор read-model нужен только для редких случаев: изменение алгоритма номинаций, исправление багов проекции или пересчёт после ручного восстановления данных.
Он запускается явно через `Migrator:RebuildChroniclesProjections=true` или CLI-флаг `--rebuild-chronicles`.
Во время полного пересбора каждая обработанная inbox payload сохраняется отдельно перед переходом к следующей сессии.
Это нужно, потому что projection-processor перечитывает текущие score/stats из БД; следующий payload должен видеть записи, созданные предыдущим payload, иначе возможны дубликаты по уникальным ключам.

Metadata-backfill нужен для переходных версий, где старые inbox payload могли ещё не содержать имя/фамилию пользователя, класс и специализацию персонажа.
Он обновляет только строки с отсутствующими отображаемыми данными, не меняет результат боя и не создаёт дополнительные integration events.

Критерий backfill: завершённая сессия `GameRuntime` считается необработанной, если для неё нет `integration.outbox_messages` с `AggregateId = GameSession.Id` и типом `GameSessionCompletedIntegrationEvent`.

В Aspire `Web.Api` и `Chronicles.Worker` ждут успешного завершения `Migrator`.
В Docker Compose/Portainer это выражено тем же контрактом: `migrator` запускается как one-shot контейнер, а `web-api` и `chronicles-worker` зависят от его `service_completed_successfully`.
Если хостинг не поддерживает такой dependency condition, порядок нужно обеспечить внешним release pipeline: сначала выполнить migrator, затем запускать runtime-сервисы.

## Последствия

### Плюсы

- схема готова до старта runtime-сервисов;
- backfill использует тот же integration contract, что и production поток;
- `Chronicles.Worker` остаётся простым single-instance processor;
- `Web.Api` больше не выполняет DDL на старте.

### Минусы

- появляется отдельный deploy/startup step;
- first-run backfill зависит от доступности `GameRuntime` и `Game` схем;
- при больших исторических объёмах backfill может потребовать отдельного batch/monitoring сценария.
