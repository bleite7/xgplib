using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XgpLib.SyncService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "genres",
                table: "games",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "genres",
                table: "games");
        }
    }
}
