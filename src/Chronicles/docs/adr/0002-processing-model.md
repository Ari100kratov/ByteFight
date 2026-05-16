# ADR 0002: Production-модель обработки хроник

## Статус

Принято

## Контекст

`Chronicles` строит историческую витрину поверх завершённых игровых сессий, но не должен:

- читать весь исторический объём при каждом цикле;
- зависеть от структуры доменной БД `GameRuntime`;
- блокировать завершение боя;
- обрабатывать клиентский HTTP API как внутренний integration channel.

Ранее рассматривались pull-модели через прямое чтение завершённых `GameSession`, но для production-потока они создают лишнюю связность и хуже масштабируются.

## Решение

Основной production-поток строится так:

1. `GameRuntime` завершает игровую сессию.
2. В той же транзакции пишет `GameSessionCompletedIntegrationEvent` в `integration.outbox_messages`.
3. `Chronicles.Worker` читает новые outbox-сообщения по локальному cursor и импортирует их в `chronicles.inbox_messages`.
4. `Chronicles.Worker` обрабатывает непрочитанные inbox-сообщения и обновляет:
   - `CharacterChronicleStats`
   - `ChronicleRecords`
   - справочники и read-model витрины

Внутри `Chronicles` поток разделён на два use-case:

- `IChroniclesInboxImportService`
- `IChroniclesProjectionService`

Проекция работает только по локальному inbox и не зависит напрямую от транспорта доставки.
Тот же pipeline используется migrator'ом для первичной догонки: migrator не пишет read-model напрямую, а импортирует и обрабатывает недостающие события теми же application-сервисами.

Для extensibility расчёт номинаций вынесен в стратегии `IChronicleNominationProjector`.
Проекторы больше не заставляют reader знать правила каждой номинации: они обновляют универсальную read-model `CharacterNominationScores`, а `ChronicleRecords` остаются историей конкретных достижений по сессиям.
Лидерборды читают `ChronicleNominations` + `CharacterNominationScores` и сортируют результат по metadata номинации.

На текущем этапе `Chronicles.Worker` запускается в одном экземпляре. Поэтому pipeline намеренно не усложняется lease/claim-механикой для конкурирующих consumer-ов.
Если появится необходимость масштабировать worker горизонтально, следующим шагом должен стать безопасный claim inbox-сообщений через Postgres locking или lease columns.

## Почему не checkpoint в production-потоке

Checkpoint-курсоры удобны для pull/scan сценариев, но они не являются основной моделью для event-driven integration.
В production-потоке текущая единица идемпотентности и прогресса:

- `OutboxMessage.Id` на стороне источника
- `InboxMessage.Id` на стороне потребителя
- `ProcessedAtUtc` в inbox как признак успешной локальной обработки
- `chronicles.import_cursors` как локальный прогресс чтения внешнего outbox

Дополнительный ledger `chronicles.completed_sessions` не вводится: для текущей рабочей версии source of truth остаётся в `GameRuntime`, а `Chronicles` хранит только inbox и read-модели.

## Последствия

### Плюсы

- завершение боя не ждёт расчёта хроник;
- `Chronicles` больше не читает напрямую `GameSession` как production source;
- transport и projection разделены;
- inbox позволяет безопасно повторять обработку и хранить ошибки локально;
- новые номинации добавляются без переписывания центрального пайплайна.
- первичная обработка старых сессий не требует постоянного полного пересбора read-model.

### Минусы

- появляется дополнительная инфраструктура `Outbox/Inbox`;
- нужно поддерживать importer и transport-границу;
- backfill больше не может быть просто частью production worker и требует отдельного сценария.

## Что дальше

- добавить отдельный backfill/export механизм поверх integration contract;
- при необходимости заменить текущий importer на message broker consumer, не меняя projection-слой.
