using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheRealBest.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "own_goals",
                table: "match_player_stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "own_goals",
                table: "match_player_stats");
        }
    }
}
