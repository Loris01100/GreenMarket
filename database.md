# Base de données — Guide de mise en place

## Prérequis

- [Docker](https://www.docker.com/) installé et démarré
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Outil EF Core CLI :

```bash
dotnet tool install --global dotnet-ef
```

---

## 1. Démarrer l'infrastructure

```bash
docker compose up -d
```

Cela lance PostgreSQL (port `5432`) et Keycloak (port `8080`).

---

## 2. Appliquer les migrations

Depuis le dossier `GreenMarket/` (là où se trouve le `.sln`) :

```bash
dotnet ef database update --project GreenMarket.Application --startup-project GreenMarket.API
```

Cela crée le schéma `greenmarket` et toutes les tables dans la base `greenmarket_db`.

> Le schéma `public` est réservé à Keycloak — ne jamais y placer de tables applicatives.

---

## 3. Ajouter une nouvelle migration

Après avoir modifié une entité dans `GreenMarket.Domain/Entities/` :

```bash
dotnet ef migrations add <NomDeLaMigration> --project GreenMarket.Application --startup-project GreenMarket.API
```

Puis appliquer :

```bash
dotnet ef database update --project GreenMarket.Application --startup-project GreenMarket.API
```

---

## 4. Annuler la dernière migration (non appliquée)

```bash
dotnet ef migrations remove --project GreenMarket.Application --startup-project GreenMarket.API
```

---

## 5. Revenir à une migration précédente

```bash
dotnet ef database update <NomDeLaMigration> --project GreenMarket.Application --startup-project GreenMarket.API
```

---

## Chaîne de connexion

Définie dans `GreenMarket.API/appsettings.json` :

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=greenmarket_db;Username=greenmarket;Password=greenmarket"
}
```

---

## Architecture EF Core

| Couche | Rôle |
|---|---|
| `GreenMarket.Domain/Entities/` | Entités C# (POCO) |
| `GreenMarket.Application/Data/GreenMarketDbContext.cs` | DbContext + configuration Fluent API |
| `GreenMarket.Application/Migrations/` | Fichiers de migration générés |
| `GreenMarket.API/Program.cs` | Enregistrement du DbContext dans le DI |
