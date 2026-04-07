# Répartition du travail — GreenMarket

## Équipe

| Dev | Nom | Domaine fonctionnel |
|-----|------|---------------------|
| Dev 1 | À compléter | Utilisateur + Producteur + Auth Keycloak |
| Dev 2 | À compléter | Produit + Catégorie + Stock |
| Dev 3 | À compléter | Commande + Ligne de commande + Paiement Stripe |

---

### Dev 1 — `feature/utilisateur-producteur`

**Périmètre fonctionnel :** F1 (Gestion des comptes et authentification)

#### Domain

| Fichier | Dossier |
|---------|---------|
| `Utilisateur.cs` | `Entities/` |
| `Producteur.cs` | `Entities/` |
| `IUtilisateurRepository.cs` | `Interfaces/` |
| `IProducteurRepository.cs` | `Interfaces/` |

#### Application

| Fichier | Dossier |
|---------|---------|
| `GetUtilisateurQuery.cs` | `UseCases/Utilisateurs/` |
| `CreateProducteurCommand.cs` | `UseCases/Producteurs/` |
| `GetProducteurByIdQuery.cs` | `UseCases/Producteurs/` |
| `GetProducteursQuery.cs` | `UseCases/Producteurs/` |
| `IKeycloakService.cs` | `Interfaces/` |

#### Shared

| Fichier | Dossier |
|---------|---------|
| `ProducteurDto.cs` | `DTOs/` |
| `ProducteurCreateDto.cs` | `DTOs/` |

#### API

| Fichier | Dossier |
|---------|---------|
| `UtilisateurConfiguration.cs` | `Data/Configurations/` |
| `ProducteurConfiguration.cs` | `Data/Configurations/` |
| `UtilisateurRepository.cs` | `Repositories/` |
| `ProducteurRepository.cs` | `Repositories/` |
| `ProducteursController.cs` | `Controllers/` |
| `KeycloakService.cs` | `Services/` |

#### Migration

```bash
dotnet ef migrations add AddUtilisateurProducteur --project src/GreenMarket.API
```

#### DbSet à ajouter dans GreenMarketDbContext

```csharp
public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
public DbSet<Producteur> Producteurs => Set<Producteur>();
```

---

### Dev 2 — `feature/produit-catalogue-stock`

**Périmètre fonctionnel :** F2 (Consultation produits), F5 (Catalogue producteur), F6 (Stocks)

#### Domain

| Fichier | Dossier |
|---------|---------|
| `Produit.cs` | `Entities/` |
| `Categorie.cs` | `Entities/` |
| `Stock.cs` | `Entities/` |
| `IProduitRepository.cs` | `Interfaces/` |
| `ICategorieRepository.cs` | `Interfaces/` |
| `IStockRepository.cs` | `Interfaces/` |
| `StockInsuffisantException.cs` | `Exceptions/` |
| `ProduitInactifException.cs` | `Exceptions/` |

#### Application

| Fichier | Dossier |
|---------|---------|
| `GetProduitsQuery.cs` | `UseCases/Produits/` |
| `GetProduitByIdQuery.cs` | `UseCases/Produits/` |
| `CreateProduitCommand.cs` | `UseCases/Produits/` |
| `UpdateProduitCommand.cs` | `UseCases/Produits/` |
| `UpdateStockCommand.cs` | `UseCases/Stocks/` |
| `GetStockByProduitQuery.cs` | `UseCases/Stocks/` |

#### Shared

| Fichier | Dossier |
|---------|---------|
| `ProduitDto.cs` | `DTOs/` |
| `ProduitCreateDto.cs` | `DTOs/` |
| `StockDto.cs` | `DTOs/` |
| `CategorieDto.cs` | `DTOs/` |

#### API

| Fichier | Dossier |
|---------|---------|
| `ProduitConfiguration.cs` | `Data/Configurations/` |
| `CategorieConfiguration.cs` | `Data/Configurations/` |
| `StockConfiguration.cs` | `Data/Configurations/` |
| `ProduitRepository.cs` | `Repositories/` |
| `CategorieRepository.cs` | `Repositories/` |
| `StockRepository.cs` | `Repositories/` |
| `ProduitsController.cs` | `Controllers/` |
| `CategoriesController.cs` | `Controllers/` |
| `StocksController.cs` | `Controllers/` |

