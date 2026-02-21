# Game Migrations

Миграции для `GameDbContext`.

## Добавить новую миграцию

```bash
dotnet ef migrations add <MigrationName> \
  --context GameDbContext \
  --output-dir Database/Game/Migrations \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

Пример:

```bash
dotnet ef migrations add AddPositionsToArena --context GameDbContext --output-dir Database/Game/Migrations --project src/Infrastructure/Infrastructure.csproj --startup-project src/Web.Api/Web.Api.csproj
```

## Применить миграции

```bash
dotnet ef database update \
  --context GameDbContext \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```