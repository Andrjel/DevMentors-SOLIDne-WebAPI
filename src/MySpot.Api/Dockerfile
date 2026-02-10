FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MySpot.Api/MySpot.Api.csproj", "src/MySpot.Api/"]
COPY ["src/MySpot.Core/MySpot.Core.csproj", "src/MySpot.Core/"]
COPY ["src/MySpot.Infrastructure/MySpot.Infrastructure.csproj", "src/MySpot.Infrastructure/"]
COPY ["src/MySpot.App/MySpot.App.csproj", "src/MySpot.App/"]
RUN dotnet restore "src/MySpot.Api/MySpot.Api.csproj"
COPY . .
WORKDIR "/src/src/MySpot.Api"
RUN dotnet build "./MySpot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MySpot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MySpot.Api.dll"]
