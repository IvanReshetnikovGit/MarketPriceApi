using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPriceApi.Migrations
{
    /// <inheritdoc />
    public partial class AssetReimagined : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPrice",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Assets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastPrice",
                table: "Assets",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Assets",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
