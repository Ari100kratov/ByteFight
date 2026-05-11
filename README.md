# ByteFight

<p align="center">
  <a href="https://github.com/Ari100kratov/ByteFight/actions/workflows/build.yml">
    <img alt="Build" src="https://github.com/Ari100kratov/ByteFight/actions/workflows/build.yml/badge.svg" />
  </a>
  <a href="https://github.com/Ari100kratov/ByteFight/blob/main/LICENSE">
    <img alt="License" src="https://img.shields.io/github/license/Ari100kratov/ByteFight?style=flat-square" />
  </a>
  <a href="https://github.com/Ari100kratov/ByteFight/commits/main">
    <img alt="Last commit" src="https://img.shields.io/github/last-commit/Ari100kratov/ByteFight/main?style=flat-square" />
  </a>
  <a href="https://github.com/Ari100kratov/ByteFight/issues">
    <img alt="Issues" src="https://img.shields.io/github/issues/Ari100kratov/ByteFight?style=flat-square" />
  </a>
  <a href="https://github.com/Ari100kratov/ByteFight/pulls">
    <img alt="PRs" src="https://img.shields.io/github/issues-pr/Ari100kratov/ByteFight?style=flat-square" />
  </a>
  <a href="https://github.com/Ari100kratov/ByteFight/stargazers">
    <img alt="Stars" src="https://img.shields.io/github/stars/Ari100kratov/ByteFight?style=flat-square" />
  </a>
</p>

