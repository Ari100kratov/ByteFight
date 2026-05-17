# ADR проекта

Этот каталог содержит архитектурные решения, которые затрагивают весь проект, а не один
bounded context.

- `0001-production-hosting-topology.md` - production-топология, порядок запуска
  `migrator`, `web-api`, `chronicles-worker` и инфраструктурных сервисов.
- `0002-test-strategy-and-architectural-guards.md` - стратегия unit/architecture тестов и
  архитектурные ограничения solution-level.
- `0003-frontend-quality-and-bundle-splitting.md` - контроль качества фронтенда, правила
  форматирования/линтинга и разбиение Vite-бандла.

ADR, относящиеся только к `Chronicles`, лежат в `src/Chronicles/docs/adr`.
