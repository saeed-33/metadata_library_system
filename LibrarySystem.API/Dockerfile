FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LibrarySystem.API/LibrarySystem.API.csproj", "LibrarySystem.API/"]
COPY ["LibrarySystem.Application/LibrarySystem.Application.csproj", "LibrarySystem.Application/"]
COPY ["LibrarySystem.DataAccess/LibrarySystem.DataAccess.csproj", "LibrarySystem.DataAccess/"]
COPY ["LibrarySystem.Domain/LibrarySystem.Domain.csproj", "LibrarySystem.Domain/"]
RUN dotnet restore "LibrarySystem.API/LibrarySystem.API.csproj"

COPY . .
WORKDIR "/src/LibrarySystem.API"
RUN dotnet publish "LibrarySystem.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.API.dll"]
