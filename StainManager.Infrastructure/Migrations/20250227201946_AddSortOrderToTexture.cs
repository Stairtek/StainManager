using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrderToTexture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Textures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Textures");
        }
    }
}
