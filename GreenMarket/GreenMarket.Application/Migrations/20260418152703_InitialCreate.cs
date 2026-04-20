using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GreenMarket.Application.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "greenmarket");

            migrationBuilder.CreateTable(
                name: "categorie",
                schema: "greenmarket",
                columns: table => new
                {
                    categorie_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorie", x => x.categorie_id);
                });

            migrationBuilder.CreateTable(
                name: "utilisateur",
                schema: "greenmarket",
                columns: table => new
                {
                    keycloak_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_inscription = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateur", x => x.keycloak_id);
                });

            migrationBuilder.CreateTable(
                name: "commande",
                schema: "greenmarket",
                columns: table => new
                {
                    commande_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    utilisateur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_commande = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    montant_total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    statut_paiement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commande", x => x.commande_id);
                    table.CheckConstraint("CK_commande_montant_total", "montant_total >= 0");
                    table.CheckConstraint("CK_commande_statut_paiement", "statut_paiement IN ('en_attente', 'paye', 'refuse', 'rembourse', 'annule')");
                    table.ForeignKey(
                        name: "FK_commande_utilisateur_utilisateur_id",
                        column: x => x.utilisateur_id,
                        principalSchema: "greenmarket",
                        principalTable: "utilisateur",
                        principalColumn: "keycloak_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "producteur",
                schema: "greenmarket",
                columns: table => new
                {
                    producteur_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    utilisateur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom_producteur = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    adresse = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    date_adhesion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producteur", x => x.producteur_id);
                    table.ForeignKey(
                        name: "FK_producteur_utilisateur_utilisateur_id",
                        column: x => x.utilisateur_id,
                        principalSchema: "greenmarket",
                        principalTable: "utilisateur",
                        principalColumn: "keycloak_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "produit",
                schema: "greenmarket",
                columns: table => new
                {
                    produit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    producteur_id = table.Column<int>(type: "integer", nullable: false),
                    categorie_id = table.Column<int>(type: "integer", nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    prix_unitaire = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    score_environnemental = table.Column<int>(type: "integer", nullable: true),
                    tracabilite = table.Column<string>(type: "text", nullable: true),
                    est_actif = table.Column<bool>(type: "boolean", nullable: false),
                    date_creation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_produit", x => x.produit_id);
                    table.CheckConstraint("CK_produit_prix_unitaire", "prix_unitaire >= 0");
                    table.CheckConstraint("CK_produit_score_environnemental", "score_environnemental BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "FK_produit_categorie_categorie_id",
                        column: x => x.categorie_id,
                        principalSchema: "greenmarket",
                        principalTable: "categorie",
                        principalColumn: "categorie_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_produit_producteur_producteur_id",
                        column: x => x.producteur_id,
                        principalSchema: "greenmarket",
                        principalTable: "producteur",
                        principalColumn: "producteur_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ligne_commande",
                schema: "greenmarket",
                columns: table => new
                {
                    commande_id = table.Column<int>(type: "integer", nullable: false),
                    produit_id = table.Column<int>(type: "integer", nullable: false),
                    producteur_id = table.Column<int>(type: "integer", nullable: false),
                    quantite = table.Column<int>(type: "integer", nullable: false),
                    prix_unitaire = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ligne_commande", x => new { x.commande_id, x.produit_id });
                    table.CheckConstraint("CK_ligne_commande_prix_unitaire", "prix_unitaire >= 0");
                    table.CheckConstraint("CK_ligne_commande_quantite", "quantite > 0");
                    table.ForeignKey(
                        name: "FK_ligne_commande_commande_commande_id",
                        column: x => x.commande_id,
                        principalSchema: "greenmarket",
                        principalTable: "commande",
                        principalColumn: "commande_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ligne_commande_producteur_producteur_id",
                        column: x => x.producteur_id,
                        principalSchema: "greenmarket",
                        principalTable: "producteur",
                        principalColumn: "producteur_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ligne_commande_produit_produit_id",
                        column: x => x.produit_id,
                        principalSchema: "greenmarket",
                        principalTable: "produit",
                        principalColumn: "produit_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock",
                schema: "greenmarket",
                columns: table => new
                {
                    stock_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    produit_id = table.Column<int>(type: "integer", nullable: false),
                    quantite_disponible = table.Column<int>(type: "integer", nullable: false),
                    seuil_alerte = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock", x => x.stock_id);
                    table.CheckConstraint("CK_stock_quantite_disponible", "quantite_disponible >= 0");
                    table.CheckConstraint("CK_stock_seuil_alerte", "seuil_alerte >= 0");
                    table.ForeignKey(
                        name: "FK_stock_produit_produit_id",
                        column: x => x.produit_id,
                        principalSchema: "greenmarket",
                        principalTable: "produit",
                        principalColumn: "produit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categorie_libelle",
                schema: "greenmarket",
                table: "categorie",
                column: "libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_commande_utilisateur_id",
                schema: "greenmarket",
                table: "commande",
                column: "utilisateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_commande_producteur_id",
                schema: "greenmarket",
                table: "ligne_commande",
                column: "producteur_id");

            migrationBuilder.CreateIndex(
                name: "IX_ligne_commande_produit_id",
                schema: "greenmarket",
                table: "ligne_commande",
                column: "produit_id");

            migrationBuilder.CreateIndex(
                name: "IX_producteur_utilisateur_id",
                schema: "greenmarket",
                table: "producteur",
                column: "utilisateur_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_produit_categorie_id",
                schema: "greenmarket",
                table: "produit",
                column: "categorie_id");

            migrationBuilder.CreateIndex(
                name: "IX_produit_producteur_id",
                schema: "greenmarket",
                table: "produit",
                column: "producteur_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_produit_id",
                schema: "greenmarket",
                table: "stock",
                column: "produit_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ligne_commande",
                schema: "greenmarket");

            migrationBuilder.DropTable(
                name: "stock",
                schema: "greenmarket");

            migrationBuilder.DropTable(
                name: "commande",
                schema: "greenmarket");

            migrationBuilder.DropTable(
                name: "produit",
                schema: "greenmarket");

            migrationBuilder.DropTable(
                name: "categorie",
                schema: "greenmarket");

            migrationBuilder.DropTable(
                name: "producteur",
                schema: "greenmarket");

            migrationBuilder.DropTable(
                name: "utilisateur",
                schema: "greenmarket");
        }
    }
}
