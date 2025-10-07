FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TMKMiniApp.csproj", "."]
RUN dotnet restore "TMKMiniApp.csproj"
COPY . .
RUN dotnet build "TMKMiniApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TMKMiniApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Создаем папки для данных
RUN mkdir -p /app/Data /app/updates /app/updates/processed
# Копируем JSON файлы данных
COPY Data/ /app/Data/
ENTRYPOINT ["dotnet", "TMKMiniApp.dll"]