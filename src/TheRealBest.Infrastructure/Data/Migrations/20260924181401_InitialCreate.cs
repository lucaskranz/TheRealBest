using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheRealBest.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "action_type_translations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    action_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    locale = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    label_plural = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_action_type_translations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "competitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    external_api_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tier = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    tournament_multiplier = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    season_year = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_competitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    external_api_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    primary_position = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scoring_weights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    position = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    action_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    weight_value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_penalty = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scoring_weights", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    external_api_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    short_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    elo_ranking = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "competition_translations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    locale = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    country_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_competition_translations", x => x.id);
                    table.ForeignKey(
                        name: "fk_competition_translations_competitions_competition_id",
                        column: x => x.competition_id,
                        principalTable: "competitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "season_rankings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    season_year = table.Column<int>(type: "integer", nullable: false),
                    total_matches = table.Column<int>(type: "integer", nullable: false),
                    total_minutes = table.Column<int>(type: "integer", nullable: false),
                    mps_average = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    mps_sum_weighted = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    presence_factor = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    fss_score = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    overall_rank = table.Column<int>(type: "integer", nullable: false),
                    position_rank = table.Column<int>(type: "integer", nullable: false),
                    clutch_index = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    top_5_matches = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    recalculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_season_rankings", x => x.id);
                    table.ForeignKey(
                        name: "fk_season_rankings_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    external_api_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    home_team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    away_team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    home_score = table.Column<int>(type: "integer", nullable: true),
                    away_score = table.Column<int>(type: "integer", nullable: true),
                    round_phase = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    match_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_knockout = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matches", x => x.id);
                    table.ForeignKey(
                        name: "fk_matches_competitions_competition_id",
                        column: x => x.competition_id,
                        principalTable: "competitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_matches_teams_away_team_id",
                        column: x => x.away_team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_matches_teams_home_team_id",
                        column: x => x.home_team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "match_player_stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_played = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    minutes_played = table.Column<int>(type: "integer", nullable: false),
                    goals = table.Column<int>(type: "integer", nullable: false),
                    assists = table.Column<int>(type: "integer", nullable: false),
                    shots_total = table.Column<int>(type: "integer", nullable: false),
                    shots_on_target = table.Column<int>(type: "integer", nullable: false),
                    passes_total = table.Column<int>(type: "integer", nullable: false),
                    passes_accurate = table.Column<int>(type: "integer", nullable: false),
                    pass_accuracy = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    key_passes = table.Column<int>(type: "integer", nullable: false),
                    tackles_total = table.Column<int>(type: "integer", nullable: false),
                    interceptions = table.Column<int>(type: "integer", nullable: false),
                    blocks = table.Column<int>(type: "integer", nullable: false),
                    duels_total = table.Column<int>(type: "integer", nullable: false),
                    duels_won = table.Column<int>(type: "integer", nullable: false),
                    aerial_duels_won = table.Column<int>(type: "integer", nullable: false),
                    aerial_duels_total = table.Column<int>(type: "integer", nullable: false),
                    dribbles_attempted = table.Column<int>(type: "integer", nullable: false),
                    dribbles_success = table.Column<int>(type: "integer", nullable: false),
                    dribbled_past = table.Column<int>(type: "integer", nullable: false),
                    fouls_committed = table.Column<int>(type: "integer", nullable: false),
                    fouls_drawn = table.Column<int>(type: "integer", nullable: false),
                    yellow_cards = table.Column<int>(type: "integer", nullable: false),
                    red_cards = table.Column<int>(type: "integer", nullable: false),
                    offsides = table.Column<int>(type: "integer", nullable: false),
                    saves = table.Column<int>(type: "integer", nullable: false),
                    goals_conceded = table.Column<int>(type: "integer", nullable: false),
                    penalties_won = table.Column<int>(type: "integer", nullable: false),
                    penalties_committed = table.Column<int>(type: "integer", nullable: false),
                    penalties_saved = table.Column<int>(type: "integer", nullable: false),
                    penalties_missed = table.Column<int>(type: "integer", nullable: false),
                    clean_sheet = table.Column<bool>(type: "boolean", nullable: false),
                    xg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    xa = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    big_chances_missed = table.Column<int>(type: "integer", nullable: false),
                    big_chances_created = table.Column<int>(type: "integer", nullable: false),
                    errors_leading_to_goal = table.Column<int>(type: "integer", nullable: false),
                    shot_creating_actions = table.Column<int>(type: "integer", nullable: false),
                    progressive_passes = table.Column<int>(type: "integer", nullable: false),
                    progressive_carries = table.Column<int>(type: "integer", nullable: false),
                    ball_recoveries = table.Column<int>(type: "integer", nullable: false),
                    turnovers = table.Column<int>(type: "integer", nullable: false),
                    touches = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_player_stats", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_player_stats_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_match_player_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_match_player_stats_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "match_performance_scores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    match_player_stats_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_evaluated = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    base_score = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    action_breakdown = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    penalty_breakdown = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    subtotal_raw = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    tournament_multiplier = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    opponent_multiplier = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    clutch_multiplier = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    context_multiplier_combined = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    minutes_factor = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    final_mps = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    algorithm_version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_performance_scores", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_performance_scores_match_player_stats_match_player_st",
                        column: x => x.match_player_stats_id,
                        principalTable: "match_player_stats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_match_performance_scores_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_match_performance_scores_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_action_type_translations_action_key_locale",
                table: "action_type_translations",
                columns: new[] { "action_key", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_competition_translations_competition_id_locale",
                table: "competition_translations",
                columns: new[] { "competition_id", "locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_competitions_external_api_id_season_year",
                table: "competitions",
                columns: new[] { "external_api_id", "season_year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_match_performance_scores_match_id",
                table: "match_performance_scores",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_performance_scores_match_player_stats_id",
                table: "match_performance_scores",
                column: "match_player_stats_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_match_performance_scores_player_id_algorithm_version",
                table: "match_performance_scores",
                columns: new[] { "player_id", "algorithm_version" });

            migrationBuilder.CreateIndex(
                name: "ix_match_performance_scores_player_id_match_id",
                table: "match_performance_scores",
                columns: new[] { "player_id", "match_id" });

            migrationBuilder.CreateIndex(
                name: "ix_match_player_stats_match_id_player_id",
                table: "match_player_stats",
                columns: new[] { "match_id", "player_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_match_player_stats_player_id",
                table: "match_player_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_player_stats_team_id",
                table: "match_player_stats",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_matches_away_team_id",
                table: "matches",
                column: "away_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_matches_competition_id_match_date",
                table: "matches",
                columns: new[] { "competition_id", "match_date" });

            migrationBuilder.CreateIndex(
                name: "ix_matches_external_api_id",
                table: "matches",
                column: "external_api_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_matches_home_team_id",
                table: "matches",
                column: "home_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_external_api_id",
                table: "players",
                column: "external_api_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_players_name",
                table: "players",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_players_primary_position",
                table: "players",
                column: "primary_position");

            migrationBuilder.CreateIndex(
                name: "ix_scoring_weights_position_action_type_version",
                table: "scoring_weights",
                columns: new[] { "position", "action_type", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_player_id_season_year",
                table: "season_rankings",
                columns: new[] { "player_id", "season_year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_season_year_overall_rank",
                table: "season_rankings",
                columns: new[] { "season_year", "overall_rank" });

            migrationBuilder.CreateIndex(
                name: "ix_season_rankings_season_year_position_rank",
                table: "season_rankings",
                columns: new[] { "season_year", "position_rank" });

            migrationBuilder.CreateIndex(
                name: "ix_teams_external_api_id",
                table: "teams",
                column: "external_api_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "action_type_translations");

            migrationBuilder.DropTable(
                name: "competition_translations");

            migrationBuilder.DropTable(
                name: "match_performance_scores");

            migrationBuilder.DropTable(
                name: "scoring_weights");

            migrationBuilder.DropTable(
                name: "season_rankings");

            migrationBuilder.DropTable(
                name: "match_player_stats");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "competitions");

            migrationBuilder.DropTable(
                name: "teams");
        }
    }
}
