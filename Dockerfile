# Dockerfile for Lemoo.UI API
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Hosts/Lemoo.Api/Lemoo.Api.csproj", "Lemoo.Api/"]
COPY ["src/Hosts/Lemoo.Bootstrap/Lemoo.Bootstrap.csproj", "Lemoo.Bootstrap/"]
COPY ["src/Core/Lemoo.Core.Abstractions/Lemoo.Core.Abstractions.csproj", "Lemoo.Core.Abstractions/"]
COPY ["src/Core/Lemoo.Core.Domain/Lemoo.Core.Domain.csproj", "Lemoo.Core.Domain/"]
COPY ["src/Core/Lemoo.Core.Application/Lemoo.Core.Application.csproj", "Lemoo.Core.Application/"]
COPY ["src/Core/Lemoo.Core.Infrastructure/Lemoo.Core.Infrastructure.csproj", "Lemoo.Core.Infrastructure/"]
COPY ["src/Core/Lemoo.Core.Common/Lemoo.Core.Common.csproj", "Lemoo.Core.Common/"]
COPY ["src/Modules/Lemoo.Modules.Abstractions/Lemoo.Modules.Abstractions.csproj", "Lemoo.Modules.Abstractions/"]
COPY ["src/Modules/Lemoo.Modules.TaskManager/Lemoo.Modules.TaskManager.csproj", "Lemoo.Modules.TaskManager/"]

# Restore dependencies
RUN dotnet restore "Lemoo.Api/Lemoo.Api.csproj"

# Copy all source files
COPY . .

# Build project
WORKDIR "/src/Lemoo.Api"
RUN dotnet build "Lemoo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Lemoo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lemoo.Api.dll"]