## 📚 Содержание
- [О проекте](#-о-проекте)
- [Суть проекта и ключевые особенности](#-суть-проекта-и-ключевые-особенности)
- [Технологии и архитектура](#-технологии-и-архитектура)
- [Локальный запуск](#-локальный-запуск-windows)
- [Подготовка к production и Portainer](#-подготовка-к-production-и-portainer)
- [Контрибьютинг и обратная связь](#-контрибьютинг-и-обратная-связь)
- [Планы развития](#-планы-развития)
- [Лицензия](#-лицензия)

---

## 📌 О проекте

**ByteFight** — это образовательный проект, в котором пользователь программирует поведение игрового персонажа, запускает бой и наблюдает его развитие в реальном времени.

Проект выступает как полноценная площадка для практики разработки интерактивных систем и игровых механик:
- проектирование доменной модели и игрового цикла.
- разработка серверной и клиентской части.
- организация обмена событиями в реальном времени.
- проектирование и реализация Intellisense для работы с кодом.
- безопасная компиляция и выполнение пользовательского кода.
- проектирование и реализация игровой логики (правила боя, поведение юнитов, взаимодействие объектов).
- работа с визуальной частью: спрайты, анимации, игровые ассеты.
- построение клиентского рендера сцены (арена, персонажи, эффекты).

---

## 🎯 Суть проекта и ключевые особенности

### В чем основная идея

**ByteFight** — это программируемая игровая арена, в которой поведение персонажа полностью определяется кодом пользователя:
1. Пользователь регистрируется и создает персонажа.
2. Выбирает игровой режим и арену.
3. Описывает поведение персонажа с помощью кода.
4. Запускает игровую сессию.
5. Наблюдает пошаговое развитие боя и его результат в реальном времени.

### Текущие возможности

#### Игровая логика
- Поддержка режимов: `Тренировка`, `PvE`
- Пошаговый игровой цикл
- Поведение игрока определяется пользовательским кодом
- Встроенный AI для противников на арене

#### Работа с сессиями
- Запуск и завершение игровых сессий
- Фиксация результатов
- Логирование каждого хода
- Просмотр истории боев

#### События в реальном времени
- Подключение к сессии
- Поток игровых событий (тики)
- Событие завершения боя

#### Редактор кода
- Построен на базе [Monaco Editor](https://github.com/microsoft/monaco-editor)
- Возможность работы с несколькими версиями кода для персонажа
- Стартовые шаблоны

#### Подсказки при программировании
- Проверка кода (diagnostics)
- Автодополнение (completions)
- Подсказки по типам и методам (hover)
- Подсказки по сигнатурам методов (signature help)

#### Безопасность выполнения
- Ограничение доступа к небезопасным API
- Изолированное выполнение пользовательского кода
- Ограничения на длительность выполнения и параллельный запуск нескольких боев

#### Отображение игры
- Отрисовка арены и персонажей
- Анимации действий
- Журнал боя
- Отображение результата
  
---

## 🧱 Технологии и архитектура

## Backend / Platform (.NET)

### Основной стек
- .NET 10
- ASP.NET Core (Minimal API)
- OpenAPI + Scalar

### Данные и инфраструктура
- Entity Framework Core
- PostgreSQL
- Разделение контекстов данных
- S3-совместимое хранилище (MinIO)

### Авторизация
- JWT Bearer Authentication
- Refresh Tokens
- Permission-based authorization

### Игровой runtime
- SignalR для передачи событий
- Roslyn для компиляции и анализа пользовательского кода 
- Изолированный исполнитель пользовательского кода

### Observability
- Aspire.ServiceDefaults
- HealthChecks
- OpenTelemetry

### Локальная инфраструктура
- Оркестрация сервисов через .NET Aspire

### Архитектурные подходы
- Слоистая архитектура с разделением на `Domain`, `Application`, `Infrastructure`, `Web.Api`, `GameRuntime`.
- Подход близок к **Clean Architecture**:
  - доменная модель изолирована и не зависит от инфраструктуры,
  - внешние зависимости подключаются через слой `Infrastructure`.
- В слое `Application` используется упрощённый **CQRS-подход**:
  - разделение команд и запросов (`ICommandHandler`, `IQueryHandler`),
  - применение декораторов для валидации и логирования.
- `Web.Api` выступает как тонкий слой доставки (HTTP) без бизнес-логики.
- Выделен отдельный контур выполнения — `GameRuntime`:
  - реализует игровой цикл,
  - выполняет пользовательский код (изолированно от основного процесса),
  - содержит API и IntelliSense для пользовательского кода

---

## Frontend (React + Vite)

### Библиотеки (основные)
- React 19 + TypeScript
- Vite
- react-router-dom
- tanstack/react-query
- zustand
- microsoft/signalr
- monaco-editor + @monaco-editor/react
- pixi.js + @pixi/react
- Tailwind CSS 4 + Radix UI + shadcn/ui-паттерны.

### Архитектурные подходы
- Структура построена по принципу **Feature-Sliced (feature-oriented)**:
  - код разделён по функциональным модулям, а не по техническим слоям.
- Разделение состояния:
  - **server-state** — управление и кеширование данных через TanStack Query,
  - **client-state** — локальное состояние интерфейса через Zustand.
- Используется **event-driven подход**:
  - состояние игры обновляется через поток событий в реальном времени.
- Игровой рендер вынесен в отдельный слой:
  - используется **Pixi.js (WebGL с fallback на Canvas)**,
  - реализована 2D-отрисовка сцены, спрайтов и анимаций.
- UI и игровой рендер разделены:
  - интерфейс управляет состоянием,
  - рендер отвечает за визуализацию.

---

## Архитектурный срез репозитория

```text
src/
  Domain/                # бизнес-сущности и правила
  Application/           # команды/запросы
  Infrastructure/        # EF Core, auth, MinIO, policy provider
  GameRuntime/           # игровой цикл, AI, user code compilation/execution, realtime
  Web.Api/               # HTTP endpoint'ы, DI, middleware
  ClientApp/             # React SPA
  Aspire.AppHost/        # оркестрация локального окружения
```
---

## 🚀 Локальный запуск (Windows)

### Требования

- Visual Studio с поддержкой .NET и Aspire
- Docker Desktop
- .NET SDK 10
- Node.js LTS 22+
- pnpm

### 1. Установка зависимостей

- .NET SDK: https://dotnet.microsoft.com/download
- Visual Studio: https://visualstudio.microsoft.com/
- Docker Desktop: https://www.docker.com/products/docker-desktop/
- Node.js: https://nodejs.org/
- pnpm: https://pnpm.io/installation
- .NET Aspire: https://learn.microsoft.com/dotnet/aspire/


### 2. Важно знать перед запуском

При первом запуске приложения, если база данных отсутствует, она будет автоматически создана. Все необходимые начальные данные (seed) также будут добавлены.

#### Учетные данные администратора по умолчанию

* **Email:** admin@bytefight.ru
* **Пароль:** admin123

После входа можно изменить учетные данные администратора через веб-интерфейс приложения.


### 3. Клонирование репозитория

```powershell
git clone https://github.com/Ari100kratov/ByteFight.git
cd ByteFight
```


### 4. Установка зависимостей клиента

```powershell
cd src/ClientApp
pnpm install
cd ../..
```


### 5. Запуск серверной части через Aspire

#### Вариант A — через Visual Studio/Rider

1. Откройте `ByteFight.sln`
2. Выберите стартовый проект `Aspire.AppHost`
3. Запустите проект (`F5` или `Ctrl+F5`)

#### Вариант B — через консоль

```powershell
dotnet run --project src/Aspire.AppHost/Aspire.AppHost.csproj
```

Aspire автоматически поднимет необходимые сервисы: базу данных, объектное хранилище и Web API.

Вот более понятный и аккуратно оформленный вариант с пояснениями и `note`:


### 6. ⚠️ ВАЖНО! Перед запуском клиента

Перед запуском клиентского приложения необходимо подготовить файловое хранилище **MinIO**.

#### Шаги:

1. Скачайте ассеты по ссылке:
   [https://disk.yandex.ru/d/-kMKjfv1s9MeTg](https://disk.yandex.ru/d/-kMKjfv1s9MeTg)

2. Откройте веб-интерфейс MinIO:
   [http://localhost:9000](http://localhost:9000)

3. Войдите в систему, используя учетные данные по умолчанию:

   * **Логин:** admin
   * **Пароль:** password123

4. В MinIO необходимо:

   * вручную создать **бакеты** (bucket = корневая директория);
   * перенести каталоги и файлы из архива напрямую в соответствующие бакеты (MinIO поддерживает загрузку папок целиком).

📌 **Как устроено хранилище:**

* Бакеты — это верхний уровень (аналог корневых папок);
* Внутри бакетов размещаются папки и файлы;
* Структура должна полностью совпадать с той, что находится в архиве.

> 💡 **Note:**
> Aspire автоматически поднимает MinIO с дефолтными учетными данными, однако **ассеты не загружаются автоматически**.
> Если пропустить этот шаг или нарушить структуру файлов, клиентское приложение не сможет корректно отображать ресурсы (изображения, медиа и т.д.).


### 7. Запуск клиента

```powershell
cd src/ClientApp
pnpm dev
```

### Адреса по умолчанию

- Клиент: `http://localhost:5173`
- API: `http://localhost:5000`
- Хранилище MinIO: `http://localhost:9000`
- Консоль MinIO: `http://localhost:9001`

### Проверка сборки и тестов

```powershell
dotnet restore ByteFight.sln
dotnet build ByteFight.sln
dotnet test ByteFight.sln

cd src/ClientApp
pnpm build
```

### Возможные проблемы

- Убедитесь, что Docker Desktop запущен
- Проверьте, что порты `5000`, `5173`, `5432`, `9000`, `9001` свободны
- Проверьте установленную версию .NET командой `dotnet --info`
- При ошибках зависимостей клиента удалите `node_modules` и выполните `pnpm install` повторно
- При проблемах с запуском Aspire проверьте, что установлены необходимые компоненты Visual Studio и актуальная версия .NET SDK

---


## 🚢 Подготовка к production и Portainer

### Что нужно сделать сначала

1. **Определить публичные адреса**: домен клиента, домен/API-путь, способ доступа к MinIO Console и Aspire Dashboard. Для текущего `docker-compose.yml` клиент работает как основной вход, а запросы к API идут через `/api`.
2. **Подготовить секреты**: скопировать `.env.example` в `.env` и заменить все `change-me-*` значения. Минимально обязательны `POSTGRES_PASSWORD`, `MINIO_ROOT_USER`, `MINIO_ROOT_PASSWORD`, `JWT_SECRET`.
3. **Решить вопрос TLS**: в production ставьте обратный прокси перед Portainer stack (Traefik, Nginx Proxy Manager, Caddy или внешний балансировщик) и публикуйте наружу только нужные HTTP(S)-точки.
4. **Загрузить ассеты в MinIO**: после первого запуска создать/проверить bucket `assets` и загрузить файлы из архива ассетов с сохранением структуры.
5. **Проверить миграции и seed**: первый запуск можно делать с `APPLY_MIGRATIONS=true` и `SEED_ON_STARTUP=true`; после успешного seed обычно стоит выставить `SEED_ON_STARTUP=false`.
6. **Ограничить доступ к диагностике**: Aspire Dashboard показывает логи, traces, метрики и потенциально чувствительные данные. Не публикуйте его без авторизации/VPN/IP allowlist.

### Состав production stack

В корне репозитория добавлены файлы для Portainer/Docker Compose:

- `docker-compose.yml` — stack из PostgreSQL, MinIO, Web API, React/Nginx клиента и standalone Aspire Dashboard.
- `.env.example` — шаблон переменных окружения для Portainer stack.
- `.dockerignore` — исключает `bin`, `obj`, `node_modules`, `.env` и локальные контейнерные данные из Docker build context.
- `src/Web.Api/Dockerfile` — production-сборка API на .NET 10 с публикацией `user-code-worker`.
- `src/ClientApp/Dockerfile` и `src/ClientApp/nginx.conf` — production-сборка SPA и reverse proxy для `/api` и `/game-runtime-hub`.

### Быстрый запуск локально через Docker Compose

```bash
cp .env.example .env
# Отредактируйте .env: задайте надежные POSTGRES_PASSWORD, MINIO_ROOT_PASSWORD и JWT_SECRET.
docker compose up -d --build
```

Адреса по умолчанию:

- Клиент: `http://localhost`
- API напрямую: `http://localhost:5000`
- MinIO API: `http://localhost:9000`
- MinIO Console: `http://localhost:9001`
- Aspire Dashboard: `http://localhost:18888`

### Развертывание в Portainer

1. Откройте **Stacks → Add stack**.
2. Выберите репозиторий Git или вставьте содержимое `docker-compose.yml`.
3. В секции **Environment variables** перенесите значения из `.env.example` и замените секреты.
4. Нажмите **Deploy the stack**.
5. После запуска проверьте health endpoint API: `http://<host>:5000/health`.
6. Зайдите в MinIO Console и загрузите ассеты в bucket `assets`.

### Aspire Dashboard в production

В stack используется standalone dashboard image `mcr.microsoft.com/dotnet/aspire-dashboard`. Web API отправляет телеметрию через OTLP/gRPC:

```text
OTEL_SERVICE_NAME=bytefight-web-api
OTEL_RESOURCE_ATTRIBUTES=service.namespace=bytefight,deployment.environment=production
OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889
OTEL_EXPORTER_OTLP_PROTOCOL=grpc
```

Dashboard UI доступен на порту `ASPIRE_DASHBOARD_PORT` (по умолчанию `18888`). По умолчанию `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=false`, поэтому токен входа нужно взять из логов контейнера `aspire-dashboard` в Portainer. Для локальной разработки можно временно поставить `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true`, но для публичного production так делать нельзя.

В compose dashboard явно слушает `0.0.0.0:18888`, `0.0.0.0:18889` и `0.0.0.0:18890`, чтобы OTLP был доступен не только внутри самого контейнера dashboard, но и из контейнера `web-api` по DNS-имени `aspire-dashboard`.

Если в dashboard не появляются метрики/логи/traces:

1. Проверьте, что в `web-api` реально есть переменные `OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889` и `OTEL_EXPORTER_OTLP_PROTOCOL=grpc`.
2. Проверьте логи `web-api`: ошибки вида `Unavailable`, `connection refused`, `Name or service not known` указывают на проблему DNS/порта/доступности dashboard.
3. Проверьте логи `aspire-dashboard`: должен быть endpoint OTLP/gRPC на `18889`; при включенной авторизации frontend токен входа также будет в этих логах.
4. Сделайте несколько HTTP-запросов в API и подождите до минуты: метрики экспортируются периодически, а не синхронно с каждым запросом.
5. В standalone dashboard список Aspire resources может быть пустым без resource service, но telemetry pages должны показывать сервис `bytefight-web-api`.

> Важно: standalone Aspire Dashboard хранит телеметрию в памяти и предназначен для разработки/краткосрочной диагностики. Для долгосрочного production-monitoring дополнительно планируйте постоянное хранилище логов/метрик (например, Grafana stack, Seq, Azure Monitor и т.п.).

### Production-настройки приложения

- `Database:ApplyMigrations` и `Database:SeedOnStartup` теперь можно включать через переменные `Database__ApplyMigrations` и `Database__SeedOnStartup` без перевода приложения в `Development`.
- CORS настраивается через `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1` и т.д. При размещении клиента и API за одним Nginx (`/api`) CORS почти не используется, но настройка оставлена для отдельных доменов.
- `src/ClientApp/.env.production` использует `VITE_API_URL=/api` и `VITE_GAME_HUB_URL=/game-runtime-hub`; Nginx в клиентском контейнере проксирует `/api/*` в Web API с удалением префикса `/api`, а SignalR идет через отдельный WebSocket location.
- Значения `CLIENT_API_URL` и `CLIENT_GAME_HUB_URL` попадают в Vite на этапе сборки клиентского Docker image. Если меняете публичную схему маршрутизации, пересоберите контейнер клиента.

### Что еще нужно перед реальным публичным запуском

- Заменить все дефолтные пароли и `JWT_SECRET` на секреты из password manager/Portainer secrets.
- Настроить TLS и безопасные cookies/headers на внешнем reverse proxy.
- Закрыть прямые порты PostgreSQL и MinIO API снаружи, если они не нужны публично.
- Настроить backup volumes `postgres-data` и `minio-data`.
- Проверить политику выполнения пользовательского кода и лимиты ресурсов контейнера `web-api`.
- Прогнать `dotnet test ByteFight.sln` и `pnpm build` перед публикацией образов.

---

## 🤝 Контрибьютинг и обратная связь

Вклад в проект приветствуется.

Вы можете работать двумя способами:

### Вариант 1 — через fork (рекомендуется для внешних участников)

1. Сделайте fork репозитория  
2. Создайте отдельную ветку  
3. Внесите изменения  
4. Откройте Pull Request  

### Вариант 2 — напрямую (для доступа - напишите мне)

Если у вас есть права на запись в репозиторий, можно:

1. Создать ветку в основном репозитории  
2. Сделать изменения  
3. Открыть Pull Request  

### Рекомендации

- Делайте небольшие и атомарные изменения  
- Пишите понятные сообщения коммитов  
- Описывайте в Pull Request:
  - что сделано
  - зачем это нужно
  - как это проверить  

---

### Обратная связь

- Issues: https://github.com/Ari100kratov/ByteFight/issues  
- Telegram: https://t.me/whatislovesir
- Google-форма: https://docs.google.com/forms/d/e/1FAIpQLSd-krD2U1ENQKC0zog9loBzZQvXJMm3sfrzJ-w8HAjb2lGZOw/viewform?usp=dialog

---

## 🧭 Планы развития

### Игровые режимы
- **PvP** режим
- Полноценный **PvE** режим (сюжет)
- **Кооперативный PvE** режим (совместное прохождение)

### Геймдизайн и боевая система
- Больше особенностей классов/специализаций
- Пассивные и активные способности, заклинания
- Больше интерактивности на арене
- Новые арены с различной механикой

### Программирование поведения персонажа
- Более структурный код поведения
- Возможность работы с состоянием между ходами
- Несколько взаимодействующих файлов/модулей
- Дальнейшие улучшения IntelliSense
- Поддержка **TypeScript/JavaScript** для программирования

### Инфраструктура, качество и UX
- Инфраструктурные улучшения и оптимизации
- Редизайн игрового процесса: эффекты, ассеты, анимации
- Расширение покрытия юнит и архитектурными тестами

---

## 📄 Лицензия

Смотрите файл [LICENSE](./LICENSE).
