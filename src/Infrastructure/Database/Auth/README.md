# Auth Migrations

Миграции для `AuthDbContext`.

## Добавить новую миграцию

```bash
dotnet ef migrations add <MigrationName> \
  --context AuthDbContext \
  --output-dir Database/Auth/Migrations \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

Пример:

```bash
dotnet ef migrations add InitialAuthMigration --context AuthDbContext --output-dir Database/Auth/Migrations --project src/Infrastructure/Infrastructure.csproj --startup-project src/Web.Api/Web.Api.csproj
```

## Применить миграции

```bash
dotnet ef database update \
  --context AuthDbContext \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```