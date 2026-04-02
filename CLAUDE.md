# GreenMarket — Guide Claude Code

## Stack technique

| Couche | Technologie |
|---|---|
| Framework | .NET 10 |
| Frontend | Blazor (GreenMarket.Client) |
| Backend / API | ASP.NET Core minimal API (GreenMarket.API) |
| Base de données | PostgreSQL 18 — accès via Entity Framework Core (Code First) |
| Authentification | Keycloak 26.2 |
| Infrastructure locale | Docker Compose |

## Architecture de la solution

```
GreenMarket/
├── GreenMarket.sln
├── GreenMarket.API/          # Point d'entrée HTTP, controllers/endpoints, config DI
├── GreenMarket.Application/  # Cas d'usage, services applicatifs, interfaces
├── GreenMarket.Domain/       # Entités, value objects, règles métier pures
├── GreenMarket.Client/       # Blazor WebAssembly / Server (frontend)
├── GreenMarket.Shared/       # DTOs et contrats partagés API ↔ Client
└── GreenMarket.Tests/        # Tests unitaires et d'intégration
```

Architecture en couches (Clean Architecture) :
- **Domain** n'a aucune dépendance externe.
- **Application** dépend de Domain.
- **API** dépend de Application et Shared.
- **Client** dépend uniquement de Shared.

## Infrastructure locale (Docker Compose)

Lancer l'environnement :
```bash
docker compose up -d
```

| Service | URL locale | Identifiants |
|---|---|---|
| PostgreSQL | `localhost:5432` | user: `greenmarket` / pass: `greenmarket` / db: `greenmarket_db` |
| Keycloak | `http://localhost:8080` | admin / admin |

## Base de données — Code First (EF Core)

- Les entités sont définies dans **GreenMarket.Domain**.
- Le `DbContext` et les configurations Fluent API sont dans **GreenMarket.Application** ou **GreenMarket.Infrastructure** (selon l'évolution du projet).
- Commandes courantes :

```bash
# Ajouter une migration
dotnet ef migrations add <NomMigration> --project GreenMarket.Application --startup-project GreenMarket.API

# Appliquer les migrations
dotnet ef database update --project GreenMarket.Application --startup-project GreenMarket.API
```

## Authentification — Keycloak

- Keycloak gère les utilisateurs, les rôles et l'émission des tokens JWT (OIDC).
- L'API valide les tokens via le middleware ASP.NET Core (`AddAuthentication().AddJwtBearer()`).
- Le realm et le client Keycloak doivent être configurés manuellement ou via import de realm.
- Ne jamais coder en dur les secrets Keycloak : utiliser `appsettings.json` (section `Keycloak`) ou des variables d'environnement.

## Conventions de développement

- **Nullable enable** activé sur tous les projets — gérer explicitement les nulls.
- **ImplicitUsings enable** — pas besoin de répéter les `using` standards.
- Les DTOs et contrats d'API vivent dans **GreenMarket.Shared** pour être partagés entre API et Client.
- Préférer les **Minimal APIs** dans `Program.cs` ou des fichiers d'extension `*.Endpoints.cs`.
- Utiliser les **records** pour les DTOs immuables.

## Lancer le projet

```bash
# Infrastructure
docker compose up -d

# API
dotnet run --project GreenMarket/GreenMarket.API

# Client Blazor
dotnet run --project GreenMarket/GreenMarket.Client

# Tests
dotnet test GreenMarket/GreenMarket.Tests
```

## Points d'attention

- S'assurer que Docker est démarré avant de lancer l'API (dépendance PostgreSQL).
- Keycloak en mode `start-dev` : ne pas utiliser en production.
- Le schéma Keycloak est stocké dans la même base PostgreSQL (`greenmarket_db`).
