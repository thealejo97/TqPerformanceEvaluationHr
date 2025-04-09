FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TqPerformanceEvaluationHr.Web.csproj"
RUN dotnet build "TqPerformanceEvaluationHr.Web.csproj" -c Release -o /app/build
RUN dotnet publish "TqPerformanceEvaluationHr.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TqPerformanceEvaluationHr.Web.dll"]
