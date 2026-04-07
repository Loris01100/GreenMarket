-- TABLE : utilisateur

CREATE TABLE utilisateur (
    keycloak_id     UUID            PRIMARY KEY,
    date_inscription TIMESTAMPTZ    NOT NULL DEFAULT NOW()
);

-- TABLE : producteur
-- Lié 1:1 à utilisateur via utilisateur_id.

CREATE TABLE producteur (
    producteur_id   SERIAL          PRIMARY KEY,
    utilisateur_id  UUID            NOT NULL UNIQUE
                                    REFERENCES utilisateur(keycloak_id)
                                    ON DELETE CASCADE,
    nom_producteur  VARCHAR(200)    NOT NULL,
    adresse         VARCHAR(500)    NOT NULL,
    description     TEXT,
    date_adhesion   TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- TABLE : categorie

CREATE TABLE categorie (
    categorie_id    SERIAL          PRIMARY KEY,
    libelle         VARCHAR(100)    NOT NULL UNIQUE,
    description     TEXT
);

-- TABLE : produit
CREATE TABLE produit (
    produit_id              SERIAL          PRIMARY KEY,
    producteur_id           INT             NOT NULL
                                            REFERENCES producteur(producteur_id)
                                            ON DELETE CASCADE,
    categorie_id            INT             NOT NULL
                                            REFERENCES categorie(categorie_id)
                                            ON DELETE RESTRICT,
    nom                     VARCHAR(200)    NOT NULL,
    description             TEXT,
    prix_unitaire           NUMERIC(10,2)   NOT NULL CHECK (prix_unitaire >= 0),
    score_environnemental   INT             CHECK (score_environnemental BETWEEN 0 AND 100),
    tracabilite             TEXT,
    est_actif               BOOLEAN         NOT NULL DEFAULT TRUE,
    date_creation           TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- TABLE : stock
-- Suivi des quantités disponibles — relation 1:1 avec produit.

CREATE TABLE stock (
    stock_id                SERIAL          PRIMARY KEY,
    produit_id              INT             NOT NULL UNIQUE
                                            REFERENCES produit(produit_id)
                                            ON DELETE CASCADE,
    quantite_disponible     INT             NOT NULL DEFAULT 0
                                            CHECK (quantite_disponible >= 0),
    seuil_alerte            INT             NOT NULL DEFAULT 5
                                            CHECK (seuil_alerte >= 0)
);

-- TABLE : commande

CREATE TABLE commande (
    commande_id     SERIAL          PRIMARY KEY,
    utilisateur_id  UUID            NOT NULL
                                    REFERENCES utilisateur(keycloak_id)
                                    ON DELETE RESTRICT,
    date_commande   TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    montant_total   NUMERIC(10,2)   NOT NULL CHECK (montant_total >= 0),
    statut_paiement VARCHAR(50)     NOT NULL DEFAULT 'en_attente'
                                    CHECK (statut_paiement IN (
                                        'en_attente',
                                        'paye',
                                        'refuse',
                                        'rembourse',
                                        'annule'
                                    ))
);

-- TABLE : ligne_commande
-- Clé primaire composite (commande_id, produit_id).

CREATE TABLE ligne_commande (
    commande_id     INT             NOT NULL
                                    REFERENCES commande(commande_id)
                                    ON DELETE CASCADE,
    producteur_id   INT             NOT NULL
                                    REFERENCES producteur(producteur_id)
                                    ON DELETE RESTRICT,
    produit_id      INT             NOT NULL
                                    REFERENCES produit(produit_id)
                                    ON DELETE RESTRICT,
    quantite        INT             NOT NULL CHECK (quantite > 0),
    prix_unitaire   NUMERIC(10,2)   NOT NULL CHECK (prix_unitaire >= 0),

    PRIMARY KEY (commande_id, produit_id)
);
