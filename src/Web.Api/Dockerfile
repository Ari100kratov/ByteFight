# Production image for ByteFight Web API.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Web.Api/Web.Api.csproj", "src/Web.Api/"]
COPY ["src/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "src/Aspire.ServiceDefaults/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/Chronicles/Chronicles.Application/Chronicles.Application.csproj", "src/Chronicles/Chronicles.Application/"]
COPY ["src/Chronicles/Chronicles.Domain/Chronicles.Domain.csproj", "src/Chronicles/Chronicles.Domain/"]
COPY ["src/Chronicles/Chronicles.Infrastructure/Chronicles.Infrastructure.csproj", "src/Chronicles/Chronicles.Infrastructure/"]
COPY ["src/IntegrationContracts/IntegrationContracts.csproj", "src/IntegrationContracts/"]
COPY ["src/GameRuntime/GameRuntime/GameRuntime.csproj", "src/GameRuntime/GameRuntime/"]
COPY ["src/GameRuntime/GameRuntime.Common/GameRuntime.Common.csproj", "src/GameRuntime/GameRuntime.Common/"]
COPY ["src/GameRuntime/GameRuntime.Logic.User.Api/GameRuntime.Logic.User.Api.csproj", "src/GameRuntime/GameRuntime.Logic.User.Api/"]
COPY ["src/GameRuntime/GameRuntime.UserCodeApiDocumentation/GameRuntime.UserCodeApiDocumentation.csproj", "src/GameRuntime/GameRuntime.UserCodeApiDocumentation/"]
COPY ["src/GameRuntime/GameRuntime.UserCodeWorker/GameRuntime.UserCodeWorker.csproj", "src/GameRuntime/GameRuntime.UserCodeWorker/"]
RUN dotnet restore "./src/Web.Api/Web.Api.csproj"
RUN dotnet restore "./src/GameRuntime/GameRuntime.UserCodeWorker/GameRuntime.UserCodeWorker.csproj"

COPY . .
WORKDIR "/src/src/Web.Api"
RUN dotnet build "./Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.Api.dll"]
