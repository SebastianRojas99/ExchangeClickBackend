using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeClick.Migrations
{
    /// <inheritdoc />
    public partial class ready : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubCount",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "SubCount",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubCount",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "SubCount",
                table: "Subscriptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
