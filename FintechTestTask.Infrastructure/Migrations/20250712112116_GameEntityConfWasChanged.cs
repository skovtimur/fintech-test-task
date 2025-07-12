using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintechTestTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameEntityConfWasChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "winner_player",
                table: "games");

            migrationBuilder.AddColumn<Guid>(
                name: "winner_player_id",
                table: "games",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "winner_player_id",
                table: "games");

            migrationBuilder.AddColumn<int>(
                name: "winner_player",
                table: "games",
                type: "integer",
                nullable: true);
        }
    }
}
