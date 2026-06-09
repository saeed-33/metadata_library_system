FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Logging.API/Logging.API.csproj", "Logging.API/"]
COPY ["Logging.Application/Logging.Application.csproj", "Logging.Application/"]
COPY ["Logging.Infrastructure/Logging.Infrastructure.csproj", "Logging.Infrastructure/"]
COPY ["Logging.Domain/Logging.Domain.csproj", "Logging.Domain/"]
RUN dotnet restore "Logging.API/Logging.API.csproj"

COPY . .
WORKDIR "/src/Logging.API"
RUN dotnet publish "Logging.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Logging.API.dll"]
