#!/bin/bash
# Exécuté UNE SEULE FOIS au premier démarrage de Postgres (volume vide).
# Crée la base et l'utilisateur dédiés à Keycloak, séparés des données métier.
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER keycloak WITH PASSWORD '${KEYCLOAK_DB_PASSWORD}';
    CREATE DATABASE keycloak_db OWNER keycloak;
    GRANT ALL PRIVILEGES ON DATABASE keycloak_db TO keycloak;
EOSQL
