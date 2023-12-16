using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeClick.Migrations
{
    /// <inheritdoc />
    public partial class newchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conversions");

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionTotal",
                table: "Currencies",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Users_UserId",
                table: "Currencies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Users_UserId",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "ConversionTotal",
                table: "Currencies");

            migrationBuilder.CreateTable(
                name: "Conversions",
                columns: table => new
                {
                    IdConversion = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromCurrencyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToCurrencyId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConversionTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    DateConversion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FromCurrency = table.Column<decimal>(type: "TEXT", nullable: false),
                    ToCurrency = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversions", x => x.IdConversion);
                    table.ForeignKey(
                        name: "FK_Conversions_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversions_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversions_FromCurrencyId",
                table: "Conversions",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversions_ToCurrencyId",
                table: "Conversions",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversions_UserId",
                table: "Conversions",
                column: "UserId");
        }
    }
}
