using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheRealBest.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchEloRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "away_elo_rating",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "home_elo_rating",
                table: "matches",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "away_elo_rating",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "home_elo_rating",
                table: "matches");
        }
    }
}
