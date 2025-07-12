using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintechTestTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityConfWasChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_games_users_CircleUserId",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "FK_games_users_CrossUserId",
                table: "games");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_CircleUser",
                table: "games",
                column: "CircleUserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_CrossUser",
                table: "games",
                column: "CrossUserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_CircleUser",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_CrossUser",
                table: "games");

            migrationBuilder.AddForeignKey(
                name: "FK_games_users_CircleUserId",
                table: "games",
                column: "CircleUserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_games_users_CrossUserId",
                table: "games",
                column: "CrossUserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
