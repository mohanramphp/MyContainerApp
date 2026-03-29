# Multi-stage build for .NET 8 Web API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["MyContainerApp.sln", "."]
COPY ["src/MyContainerApp.API/MyContainerApp.API.csproj", "src/MyContainerApp.API/"]
COPY ["src/MyContainerApp.Application/MyContainerApp.Application.csproj", "src/MyContainerApp.Application/"]
COPY ["src/MyContainerApp.Domain/MyContainerApp.Domain.csproj", "src/MyContainerApp.Domain/"]
COPY ["src/MyContainerApp.Infrastructure/MyContainerApp.Infrastructure.csproj", "src/MyContainerApp.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "MyContainerApp.sln"

# Copy remaining source code
COPY . .

# Build application
RUN dotnet build "MyContainerApp.sln" -c Release -o /app/build

# Publish application
RUN dotnet publish "src/MyContainerApp.API/MyContainerApp.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 8080 (standard for OpenShift)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "MyContainerApp.API.dll"]
