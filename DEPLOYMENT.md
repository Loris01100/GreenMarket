# GreenMarket — Guide de déploiement

Déploiement auto-hébergé sur un PC personnel sous **CachyOS (Linux)** servant de serveur, avec exposition publique via **Cloudflare Tunnel** pour la démo depuis l'extérieur.

> Stack ciblée : Docker natif sur CachyOS + Caddy (reverse proxy interne) + Cloudflare Tunnel (exposition HTTPS publique, sans ouvrir de port sur la box).

---

## 1. Architecture de déploiement

```
                     Internet
                        │
                        ▼
            ┌─────────────────────┐
            │ Cloudflare Tunnel   │ (HTTPS, anycast)
            │  *.tondomaine.com   │
            └──────────┬──────────┘
                       │ tunnel chiffré sortant
                       ▼
    ┌──────────────────────────────────────────┐
    │  PC fixe CachyOS — réseau local maison   │
    │                                          │
    │  ┌────────────┐                          │
    │  │ cloudflared│ (conteneur)              │
    │  └─────┬──────┘                          │
    │        │                                 │
    │        ▼                                 │
    │  ┌────────────┐                          │
    │  │   Caddy    │ reverse proxy interne    │
    │  └─┬────┬───┬─┘                          │
    │    │    │   │                            │
    │    ▼    ▼   ▼                            │
    │  ┌──┐ ┌──┐ ┌──────────┐                  │
    │  │API│ │Client│ │Keycloak │              │
    │  └─┬─┘ └────┘ └────┬─────┘                │
    │    │               │                     │
    │    └───────┬───────┘                     │
    │            ▼                             │
    │     ┌──────────────┐                     │
    │     │ PostgreSQL   │                     │
    │     │  ├─ greenmarket_db                 │
    │     │  └─ keycloak_db                    │
    │     └──────────────┘                     │
    └──────────────────────────────────────────┘
```

**Aucun port n'est ouvert** sur la box internet. `cloudflared` établit une connexion sortante chiffrée vers Cloudflare, qui route les requêtes publiques entrantes via ce tunnel.

---

## 2. Stratégie de base de données

### Situation actuelle (dev)

Dans `docker-compose.yml`, Keycloak et l'application partagent **la même base** :

```yaml
KC_DB_URL: jdbc:postgresql://postgres:5432/greenmarket_db
```

Concrètement :
- **1 conteneur PostgreSQL** (`greenmarket-postgres`)
- **1 base** `greenmarket_db` qui contient deux familles de tables :
  - Tables Keycloak (`user_entity`, `realm`, `client`...) créées au démarrage de Keycloak
  - Tables applicatives (`Produits`, etc.) créées par les migrations EF Core

Ça fonctionne car les noms ne se chevauchent pas, mais ce n'est **pas adapté à la prod**.

### Cible pour la prod : bases séparées

| Base | Utilisateur | Rôle |
|---|---|---|
| `greenmarket_db` | `greenmarket` | Données métier (produits, commandes, stock...) |
| `keycloak_db` | `keycloak` | Données d'auth (users, realms, rôles, sessions...) |

Avantages :
- Sauvegardes indépendantes (`pg_dump` ciblé)
- Permissions cloisonnées (chaque user ne voit que sa base)
- Migrations EF Core sans risque pour Keycloak
- Possibilité future de séparer sur deux serveurs PostgreSQL

### Script d'initialisation

Créer `init-db/01-init.sql` (monté dans le conteneur Postgres) :

```sql
CREATE USER keycloak WITH PASSWORD 'CHANGE_ME';
CREATE DATABASE keycloak_db OWNER keycloak;
GRANT ALL PRIVILEGES ON DATABASE keycloak_db TO keycloak;
```

Le user `greenmarket` et la base `greenmarket_db` restent créés par les variables d'env du conteneur Postgres comme aujourd'hui.

---

## 3. Préparer le PC serveur (CachyOS)

```bash
# Installer Docker
sudo pacman -S docker docker-compose git

# Activer Docker au démarrage
sudo systemctl enable --now docker

# Pouvoir utiliser docker sans sudo
sudo usermod -aG docker $USER
# (se déconnecter / reconnecter ensuite)

# Empêcher la mise en veille (serveur 24/7)
sudo systemctl mask sleep.target suspend.target hibernate.target hybrid-sleep.target

# Vérifier
docker run hello-world
```

