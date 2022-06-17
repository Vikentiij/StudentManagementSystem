using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManagementSystem.Migrations
{
    public partial class UpdateApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                schema: "security",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "security",
                table: "Users");
        }
    }
}
