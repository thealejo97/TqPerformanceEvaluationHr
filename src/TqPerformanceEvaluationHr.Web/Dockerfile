# Runtime puro (solo para ejecutar la app en producción)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de build: compila y publica la app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/TqPerformanceEvaluationHr.Web/TqPerformanceEvaluationHr.Web.csproj"
RUN dotnet build "src/TqPerformanceEvaluationHr.Web/TqPerformanceEvaluationHr.Web.csproj" -c Release -o /app/build
RUN dotnet publish "src/TqPerformanceEvaluationHr.Web/TqPerformanceEvaluationHr.Web.csproj" -c Release -o /app/publish

# Etapa final para ejecutar la app publicada
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TqPerformanceEvaluationHr.Web.dll"]

# Etapa para migraciones (no se publica, solo ejecuta ef)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
WORKDIR /src
COPY . .
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
