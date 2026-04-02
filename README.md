# GreenMarket

![CI](https://github.com/<org>/GreenMarket/actions/workflows/ci.yml/badge.svg)

Application web de marché en ligne développée avec .NET 10, Blazor et Keycloak.

## Stack technique

| Couche | Technologie |
|---|---|
| Framework | .NET 10 |
| Frontend | Blazor |
| Backend | ASP.NET Core — Minimal APIs |
| Base de données | PostgreSQL 18 (EF Core Code First) |
| Authentification | Keycloak 26.2 (OIDC / JWT) |
| Infrastructure locale | Docker Compose |
| Tests | xUnit + coverlet |

## Structure de la solution

```
GreenMarket/
├── GreenMarket.API/          # Endpoints HTTP, configuration DI, middleware
├── GreenMarket.Application/  # Cas d'usage, services, interfaces
├── GreenMarket.Domain/       # Entités, value objects, règles métier
├── GreenMarket.Client/       # Frontend Blazor
├── GreenMarket.Shared/       # DTOs partagés API ↔ Client
└── GreenMarket.Tests/        # Tests unitaires et d'intégration (xUnit)
```

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Démarrage rapide

### 1. Lancer l'infrastructure

```bash
docker compose up -d
```

| Service | URL | Identifiants |
|---|---|---|
| PostgreSQL | `localhost:5432` | `greenmarket` / `greenmarket` |
| Keycloak | `http://localhost:8080` | `admin` / `admin` |

### 2. Appliquer les migrations EF Core

```bash
dotnet ef database update \
  --project GreenMarket/GreenMarket.Application \
  --startup-project GreenMarket/GreenMarket.API
```

### 3. Lancer l'application

```bash
# API
dotnet run --project GreenMarket/GreenMarket.API

# Client Blazor (dans un second terminal)
dotnet run --project GreenMarket/GreenMarket.Client
```

### 4. Lancer les tests

```bash
dotnet test GreenMarket/GreenMarket.sln
```

## Migrations EF Core

```bash
# Créer une nouvelle migration
dotnet ef migrations add <NomMigration> \
  --project GreenMarket/GreenMarket.Application \
  --startup-project GreenMarket/GreenMarket.API

# Appliquer les migrations
dotnet ef database update \
  --project GreenMarket/GreenMarket.Application \
  --startup-project GreenMarket/GreenMarket.API

# Annuler la dernière migration
dotnet ef migrations remove \
  --project GreenMarket/GreenMarket.Application \
  --startup-project GreenMarket/GreenMarket.API
```

## CI/CD

Le pipeline GitHub Actions ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)) s'exécute à chaque push et pull request sur `main` et `develop` :

1. **Build** — compilation en mode Release
2. **Test** — exécution des tests xUnit avec collecte de la couverture de code
3. **Artifact** — upload du rapport de couverture

> Le job CI démarre automatiquement un service PostgreSQL éphémère pour les tests d'intégration.

## Configuration

La connexion à la base de données et les paramètres Keycloak sont définis dans `appsettings.json` et surchargés par des variables d'environnement en CI/production.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=greenmarket_db;Username=greenmarket;Password=greenmarket"
  },
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/<realm>",
    "ClientId": "<client-id>"
  }
}
```

Ne jamais committer de secrets — utiliser les [GitHub Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets) en CI et un gestionnaire de secrets en production.
