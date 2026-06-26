using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NatrixServices.Migrations
{
    /// <inheritdoc />
    public partial class ChessUserAddSkin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    MinPlayerCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    MaxPlayerCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    TimePerPlayer = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TotalEventDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Players = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimePerPlayer = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EventId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Fen = table.Column<string>(type: "TEXT", nullable: false),
                    Moves = table.Column<string>(type: "TEXT", nullable: false),
                    MatchResult = table.Column<string>(type: "TEXT", nullable: true),
                    NextPlayer = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerWhite = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerBlack = table.Column<string>(type: "TEXT", nullable: true),
                    DrawOffer = table.Column<string>(type: "TEXT", nullable: true),
                    TimeLeftWhite = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TimeLeftBlack = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastMoveTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    lastClockUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    TitleHolder = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeasonsWon = table.Column<int>(type: "INTEGER", nullable: false),
                    TournamentsWon = table.Column<int>(type: "INTEGER", nullable: false),
                    XP = table.Column<int>(type: "INTEGER", nullable: false),
                    Skin = table.Column<string>(type: "TEXT", nullable: false),
                    Stats = table.Column<string>(type: "TEXT", nullable: false),
                    Invites = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "Events");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
