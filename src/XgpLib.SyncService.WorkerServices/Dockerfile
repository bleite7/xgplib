# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/XgpLib.SyncService.WorkerServices/XgpLib.SyncService.WorkerServices.csproj", "src/XgpLib.SyncService.WorkerServices/"]
COPY ["src/XgpLib.SyncService.Application/XgpLib.SyncService.Application.csproj", "src/XgpLib.SyncService.Application/"]
COPY ["src/XgpLib.SyncService.Domain/XgpLib.SyncService.Domain.csproj", "src/XgpLib.SyncService.Domain/"]
COPY ["src/XgpLib.SyncService.CrossCutting/XgpLib.SyncService.CrossCutting.csproj", "src/XgpLib.SyncService.CrossCutting/"]
COPY ["src/XgpLib.SyncService.Infrastructure/XgpLib.SyncService.Infrastructure.csproj", "src/XgpLib.SyncService.Infrastructure/"]

RUN dotnet restore "./src/XgpLib.SyncService.WorkerServices/XgpLib.SyncService.WorkerServices.csproj"
COPY . .

WORKDIR "/src/src/XgpLib.SyncService.WorkerServices"
RUN dotnet build "./XgpLib.SyncService.WorkerServices.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./XgpLib.SyncService.WorkerServices.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XgpLib.SyncService.WorkerServices.dll"]
