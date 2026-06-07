# Build depuis le contexte ./GreenMarket (voir docker-compose.prod.yml)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/GreenMarket.Client
RUN dotnet publish -c Release -o /app/publish

# Sert les fichiers statiques Blazor WebAssembly via nginx
FROM nginx:alpine AS runtime
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY GreenMarket.Client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
