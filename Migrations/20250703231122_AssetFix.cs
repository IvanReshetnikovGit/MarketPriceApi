using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPriceApi.Migrations
{
    /// <inheritdoc />
    public partial class AssetFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Assets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Assets",
                type: "longtext",
                nullable: false);
        }
    }
}
