using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwimAnalytics.Data.Migrations;

/// <inheritdoc />
public partial class _20261120174503_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Clubs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clubs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Events",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Stroke = table.Column<int>(type: "int", nullable: false),
                DistanceMeters = table.Column<int>(type: "int", nullable: false),
                Gender = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Events", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Meets",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                PoolLength = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Meets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Swimmers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                Gender = table.Column<int>(type: "int", nullable: false),
                ClubId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Swimmers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Swimmers_Clubs_ClubId",
                    column: x => x.ClubId,
                    principalTable: "Clubs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "BaseTimes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BaseTimeMs = table.Column<int>(type: "int", nullable: false),
                PoolLength = table.Column<int>(type: "int", nullable: false),
                EventId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BaseTimes", x => x.Id);
                table.ForeignKey(
                    name: "FK_BaseTimes_Events_EventId",
                    column: x => x.EventId,
                    principalTable: "Events",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Results",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TimeMs = table.Column<int>(type: "int", nullable: false),
                SwimmerId = table.Column<int>(type: "int", nullable: false),
                EventId = table.Column<int>(type: "int", nullable: false),
                MeetId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Results", x => x.Id);
                table.ForeignKey(
                    name: "FK_Results_Events_EventId",
                    column: x => x.EventId,
                    principalTable: "Events",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Results_Meets_MeetId",
                    column: x => x.MeetId,
                    principalTable: "Meets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Results_Swimmers_SwimmerId",
                    column: x => x.SwimmerId,
                    principalTable: "Swimmers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Splits",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DistanceMeters = table.Column<int>(type: "int", nullable: false),
                CumulativeTimeMs = table.Column<int>(type: "int", nullable: false),
                ResultId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Splits", x => x.Id);
                table.ForeignKey(
                    name: "FK_Splits_Results_ResultId",
                    column: x => x.ResultId,
                    principalTable: "Results",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BaseTimes_EventId_PoolLength",
            table: "BaseTimes",
            columns: new[] { "EventId", "PoolLength" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Events_Stroke_DistanceMeters_Gender",
            table: "Events",
            columns: new[] { "Stroke", "DistanceMeters", "Gender" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Meets_Date",
            table: "Meets",
            column: "Date");

        migrationBuilder.CreateIndex(
            name: "IX_Results_EventId_TimeMs",
            table: "Results",
            columns: new[] { "EventId", "TimeMs" });

        migrationBuilder.CreateIndex(
            name: "IX_Results_MeetId",
            table: "Results",
            column: "MeetId");

        migrationBuilder.CreateIndex(
            name: "IX_Results_SwimmerId_EventId",
            table: "Results",
            columns: new[] { "SwimmerId", "EventId" });

        migrationBuilder.CreateIndex(
            name: "IX_Splits_ResultId_DistanceMeters",
            table: "Splits",
            columns: new[] { "ResultId", "DistanceMeters" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Swimmers_ClubId",
            table: "Swimmers",
            column: "ClubId");

        migrationBuilder.CreateIndex(
            name: "IX_Swimmers_LastName_FirstName",
            table: "Swimmers",
            columns: new[] { "LastName", "FirstName" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "BaseTimes");

        migrationBuilder.DropTable(
            name: "Splits");

        migrationBuilder.DropTable(
            name: "Results");

        migrationBuilder.DropTable(
            name: "Events");

        migrationBuilder.DropTable(
            name: "Meets");

        migrationBuilder.DropTable(
            name: "Swimmers");

        migrationBuilder.DropTable(
            name: "Clubs");
    }
}
