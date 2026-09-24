using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheRealBest.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingEligibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_season_rankings_season_year_overall_rank",
                table: "season_rankings");

            migrationBuilder.DropIndex(
                name: "ix_season_rankings_season_year_position_rank",
                table: "season_rankings");

            migrationBuilder.AddColumn<bool>(
                name: "is_ranking_eligible",
                table: "season_rankings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_season_year_overall_rank",
                table: "season_rankings",
                columns: new[] { "season_year", "overall_rank" },
                filter: "is_ranking_eligible");

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_season_year_position_rank",
                table: "season_rankings",
                columns: new[] { "season_year", "position_rank" },
                filter: "is_ranking_eligible");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_season_rankings_season_year_overall_rank",
                table: "season_rankings");

            migrationBuilder.DropIndex(
                name: "ix_season_rankings_season_year_position_rank",
                table: "season_rankings");

            migrationBuilder.DropColumn(
                name: "is_ranking_eligible",
                table: "season_rankings");

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_season_year_overall_rank",
                table: "season_rankings",
                columns: new[] { "season_year", "overall_rank" });

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_season_year_position_rank",
                table: "season_rankings",
                columns: new[] { "season_year", "position_rank" });
        }
    }
}
