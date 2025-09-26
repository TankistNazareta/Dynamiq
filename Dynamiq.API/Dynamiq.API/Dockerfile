FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

ARG APP_PORT=80

ENV APP_PORT=$APP_PORT
ENV ASPNETCORE_URLS=http://+:${APP_PORT}

EXPOSE ${APP_PORT}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Dynamiq.API/ ./src/

RUN dotnet restore "src/Dynamiq.API/Dynamiq.API.csproj"

COPY . .

WORKDIR "src/Dynamiq.API"
RUN dotnet build "./Dynamiq.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dynamiq.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dynamiq.API.dll"] 