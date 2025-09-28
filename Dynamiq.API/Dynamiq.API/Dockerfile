# ===== Base image =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Встановлюємо порт для Kestrel
ENV APP_PORT=80
ENV ASPNETCORE_URLS=http://+:${APP_PORT}

# Відкриваємо порт 80 для зовнішнього доступу
EXPOSE ${APP_PORT}

# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо проєкт
COPY Dynamiq.API/ ./src/
RUN dotnet restore "src/Dynamiq.API/Dynamiq.API.csproj"

COPY . .
WORKDIR "src/Dynamiq.API"
RUN dotnet build "./Dynamiq.API.csproj" -c Release -o /app/build

# ===== Publish stage =====
FROM build AS publish
RUN dotnet publish "./Dynamiq.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== Final stage =====
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dynamiq.API.dll"]
