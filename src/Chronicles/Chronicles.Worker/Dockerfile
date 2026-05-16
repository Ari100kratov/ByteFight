# Production image for ByteFight Chronicles worker.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Chronicles/Chronicles.Worker/Chronicles.Worker.csproj", "src/Chronicles/Chronicles.Worker/"]
COPY ["src/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "src/Aspire.ServiceDefaults/"]
COPY ["src/Chronicles/Chronicles.Application/Chronicles.Application.csproj", "src/Chronicles/Chronicles.Application/"]
COPY ["src/Chronicles/Chronicles.Domain/Chronicles.Domain.csproj", "src/Chronicles/Chronicles.Domain/"]
COPY ["src/Chronicles/Chronicles.Infrastructure/Chronicles.Infrastructure.csproj", "src/Chronicles/Chronicles.Infrastructure/"]
COPY ["src/IntegrationContracts/IntegrationContracts.csproj", "src/IntegrationContracts/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
RUN dotnet restore "./src/Chronicles/Chronicles.Worker/Chronicles.Worker.csproj"

COPY . .
WORKDIR "/src/src/Chronicles/Chronicles.Worker"
RUN dotnet build "./Chronicles.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Chronicles.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chronicles.Worker.dll"]
