using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YetenekPusulasi.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRoleSeedFromDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "198a0704-6f8b-46a3-9257-646af52a46c1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88bf2874-d88f-4bfe-8128-c62b8efb2804");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "af3aa1b5-8616-43c9-a428-efdd3dc573be");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "198a0704-6f8b-46a3-9257-646af52a46c1", null, "Admin", "ADMIN" },
                    { "88bf2874-d88f-4bfe-8128-c62b8efb2804", null, "Student", "STUDENT" },
                    { "af3aa1b5-8616-43c9-a428-efdd3dc573be", null, "Teacher", "TEACHER" }
                });
        }
    }
}
