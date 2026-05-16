# ADR 0001: Production-топология хостинга

## Статус

Принято

## Контекст

Проект больше не является одним Web API с базой данных. В runtime участвуют:

- `web-api` для HTTP, SignalR и запуска игровых сессий;
- `chronicles-worker` для асинхронного обновления Зала славы и хроник;
- `migrator` для EF migrations, seed, outbox-backfill и первичной догонки Chronicles;
- PostgreSQL как основное хранилище;
- pgAdmin как административный UI для PostgreSQL;
- MinIO как S3-совместимое хранилище ассетов;
- клиентский Nginx-контейнер для SPA и reverse proxy.

Если запускать API или worker до применения миграций, сервисы могут стартовать успешно, но падать на первых запросах или polling-циклах из-за отсутствующих таблиц.

## Решение

В production используется явный порядок запуска:

1. Поднять PostgreSQL и MinIO.
2. Запустить `migrator` как one-shot процесс.
3. Дождаться успешного завершения `migrator`.
4. Запустить `web-api` и `chronicles-worker`.
5. Запустить клиентский контейнер.
6. Поднять pgAdmin для ручного администрирования БД.

`web-api` и `chronicles-worker` не применяют миграции и не seed'ят данные на старте. Это снижает права runtime-сервисов и убирает гонки между несколькими процессами.
Если БД Chronicles отсутствует, ее создает `migrator` через EF Core migrations. Поэтому учетная запись PostgreSQL, используемая migrator, должна иметь право создавать БД.

В Docker Compose/Portainer этот порядок выражен через:

- one-shot контейнер `migrator`;
- `depends_on` с `service_completed_successfully` для `web-api` и `chronicles-worker`.
- отдельный volume `pgadmin-data` для пользовательских настроек pgAdmin.

## Последствия

- Ошибка миграции блокирует старт runtime-сервисов и становится видимой на этапе деплоя.
- Повторный запуск stack безопасен: migrator идемпотентно применяет migrations, seed и catch-up.
- Для штатного деплоя `REBUILD_CHRONICLES_PROJECTIONS=false`; полный пересбор Chronicles включается только вручную.
- В мониторинге нужно отдельно отслеживать `migrator` и `chronicles-worker`, потому что они отвечают за готовность схемы и актуальность витрины.
