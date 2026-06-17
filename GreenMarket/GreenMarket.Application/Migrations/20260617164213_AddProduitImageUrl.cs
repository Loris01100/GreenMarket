using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenMarket.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddProduitImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                schema: "greenmarket",
                table: "produit",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                schema: "greenmarket",
                table: "produit");
        }
    }
}
