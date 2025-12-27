# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Serve HTTP on 8080 inside container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Solution props (for consistent restore)
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy csproj files for restore caching
COPY src/CloudNet.Api/CloudNet.Api.csproj src/CloudNet.Api/
COPY src/CloudNet.Api.Abstractions/CloudNet.Api.Abstractions.csproj src/CloudNet.Api.Abstractions/
COPY src/CloudNet.Application/CloudNet.Application.csproj src/CloudNet.Application/
COPY src/CloudNet.Domain/CloudNet.Domain.csproj src/CloudNet.Domain/
COPY src/CloudNet.Infrastructure.Identity/CloudNet.Infrastructure.Identity.csproj src/CloudNet.Infrastructure.Identity/
COPY src/CloudNet.Infrastructure.Persistence/CloudNet.Infrastructure.Persistence.csproj src/CloudNet.Infrastructure.Persistence/

RUN dotnet restore src/CloudNet.Api/CloudNet.Api.csproj

# Copy everything else and publish
COPY . .
WORKDIR /src/src/CloudNet.Api
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CloudNet.Api.dll"]