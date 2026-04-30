# Configuration Keycloak — GreenMarket

Ce guide décrit les étapes à effectuer manuellement dans la console Keycloak pour que l'authentification de l'API GreenMarket fonctionne.

**Prérequis :** Docker doit tourner (`docker compose up -d`). Keycloak est accessible sur [http://localhost:8080](http://localhost:8080).

---

## Étape 1 — Connexion à la console admin

1. Ouvrir [http://localhost:8080](http://localhost:8080)
2. Cliquer sur **Administration Console**
3. Se connecter avec **admin / admin**

---

## Étape 2 — Créer le Realm `greenmarket`

1. Cliquer sur le menu déroulant en haut à gauche (**Master**)
2. Cliquer sur **Create realm**
3. **Realm name :** `greenmarket` *(tout en minuscules)*
4. Cliquer sur **Create**

> Toutes les configurations suivantes se font **dans le realm `greenmarket`**.

---

## Étape 3 — Créer le client `greenmarket-api`

1. Aller dans **Clients** → **Create client**
2. **Client ID :** `greenmarket-api`
3. **Name :** `GreenMarket Backend API`
4. Cliquer sur **Next**
5. **Client authentication :** ON *(obligatoire pour le client secret)*
6. **Direct access grants :** ON *(pour les tests via Postman/curl)*
7. **Service accounts roles :** ON *(nécessaire pour que l'API puisse appeler l'Admin API de Keycloak)*
8. Cliquer sur **Next** puis **Save**

### Configurer les URLs

Dans l'onglet **Settings** du client :

| Champ | Valeur |
|-------|--------|
| Valid redirect URIs | `http://localhost:5000/*` |
| Web origins | `*` |

Cliquer sur **Save**.

### Récupérer le Client Secret

1. Aller dans l'onglet **Credentials**
2. Copier la valeur du **Client secret**
3. **Coller cette valeur** dans `appsettings.json` :
   ```json
   "Keycloak": {
     "ClientSecret": "COLLER_ICI"
   }
   ```

---

## Étape 4 — Configurer les permissions du Service Account

Pour que l'API puisse assigner des rôles via l'Admin REST API, le service account du client doit avoir les droits nécessaires.

1. Aller dans **Clients** → **greenmarket-api** → onglet **Service accounts roles**
2. Cliquer sur **Assign role**
3. Dans le filtre, sélectionner **Filter by clients**
4. Rechercher `realm-management`
5. Cocher **manage-users** et **view-users**
6. Cliquer sur **Assign**

---

## Étape 5 — Créer les Rôles Realm

1. Aller dans **Realm roles** → **Create role**

Créer les 3 rôles suivants :

| Role name | Description |
|-----------|-------------|
| `Acheteur` | Utilisateur acheteur (rôle par défaut) |
| `Producteur` | Producteur certifié sur la plateforme |
| `Admin` | Administrateur de la plateforme |

---

## Étape 6 — Configurer le Role Mapper (RBAC)

Pour que les rôles apparaissent dans le token JWT sous la claim `roles` (format attendu par l'API .NET) :

1. Aller dans **Clients** → **greenmarket-api**
2. Onglet **Client scopes** → Cliquer sur **greenmarket-api-dedicated**
3. Cliquer sur **Add mapper** → **By configuration**
4. Sélectionner **User Realm Role**
5. Configurer :

| Champ | Valeur |
|-------|--------|
| Name | `roles` |
| Token Claim Name | `roles` |
| Add to ID token | ON |
| Add to access token | ON |
| Add to userinfo | ON |

6. Cliquer sur **Save**

---

## Étape 7 — Créer les utilisateurs de test

### Acheteur de test

1. Aller dans **Users** → **Add user**
2. Remplir :

| Champ | Valeur |
|-------|--------|
| Username | `test-acheteur` |
| Email | `acheteur@greenmarket.fr` |
| Email verified | ON |
| First name | `Test` |
| Last name | `Acheteur` |

3. Cliquer sur **Create**
4. Onglet **Credentials** → **Set password** → `password` → désactiver **Temporary** → **Save password**
5. Onglet **Role Mapping** → **Assign role** → **Filter by realm roles** → cocher `Acheteur` → **Assign**

### Producteur de test

Répéter la même procédure avec :

| Champ | Valeur |
|-------|--------|
| Username | `test-producteur` |
| Email | `producteur@greenmarket.fr` |
| First name | `Test` |
| Last name | `Producteur` |
| Mot de passe | `password` (non temporaire) |
| Rôle | `Producteur` |

### Admin de test

| Champ | Valeur |
|-------|--------|
| Username | `test-admin` |
| Email | `admin@greenmarket.fr` |
| First name | `Test` |
| Last name | `Admin` |
| Mot de passe | `password` (non temporaire) |
| Rôle | `Admin` |

---

## Étape 8 — Vérifier la configuration (test token)

Obtenir un token de test via curl :

```bash
curl -s -X POST \
  http://localhost:8080/realms/greenmarket/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=greenmarket-api" \
  -d "client_secret=VOTRE_SECRET" \
  -d "username=test-acheteur" \
  -d "password=password" | jq '.access_token'
```

Décoder le token sur [jwt.io](https://jwt.io) et vérifier :
- `iss` : `http://localhost:8080/realms/greenmarket`
- `roles` : `["Acheteur", ...]`
- `preferred_username` : `test-acheteur`

---

## Checklist de validation

- [ ] Realm `greenmarket` créé
- [ ] Client `greenmarket-api` créé avec Client Authentication + Service Accounts activés
- [ ] Client secret copié dans `appsettings.json`
- [ ] Service account a les rôles `manage-users` et `view-users` de `realm-management`
- [ ] Rôles `Acheteur`, `Producteur`, `Admin` créés
- [ ] Role Mapper `roles` configuré sur `greenmarket-api-dedicated`
- [ ] Utilisateurs de test créés avec leurs rôles
- [ ] Token JWT obtenu et décodé avec la claim `roles` visible

---

## Dépannage

| Problème | Cause | Solution |
|----------|-------|----------|
| `401 Unauthorized` sur l'API | Token absent ou expiré | Régénérer un token (durée de vie : 5 min) |
| `403 Forbidden` malgré un token valide | La claim `roles` est absente du token | Vérifier le Role Mapper à l'étape 6, régénérer le token |
| `IDX20803: Unable to obtain configuration` | L'API ne contacte pas Keycloak | Vérifier que `docker compose up -d` tourne et que `Authority` dans `appsettings.json` est correct |
| `Account not fully set up` | `firstName`/`lastName` manquants | Compléter le profil utilisateur dans Keycloak |
| Erreur d'assignation de rôle (403 Admin API) | Service account sans permissions | Vérifier l'étape 4 (manage-users) |
