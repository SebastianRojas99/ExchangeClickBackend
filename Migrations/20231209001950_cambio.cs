using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeClick.Migrations
{
    /// <inheritdoc />
    public partial class cambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversionTotal",
                table: "Currencies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConversionTotal",
                table: "Currencies",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