#### Migration

```bash
dotnet ef migrations add AddProduitCategorieStock --project src/GreenMarket.API
```

#### DbSet à ajouter dans GreenMarketDbContext

```csharp
public DbSet<Produit> Produits => Set<Produit>();
public DbSet<Categorie> Categories => Set<Categorie>();
public DbSet<Stock> Stocks => Set<Stock>();
```

#### Seed des catégories (dans la Configuration ou dans la migration)

```csharp
builder.HasData(
    new Categorie { CategorieId = 1, Libelle = "Légumes", Description = "Légumes frais de saison" },
    new Categorie { CategorieId = 2, Libelle = "Fruits", Description = "Fruits locaux et de saison" },
    new Categorie { CategorieId = 3, Libelle = "Produits laitiers", Description = "Lait, fromage, yaourt, beurre" },
    new Categorie { CategorieId = 4, Libelle = "Produits fermiers", Description = "Oeufs, miel, confitures artisanales" }
);
```

---

### Dev 3 — `feature/commande-paiement`

**Périmètre fonctionnel :** F3 (Panier et commandes), F4 (Paiement sécurisé)

#### Domain

| Fichier | Dossier |
|---------|---------|
| `Commande.cs` | `Entities/` |
| `LigneCommande.cs` | `Entities/` |
| `ICommandeRepository.cs` | `Interfaces/` |

#### Application

| Fichier | Dossier |
|---------|---------|
| `CreerCommandeCommand.cs` | `UseCases/Commandes/` |
| `GetCommandesUtilisateurQuery.cs` | `UseCases/Commandes/` |
| `GetCommandesProducteurQuery.cs` | `UseCases/Commandes/` |
| `ValiderPaiementCommand.cs` | `UseCases/Commandes/` |
| `IPaiementService.cs` | `Interfaces/` |

#### Shared

| Fichier | Dossier |
|---------|---------|
| `CommandeDto.cs` | `DTOs/` |
| `CommandeCreateDto.cs` | `DTOs/` |
| `LigneCommandeDto.cs` | `DTOs/` |

#### API

| Fichier | Dossier |
|---------|---------|
| `CommandeConfiguration.cs` | `Data/Configurations/` |
| `LigneCommandeConfiguration.cs` | `Data/Configurations/` |
| `CommandeRepository.cs` | `Repositories/` |
| `CommandesController.cs` | `Controllers/` |
| `StripeService.cs` | `Services/` |

#### Migration

```bash
dotnet ef migrations add AddCommandeLigneCommande --project src/GreenMarket.API
```

#### DbSet à ajouter dans GreenMarketDbContext

```csharp
public DbSet<Commande> Commandes => Set<Commande>();
public DbSet<LigneCommande> LignesCommande => Set<LigneCommande>();
```

## Gestion du DbContext partagé

Le fichier `GreenMarketDbContext.cs` est touché par les trois développeurs. Pour que ça se passe bien :

- Chacun ajoute **uniquement ses DbSet** et son `ApplyConfiguration` dans `OnModelCreating`
- Les DbSet sont des **ajouts de lignes** (pas de modification) → le merge est simple
- L'ordre de merge recommandé : Dev 1 d'abord, puis Dev 2, puis Dev 3 (Dev 3 rebase avant de merger)
- Chaque migration a un **nom distinct** → EF Core les chaîne automatiquement

---

## Ordre de merge dans `main`

```
Dev 1  ──→  merge feature/utilisateur-producteur     (aucune dépendance)
Dev 2  ──→  merge feature/produit-catalogue-stock     (aucune dépendance)
Dev 3  ──→  rebase sur main → merge feature/commande-paiement  (dépend des entités de Dev 1 et Dev 2)
```

Dev 1 et Dev 2 peuvent merger dans n'importe quel ordre.
Dev 3 merge en dernier car ses FK pointent vers les entités des deux autres.

| Dev | Tâche | Branche |
|-----|-------|---------|
| Dev 1 | Front Blazor : pages auth, espace producteur | `feature/client-auth-producteur` |
| Dev 2 | Front Blazor : catalogue, fiche produit, panier | `feature/client-catalogue-panier` |
| Dev 3 | Reporting (F7) + Back-office admin (F8) | `feature/reporting-admin` |
