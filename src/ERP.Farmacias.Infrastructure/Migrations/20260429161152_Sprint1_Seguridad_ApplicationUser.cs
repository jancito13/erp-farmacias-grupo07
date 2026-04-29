using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Farmacias.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint1_Seguridad_ApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "security",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "security",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "security",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "security",
                table: "Roles");
        }
    }
}
