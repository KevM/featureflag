using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace featureflags.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeatureFlags",
                columns: table => new
                {
                    FeatureFlagId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    ForceEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureFlags", x => x.FeatureFlagId);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ScheduleId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Activated = table.Column<DateTime>(nullable: false),
                    Deactivated = table.Column<DateTime>(nullable: true),
                    TrafficPercentage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    Activated = table.Column<DateTime>(nullable: false),
                    Deactivated = table.Column<DateTime>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    MatchExpression = table.Column<string>(nullable: true),
                    FeatureFlagId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_Rules_FeatureFlags_FeatureFlagId",
                        column: x => x.FeatureFlagId,
                        principalTable: "FeatureFlags",
                        principalColumn: "FeatureFlagId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RuleSchedules",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    MatchCount = table.Column<int>(nullable: false),
                    MatchTrafficCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleSchedules", x => new { x.RuleId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_RuleSchedules_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "RuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleSchedules_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_FeatureFlagId",
                table: "Rules",
                column: "FeatureFlagId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleSchedules_ScheduleId",
                table: "RuleSchedules",
                column: "ScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuleSchedules");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "FeatureFlags");
        }
    }
}
