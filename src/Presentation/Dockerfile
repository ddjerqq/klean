FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y curl
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
RUN dotnet restore "Presentation/Presentation.csproj"
COPY . .
WORKDIR "/src/Presentation"
RUN dotnet build "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=5s --timeout=3s --retries=3 CMD curl --silent --fail http://localhost:5000/health || exit 1
HEALTHCHECK --interval=5s --timeout=3s --retries=3 CMD curl --silent --fail http://localhost:5000/health/db || exit 1

# compose
# healthcheck:
#   test: ["CMD-SHELL", "curl --fail http://localhost:80/health || exit 1"]
#     interval: 30s
#     timeout: 30s
#     retries: 3

ENTRYPOINT ["dotnet", "Presentation.dll"]
