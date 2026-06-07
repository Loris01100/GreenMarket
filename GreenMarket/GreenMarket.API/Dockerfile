# Build depuis le contexte ./GreenMarket (voir docker-compose.prod.yml)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copie des csproj d'abord pour profiter du cache de restore
COPY ["GreenMarket.API/GreenMarket.API.csproj", "GreenMarket.API/"]
COPY ["GreenMarket.Application/GreenMarket.Application.csproj", "GreenMarket.Application/"]
COPY ["GreenMarket.Domain/GreenMarket.Domain.csproj", "GreenMarket.Domain/"]
COPY ["GreenMarket.Shared/GreenMarket.Shared.csproj", "GreenMarket.Shared/"]
RUN dotnet restore "GreenMarket.API/GreenMarket.API.csproj"

COPY . .
WORKDIR /src/GreenMarket.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GreenMarket.API.dll"]