**Réseau local** : assigner une **IP fixe** au PC via la box internet (réservation DHCP par adresse MAC), ex. `192.168.1.50`. Évite que l'IP change après redémarrage.

---

## 4. Domaine et exposition

### Option recommandée : domaine Cloudflare (~10 €/an)

1. Créer un compte sur https://dash.cloudflare.com
2. Acheter un domaine via **Cloudflare Registrar** (prix coûtant, pas de marge)
3. Conseil : choisir un nom **générique** (ex. `lbach.fr`, `lorisbach.dev`) — réutilisable pour d'autres projets via sous-domaines :
   - `greenmarket.lbach.fr`
   - `api.lbach.fr`
   - `auth.lbach.fr`
   - `portfolio.lbach.fr` plus tard, etc.

> Un nom de domaine ne se renomme pas une fois acheté, mais tu peux créer autant de sous-domaines que tu veux et les rediriger vers ce que tu veux à tout moment.

### Option 100 % gratuite : DuckDNS + port forwarding

Voir [annexe](#annexe--variante-100--gratuite-duckdns--port-forwarding).

---

## 5. Configurer le tunnel Cloudflare

1. Dashboard Cloudflare → **Zero Trust** → **Networks** → **Tunnels** → **Create a tunnel**
2. Choisir **Cloudflared**, nommer le tunnel (ex. `greenmarket-home`)
3. Copier le **token** affiché → à stocker dans `.env.production`
4. Dans la section **Public Hostnames** du tunnel, créer trois routes :

| Subdomain | Domain | Service |
|---|---|---|
| `greenmarket` | `lbach.fr` | `http://caddy:80` |
| `api` | `lbach.fr` | `http://caddy:80` |
| `auth` | `lbach.fr` | `http://caddy:80` |

Caddy fait ensuite le routage interne par hostname vers le bon conteneur.

---

## 6. Fichiers à créer dans le projet

### 6.1 `Dockerfile` côté API — `GreenMarket/GreenMarket.API/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
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
```

### 6.2 `Dockerfile` côté Client Blazor — `GreenMarket/GreenMarket.Client/Dockerfile`

Pour **Blazor WebAssembly** (servi en statique via nginx) :

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/GreenMarket.Client
RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine AS runtime
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY GreenMarket.Client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

Pour **Blazor Server**, même base que l'API (image `aspnet:10.0`).

### 6.3 `Caddyfile` — racine du projet

```caddy
{
    auto_https off
}

:80 {
    @greenmarket host greenmarket.lbach.fr
    handle @greenmarket {
        reverse_proxy client:80
    }

    @api host api.lbach.fr
    handle @api {
        reverse_proxy api:8080
    }

    @auth host auth.lbach.fr
    handle @auth {
        reverse_proxy keycloak:8080
    }
}
```

> HTTPS est géré par Cloudflare (entre le navigateur et leur edge). Le tunnel chiffre déjà le trafic edge ↔ serveur. Pas besoin de TLS côté Caddy.

### 6.4 `docker-compose.prod.yml`

```yaml
services:
  postgres:
    restart: unless-stopped
    environment:
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: ${POSTGRES_DB}
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./init-db:/docker-entrypoint-initdb.d:ro
    ports: []  # ne pas exposer en prod

  keycloak:
    image: quay.io/keycloak/keycloak:26.2
    container_name: greenmarket-keycloak
    command: start --optimized --import-realm
    restart: unless-stopped
    environment:
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak_db
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: ${KEYCLOAK_DB_PASSWORD}
      KC_HOSTNAME: ${KC_HOSTNAME}
      KC_PROXY: edge
      KC_HTTP_ENABLED: "true"
      KEYCLOAK_ADMIN: ${KEYCLOAK_ADMIN}
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
    volumes:
      - ./keycloak:/opt/keycloak/data/import:ro
    ports: []
    depends_on:
      postgres:
        condition: service_healthy

  api:
    build:
      context: ./GreenMarket
      dockerfile: GreenMarket.API/Dockerfile
    container_name: greenmarket-api
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__Default: "Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"
      Keycloak__Authority: ${PUBLIC_KEYCLOAK_URL}/realms/greenmarket
      Keycloak__Audience: greenmarket-api
      Cors__AllowedOrigins__0: ${PUBLIC_CLIENT_URL}
    depends_on:
      postgres:
        condition: service_healthy
      keycloak:
        condition: service_started

  client:
    build:
      context: ./GreenMarket
      dockerfile: GreenMarket.Client/Dockerfile
    container_name: greenmarket-client
    restart: unless-stopped
    depends_on:
      - api

  caddy:
    image: caddy:2-alpine
    container_name: greenmarket-caddy
    restart: unless-stopped
    volumes:
      - ./Caddyfile:/etc/caddy/Caddyfile:ro
    depends_on:
      - api
      - client
      - keycloak

  cloudflared:
    image: cloudflare/cloudflared:latest
    container_name: greenmarket-cloudflared
    restart: unless-stopped
    command: tunnel --no-autoupdate run --token ${CLOUDFLARE_TUNNEL_TOKEN}
    depends_on:
      - caddy

volumes:
  postgres_data:
```

### 6.5 `.env.production` (NE PAS commit dans Git)

```env
# --- PostgreSQL ---
POSTGRES_DB=greenmarket_db
POSTGRES_USER=greenmarket
POSTGRES_PASSWORD=<mot-de-passe-fort-aleatoire>

# --- Keycloak ---
KEYCLOAK_DB_PASSWORD=<autre-mdp-fort>
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=<mdp-admin-keycloak-fort>
KC_HOSTNAME=auth.lbach.fr

# --- URLs publiques ---
PUBLIC_API_URL=https://api.lbach.fr
PUBLIC_CLIENT_URL=https://greenmarket.lbach.fr
PUBLIC_KEYCLOAK_URL=https://auth.lbach.fr

# --- Cloudflare Tunnel ---
CLOUDFLARE_TUNNEL_TOKEN=<token-du-tunnel>
```

Ajouter dans `.gitignore` :
```
.env.production
init-db/
```

### 6.6 `init-db/01-init.sql`

```sql
CREATE USER keycloak WITH PASSWORD 'CHANGE_ME_SAME_AS_KEYCLOAK_DB_PASSWORD';
CREATE DATABASE keycloak_db OWNER keycloak;
GRANT ALL PRIVILEGES ON DATABASE keycloak_db TO keycloak;
```

> Le mot de passe dans ce SQL doit être identique à `KEYCLOAK_DB_PASSWORD` dans `.env.production`. Solution propre : utiliser un script bash d'init qui lit la variable d'env, plutôt qu'un `.sql` figé.

---

## 7. Adapter le code applicatif

### Côté `GreenMarket.API`

- `Program.cs` : lire `Keycloak:Authority` depuis la config (pas en dur). L'URL doit être l'**URL publique** Keycloak, pas `localhost`.
- CORS : autoriser `Cors:AllowedOrigins` depuis la config.
- Connection string PostgreSQL : lue depuis `ConnectionStrings:Default`.

### Côté `GreenMarket.Client`

- `HttpClient.BaseAddress` : doit pointer vers `PUBLIC_API_URL` (injecté au build ou via `appsettings.Production.json`).
- Config OIDC côté front : `Authority = PUBLIC_KEYCLOAK_URL/realms/greenmarket`.

### Côté Keycloak

- **Exporter le realm** depuis ta config dev : Admin Console → Realm settings → Action → **Partial export** → cocher "Include groups and roles" + "Include clients".
- Placer le JSON dans `keycloak/` (déjà monté en lecture sur `/opt/keycloak/data/import`).
- Au premier démarrage, `--import-realm` importera le realm. **Les utilisateurs et mots de passe ne sont pas exportés** — il faudra les recréer ou utiliser un export full via CLI `kc.sh export`.

---

## 8. Premier déploiement

```bash
# Sur le serveur CachyOS
mkdir -p ~/apps && cd ~/apps
git clone <ton-repo-greenmarket> GreenMarket
cd GreenMarket
git checkout main

# Créer .env.production (à partir d'un .env.production.example)
cp .env.production.example .env.production
nano .env.production   # remplir les valeurs

# Créer init-db/01-init.sql
mkdir -p init-db
nano init-db/01-init.sql

# Build + démarrage
docker compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.production up -d --build

# Vérifier
docker compose ps
docker compose logs -f api
docker compose logs -f keycloak
```

### Appliquer les migrations EF Core en prod

Deux options :

**Option 1 — Auto-migrate au démarrage** (simple, OK pour ce projet) : dans `Program.cs` de l'API, ajouter au démarrage :

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
```

**Option 2 — Migration manuelle** depuis un poste de dev :

```bash
dotnet ef database update \
  --project GreenMarket.Application \
  --startup-project GreenMarket.API \
  --connection "Host=<IP-publique-tunnel>;Port=5432;..."
```

→ Plus propre mais nécessite d'exposer Postgres temporairement.

---

## 9. Sauvegardes

```bash
mkdir -p ~/backups
crontab -e
```

Ajouter :

```cron
# Sauvegarde quotidienne à 3h du matin - base applicative
0 3 * * * docker exec greenmarket-postgres pg_dump -U greenmarket greenmarket_db | gzip > ~/backups/greenmarket-$(date +\%F).sql.gz

# Sauvegarde quotidienne à 3h10 - base Keycloak
10 3 * * * docker exec greenmarket-postgres pg_dump -U keycloak keycloak_db | gzip > ~/backups/keycloak-$(date +\%F).sql.gz

# Rotation : supprimer les sauvegardes de plus de 30 jours, le dimanche à 4h
0 4 * * 0 find ~/backups -name "*.sql.gz" -mtime +30 -delete
```

**Test de restauration** (à faire au moins une fois) :

```bash
gunzip -c ~/backups/greenmarket-2026-05-29.sql.gz | docker exec -i greenmarket-postgres psql -U greenmarket -d greenmarket_db
```

---

## 10. Tester avant la démo

Depuis ton **téléphone en données mobiles** (pas le wifi de la maison) :

- [ ] `https://greenmarket.lbach.fr` → page Blazor s'affiche
- [ ] Login Keycloak fonctionne et redirige correctement
- [ ] Appels API depuis le client passent (pas d'erreur CORS dans la console)
- [ ] Création d'un produit / commande → persisté en base
- [ ] Redémarrer le serveur → tout repart automatiquement (`restart: unless-stopped`)

**Plan B pour le jour J** : enregistrer une vidéo de démo en local au cas où l'école bloque Cloudflare (rare mais possible avec certains filtres scolaires).

---

## 11. Mise à jour de l'application

```bash
cd ~/apps/GreenMarket
git pull
docker compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.production up -d --build
```

---

## Annexe — Variante 100 % gratuite (DuckDNS + port forwarding)

Si tu refuses les ~10 €/an du domaine Cloudflare :

1. Compte DuckDNS sur https://www.duckdns.org → sous-domaine `greenmarket-loris.duckdns.org`
2. Script cron sur le serveur qui ping DuckDNS toutes les 5 min pour mettre à jour l'IP publique
3. Sur la box internet : **port forwarding** `80` et `443` vers `192.168.1.50` (IP du PC)
4. Remplacer `cloudflared` par **Caddy avec HTTPS auto** (Let's Encrypt) :

```caddy
greenmarket-loris.duckdns.org {
    reverse_proxy client:80
}
api-greenmarket-loris.duckdns.org {
    reverse_proxy api:8080
}
auth-greenmarket-loris.duckdns.org {
    reverse_proxy keycloak:8080
}
```

⚠️ **Risques** :
- Certains FAI bloquent les ports 80/443 entrants (Free, SFR mobile) → tester avant la démo
- Ton IP publique est exposée
- Le wifi de l'école peut bloquer DuckDNS (filtrage de réputation)
- DuckDNS limite à 5 sous-domaines

Pour une démo importante, Cloudflare Tunnel reste bien plus fiable.

---

## Récapitulatif des fichiers à créer/modifier

| Fichier | Type | Status |
|---|---|---|
| `DEPLOYMENT.md` | Doc (ce fichier) | À créer |
| `docker-compose.prod.yml` | Compose surcharge prod | À créer |
| `.env.production` | Secrets prod (gitignore) | À créer |
| `.env.production.example` | Template public | À créer |
| `Caddyfile` | Config reverse proxy | À créer |
| `init-db/01-init.sql` | Init base keycloak_db | À créer |
| `GreenMarket/GreenMarket.API/Dockerfile` | Image API | À créer |
| `GreenMarket/GreenMarket.Client/Dockerfile` | Image Client | À créer |
| `GreenMarket/GreenMarket.Client/nginx.conf` | Config nginx WASM | À créer (si WASM) |
| `GreenMarket.API/Program.cs` | Authority/CORS depuis env | À modifier |
| `GreenMarket.Client` config | URLs publiques | À modifier |
| `.gitignore` | Exclure `.env.production` | À modifier |
