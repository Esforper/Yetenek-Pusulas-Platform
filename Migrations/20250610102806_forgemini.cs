using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YetenekPusulasi.Migrations
{
    /// <inheritdoc />
    public partial class forgemini : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScenarioId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_StudentAnswerId",
                table: "AnalysisResults",
                column: "StudentAnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_ScenarioId",
                table: "StudentAnswers",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentId",
                table: "StudentAnswers",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisResults_StudentAnswers_StudentAnswerId",
                table: "AnalysisResults",
                column: "StudentAnswerId",
                principalTable: "StudentAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisResults_StudentAnswers_StudentAnswerId",
                table: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisResults_StudentAnswerId",
                table: "AnalysisResults");
        }
    }
}
