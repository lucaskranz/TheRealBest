using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheRealBest.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScoringEngineFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "penalties_scored",
                table: "match_player_stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "counts_towards_season",
                table: "match_performance_scores",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "position_baseline",
                table: "match_performance_scores",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "penalties_scored",
                table: "match_player_stats");

            migrationBuilder.DropColumn(
                name: "counts_towards_season",
                table: "match_performance_scores");

            migrationBuilder.DropColumn(
                name: "position_baseline",
                table: "match_performance_scores");
        }
    }
}
