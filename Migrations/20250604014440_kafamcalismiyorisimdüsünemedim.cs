using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YetenekPusulasi.Migrations
{
    /// <inheritdoc />
    public partial class kafamcalismiyorisimdüsünemedim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitialPrompt",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WasInitialPromptAIGenerated",
                table: "Scenarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AnalysisResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentAnswerId = table.Column<int>(type: "int", nullable: false),
                    AiModelUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallScore = table.Column<double>(type: "float", nullable: true),
                    DetectedSkills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawAiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnalysisDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "InitialPrompt",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "WasInitialPromptAIGenerated",
                table: "Scenarios");
        }
    }
}
