# GameRuntime Migrations

Миграции для `GameRuntimeDbContext`.

## Добавить новую миграцию

```bash
dotnet ef migrations add <MigrationName> \
  --context GameRuntimeDbContext \
  --output-dir Database/GameRuntime/Migrations \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

Пример:

```bash
dotnet ef migrations add InitialMigration --context GameRuntimeDbContext --output-dir Database/GameRuntime/Migrations --project src/Infrastructure/Infrastructure.csproj --startup-project src/Web.Api/Web.Api.csproj
```

## Применить миграции

```bash
dotnet ef database update \
  --context GameRuntimeDbContext \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```