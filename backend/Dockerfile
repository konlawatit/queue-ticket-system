# API image. Built by docker-compose.yml (build context: backend/).

# Same SDK and runtime versions as local development (global.json, package versions).
FROM mcr.microsoft.com/dotnet/sdk:10.0.404 AS build
WORKDIR /src

# Restore first, from the project files only, so this layer is cached until a
# dependency changes rather than on every code change.
COPY global.json Directory.Build.props ./
COPY src/QueueSystem.Domain/QueueSystem.Domain.csproj src/QueueSystem.Domain/
COPY src/QueueSystem.Application/QueueSystem.Application.csproj src/QueueSystem.Application/
COPY src/QueueSystem.Infrastructure/QueueSystem.Infrastructure.csproj src/QueueSystem.Infrastructure/
COPY src/QueueSystem.Api/QueueSystem.Api.csproj src/QueueSystem.Api/
RUN dotnet restore src/QueueSystem.Api/QueueSystem.Api.csproj

COPY src/ src/
RUN dotnet publish src/QueueSystem.Api/QueueSystem.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0.15 AS runtime
WORKDIR /app
COPY --from=build /app .

# Run as the image's built-in non-root user.
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "QueueSystem.Api.dll"]
