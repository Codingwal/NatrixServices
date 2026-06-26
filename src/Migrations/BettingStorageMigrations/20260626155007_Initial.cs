using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatrixServices.Migrations.BettingStorageMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bets",
                columns: table => new
                {
                    MatchId = table.Column<string>(type: "TEXT", nullable: false),
                    Owner = table.Column<string>(type: "TEXT", nullable: false),
                    ExpectedResult_Player1 = table.Column<uint>(type: "INTEGER", nullable: false),
                    ExpectedResult_Player2 = table.Column<uint>(type: "INTEGER", nullable: false),
                    Stake = table.Column<uint>(type: "INTEGER", nullable: false),
                    Done = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bets", x => new { x.Owner, x.MatchId });
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Event = table.Column<string>(type: "TEXT", nullable: false),
                    Player1 = table.Column<string>(type: "TEXT", nullable: false),
                    Player2 = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Odds_Player1 = table.Column<float>(type: "REAL", nullable: false),
                    Odds_Draw = table.Column<float>(type: "REAL", nullable: false),
                    Odds_Player2 = table.Column<float>(type: "REAL", nullable: false),
                    Result_Player1 = table.Column<uint>(type: "INTEGER", nullable: true),
                    Result_Player2 = table.Column<uint>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bets");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
