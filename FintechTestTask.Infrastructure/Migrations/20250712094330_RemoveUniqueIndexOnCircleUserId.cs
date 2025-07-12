using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FintechTestTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndexOnCircleUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_games_CircleUserId",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_CrossUserId",
                table: "games");

            migrationBuilder.CreateIndex(
                name: "IX_games_CircleUserId",
                table: "games",
                column: "CircleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_games_CrossUserId",
                table: "games",
                column: "CrossUserId");

            migrationBuilder.CreateIndex(
                name: "IX_games_owner_id",
                table: "games",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_games_CircleUserId",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_CrossUserId",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_owner_id",
                table: "games");

            migrationBuilder.CreateIndex(
                name: "IX_games_CircleUserId",
                table: "games",
                column: "CircleUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_CrossUserId",
                table: "games",
                column: "CrossUserId",
                unique: true);
        }
    }
}
