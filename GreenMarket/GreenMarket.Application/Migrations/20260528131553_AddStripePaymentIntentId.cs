using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenMarket.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentIntentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                schema: "greenmarket",
                table: "commande",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                schema: "greenmarket",
                table: "commande");
        }
    }
}
