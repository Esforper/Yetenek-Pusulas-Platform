using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YetenekPusulasi.Migrations
{
    /// <inheritdoc />
    public partial class bircaciklar4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreativeThinkingScenario_OptionsProvided",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionMakingScenario_OptionsProvided",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpathyContexts",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedSolutionSteps",
                table: "Scenarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionsProvided",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemContext",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScenarioDiscriminator",
                table: "Scenarios",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Classrooms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreativeThinkingScenario_OptionsProvided",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "DecisionMakingScenario_OptionsProvided",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "EmpathyContexts",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "ExpectedSolutionSteps",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "OptionsProvided",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "ProblemContext",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "ScenarioDiscriminator",
                table: "Scenarios");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Classrooms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
