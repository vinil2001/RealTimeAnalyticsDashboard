FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SensorAnalyticsAPI/SensorAnalyticsAPI.csproj", "SensorAnalyticsAPI/"]
RUN dotnet restore "SensorAnalyticsAPI/SensorAnalyticsAPI.csproj"
COPY . .
WORKDIR "/src/SensorAnalyticsAPI"
RUN dotnet build "SensorAnalyticsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SensorAnalyticsAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SensorAnalyticsAPI.dll"]
