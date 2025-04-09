# Etapa base solo para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/TqPerformanceEvaluationHr.Web/TqPerformanceEvaluationHr.Web.csproj"
RUN dotnet build "src/TqPerformanceEvaluationHr.Web/TqPerformanceEvaluationHr.Web.csproj" -c Release -o /app/build
RUN dotnet publish "src/TqPerformanceEvaluationHr.Web/TqPerformanceEvaluationHr.Web.csproj" -c Release -o /app/publish

# Etapa exclusiva para migraciones
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
WORKDIR /src
COPY . .
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Etapa final para ejecución web (usa aspnet)
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TqPerformanceEvaluationHr.Web.dll"]
