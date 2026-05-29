using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GreenMarket.Application.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "greenmarket",
                table: "categorie",
                columns: new[] { "categorie_id", "description", "libelle" },
                values: new object[,]
                {
                    { 1, "Légumes frais de saison", "Légumes" },
                    { 2, "Fruits locaux et de saison", "Fruits" },
                    { 3, "Lait, fromage, yaourt, beurre", "Produits laitiers" },
                    { 4, "Oeufs, miel, confitures artisanales", "Produits fermiers" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "greenmarket",
                table: "categorie",
                keyColumn: "categorie_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "greenmarket",
                table: "categorie",
                keyColumn: "categorie_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "greenmarket",
                table: "categorie",
                keyColumn: "categorie_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "greenmarket",
                table: "categorie",
                keyColumn: "categorie_id",
                keyValue: 4);
        }
    }
}
