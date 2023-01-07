FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:7.0-jammy-arm64v8 AS base
COPY ["./auxbin/speedtest", "/bin/"]
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NRpiMonitor/NRpiMonitor.csproj", "NRpiMonitor/"]
RUN dotnet restore -r linux-arm64 "NRpiMonitor/NRpiMonitor.csproj"
COPY . .
WORKDIR "/src/NRpiMonitor"
RUN dotnet build "NRpiMonitor.csproj" -c Release -o /app/build --no-restore -r linux-arm64 --no-self-contained

FROM build AS publish
RUN dotnet publish "NRpiMonitor.csproj" -c Release -r linux-arm64 --no-self-contained -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NRpiMonitor.dll"]
