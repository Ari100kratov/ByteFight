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

> [!IMPORTANT]
> Замените `Ari100kratov` в ссылках и бейджах на ваш GitHub-логин/организацию.

## Содержание
- [О проекте](#-о-проекте)
- [Суть проекта и ключевые особенности](#-суть-проекта-и-ключевые-особенности)
- [Технологии, библиотеки и архитектура](#-технологии-библиотеки-и-архитектура)
- [Локальный запуск (Windows-first)](#-локальный-запуск-windows-first)
- [Контрибьютинг и обратная связь](#-контрибьютинг-и-обратная-связь)
- [Планы развития](#-планы-развития)
- [Лицензия](#-лицензия)

---

## 📌 О проекте

**ByteFight** — образовательный pet-проект, в котором игрок программирует поведение своего персонажа на C#, запускает бой на арене и наблюдает ход сражения в real-time.

Проект создан в первую очередь для практики:
- архитектуры и проектирования доменной модели,
- backend/frontend интеграции,
- real-time взаимодействия,
- безопасной компиляции и запуска пользовательского кода.

Проект открыт для контрибьюта. Буду рад улучшениям, новым идеям, исправлениям ошибок и конструктивному обсуждению.

---

## 🎯 Суть проекта и ключевые особенности

### В чем основная идея
ByteFight — это «программируемая арена»:
1. Пользователь регистрируется и создает персонажа.
2. Редактирует код поведения персонажа.
3. Запускает игровую сессию на арене.
4. Наблюдает тики боя и итог матча в реальном времени.

### Что проект умеет сейчас
- **Игровые режимы на уровне доменной модели**: `Training`, `PvE`, `PvP`.
- **Запуск игровых сессий и обработка ходов**:
  - решения игрока берутся из пользовательского кода,
  - действия NPC выполняются встроенным AI,
  - результат фиксируется в runtime-сущностях и логах.
- **Real-time события боя через SignalR**:
  - присоединение/выход из сессии,
  - поток `Tick`-событий,
  - финальное событие `Finished`.
- **Редактор поведения персонажа**:
  - несколько вкладок/файлов,
  - локальные изменения,
  - сохранение на сервер,
  - шаблон стартового кода.
- **IntelliSense API для C#-скриптов**:
  - diagnostics,
  - completions,
  - hover,
  - signature help.
- **Безопасность пользовательского кода**:
  - ограничение вызовов опасных API,
  - изоляция исполнения через отдельный worker.
- **Рендер игровой сцены и ассетов на фронтенде**:
  - отрисовка арены,
  - отрисовка персонажей и врагов,
  - анимации состояний/действий юнитов,
  - отображение боевого лога и результата матча.
- **Инициализация ролей и игровых данных**:
  - сидирование ролей/permissions,
  - начальные арены, враги, классы.

### Почему это хороший учебный полигон
- В одном репозитории сочетаются backend, frontend, инфраструктура и игровой runtime.
- Есть реальная продуктовая сложность: доменная логика, state management, real-time доставка событий, рендер и анимация.

---

## 🧱 Технологии, библиотеки и архитектура

## Backend / Platform (.NET)

### Ядро и API
- .NET 10 (`net10.0`)
- ASP.NET Core Minimal API
- OpenAPI + Scalar
- FluentValidation
- Scrutor

### Данные и инфраструктура
- Entity Framework Core 10
- Npgsql + PostgreSQL
- Несколько DbContext: `AuthDbContext`, `GameDbContext`, `GameRuntimeDbContext`
- MinIO (S3-совместимое хранилище ассетов)

### Auth / Security
- JWT Bearer Authentication
- Permission-based authorization
- Сидирование ролей и прав

### Runtime и скриптинг
- SignalR
- Roslyn (Microsoft.CodeAnalysis): компиляция, анализ и IntelliSense для пользовательского кода
- UserCodeWorker для изолированного исполнения пользовательских скриптов

### Observability
- Aspire.ServiceDefaults
- HealthChecks
- OpenTelemetry

### Оркестрация
- .NET Aspire AppHost поднимает PostgreSQL, MinIO и Web API с нужными зависимостями.

---

## Frontend (React + Vite)

### Библиотеки
- React 19 + TypeScript + Vite
- react-router-dom
- @tanstack/react-query
- zustand
- @microsoft/signalr
- monaco-editor + @monaco-editor/react
- pixi.js + @pixi/react
- tailwindcss 4 + Radix UI
- sonner, lucide-react, react-resizable-panels

### Frontend-подходы
- Feature-oriented структура
- Разделение server-state (TanStack Query) и client-state (Zustand)
- Real-time обновление состояния игры
- Canvas/2D-рендер и анимации боевых юнитов

---

## Архитектурный срез репозитория

```text
src/
  Domain/                # бизнес-сущности и правила
  Application/           # команды/запросы, use-cases, handlers
  Infrastructure/        # EF Core, auth, MinIO, seed, policy provider
  GameRuntime/           # игровой цикл, AI, user code compilation/execution, realtime
  Web.Api/               # HTTP endpoint'ы, DI, middleware
  ClientApp/             # React SPA (редактор кода, арены, бой)
  Aspire.AppHost/        # оркестрация локального окружения
```

### Архитектурные подходы
- Слоистая архитектура
- CQRS-like подход (`ICommandHandler`, `IQueryHandler`, декораторы)
- Domain-first модель
- Event-driven realtime взаимодействие

---

## 🚀 Локальный запуск (Windows-first)

Рекомендованный стек:
- Visual Studio 2026 (или актуальная VS с поддержкой .NET + Aspire)
- Docker Desktop
- .NET SDK 10
- Node.js LTS 22+
- pnpm

### 1) Установка зависимостей

- .NET SDK: https://dotnet.microsoft.com/download
- Visual Studio: https://visualstudio.microsoft.com/
- Docker Desktop: https://www.docker.com/products/docker-desktop/
- Node.js: https://nodejs.org/
- pnpm: https://pnpm.io/installation
- .NET Aspire: https://learn.microsoft.com/dotnet/aspire/

### 2) Клонирование

```powershell
git clone https://github.com/Ari100kratov/ByteFight.git
cd ByteFight
```

### 3) Frontend зависимости

```powershell
cd src/ClientApp
pnpm install
cd ../..
```

### 4) Запуск backend инфраструктуры через Aspire

#### Вариант A (рекомендуется): Visual Studio
1. Откройте `ByteFight.sln`.
2. Выберите стартовый проект `Aspire.AppHost`.
3. Запустите (`F5` / `Ctrl+F5`).

#### Вариант B: CLI
```powershell
dotnet run --project src/Aspire.AppHost/Aspire.AppHost.csproj
```

### 5) Запуск frontend

```powershell
cd src/ClientApp
pnpm dev
```

Ожидаемые адреса по умолчанию:
- Frontend: `http://localhost:5173`
- API: `http://localhost:5000`
- MinIO API: `http://localhost:9000`
- MinIO Console: `http://localhost:9001`

### 6) Проверки и полезные команды

```powershell
dotnet restore ByteFight.sln
dotnet build ByteFight.sln -c Debug
dotnet test ByteFight.sln -c Debug

cd src/ClientApp
pnpm build
```

### 7) Пустой подраздел для последующего заполнения

> [!NOTE]
> Ниже intentionally оставлены заготовки, которые будут заполнены позже.

#### Данные администратора по умолчанию (TODO)
- **Планируется добавить**:
  - логин/почту админа по умолчанию,
  - стартовый пароль,
  - рекомендации по смене пароля после первого запуска.

#### Стартовые ассеты и загрузка в MinIO (TODO)
- **Планируется добавить**:
  - ссылку на общий ресурс с ассетами,
  - ожидаемую структуру папок,
  - какие бакеты создать,
  - куда именно и в каком формате загружать файлы вручную.

### 8) Troubleshooting
- Убедитесь, что Docker Desktop запущен.
- Проверьте, что порты `5000`, `5173`, `5432`, `9000`, `9001` свободны.
- Проверьте SDK: `dotnet --info`.
- Если фронтенд падает по зависимостям: удалите `node_modules` и выполните `pnpm install` заново.

---

## 🤝 Контрибьютинг и обратная связь

Контрибьюты приветствуются:
1. Fork репозитория
2. Feature branch
3. Коммиты с понятным описанием
4. Pull Request с пояснением: что сделано, зачем и как проверено

Обратная связь:
- Telegram: **[@whatislovesir](https://t.me/whatislovesir)**
- Позже добавлю Google Form

---

## 🧭 Планы развития

### Игровые режимы
- Полноценный **PvP** режим
- Полноценный **PvE** режим
- **Кооперативный PvE** режим (совместное прохождение)

### Геймдизайн и боевая система
- Больше особенностей классов/специализаций
- Пассивные способности
- Активные способности и заклинания
- Больше интерактивности на арене
- Новые арены с разной механикой

### Программирование поведения персонажа
- Более структурный код поведения
- Улучшенная работа с состоянием между ходами
- Несколько взаимодействующих файлов/модулей
- Дальнейшие улучшения IntelliSense
- Поддержка **TypeScript/JavaScript** для написания поведения

### Инфраструктура, качество и UX
- Инфраструктурные улучшения и оптимизации
- Редизайн игрового процесса: эффекты, ассеты, анимации
- Расширение покрытия unit и архитектурными тестами

---

## 📄 Лицензия

Смотрите файл [LICENSE](./LICENSE).
