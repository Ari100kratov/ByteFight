# Production image for ByteFight database migrator.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Migrator/Migrator.csproj", "src/Migrator/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "src/Aspire.ServiceDefaults/"]
COPY ["src/Chronicles/Chronicles.Application/Chronicles.Application.csproj", "src/Chronicles/Chronicles.Application/"]
COPY ["src/Chronicles/Chronicles.Domain/Chronicles.Domain.csproj", "src/Chronicles/Chronicles.Domain/"]
COPY ["src/Chronicles/Chronicles.Infrastructure/Chronicles.Infrastructure.csproj", "src/Chronicles/Chronicles.Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/GameRuntime/GameRuntime/GameRuntime.csproj", "src/GameRuntime/GameRuntime/"]
COPY ["src/GameRuntime/GameRuntime.Common/GameRuntime.Common.csproj", "src/GameRuntime/GameRuntime.Common/"]
COPY ["src/GameRuntime/GameRuntime.Logic.User.Api/GameRuntime.Logic.User.Api.csproj", "src/GameRuntime/GameRuntime.Logic.User.Api/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/IntegrationContracts/IntegrationContracts.csproj", "src/IntegrationContracts/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
RUN dotnet restore "./src/Migrator/Migrator.csproj"

COPY . .
WORKDIR "/src/src/Migrator"
RUN dotnet build "./Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Migrator.dll"]
