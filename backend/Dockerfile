# Use the official .NET 7 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET 7 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["TMKMiniApp.csproj", "."]
RUN dotnet restore "TMKMiniApp.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "TMKMiniApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TMKMiniApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Copy JSON data files
COPY ["Data/*.json", "Data/"]

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "TMKMiniApp.dll"]
