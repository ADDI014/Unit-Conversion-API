# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and restore
COPY UnitConversionApi.sln ./
COPY src/UnitConversionApi/UnitConversionApi.csproj ./src/UnitConversionApi/
RUN dotnet restore src/UnitConversionApi/UnitConversionApi.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish src/UnitConversionApi/UnitConversionApi.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "UnitConversionApi.dll"]