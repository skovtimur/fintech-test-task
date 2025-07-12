using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintechTestTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDrawProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_it_draw",
                table: "games",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_it_draw",
                table: "games");
        }
    }
}
