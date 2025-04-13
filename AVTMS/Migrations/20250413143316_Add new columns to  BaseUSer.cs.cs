using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVTMS.Migrations
{
    /// <inheritdoc />
    public partial class AddnewcolumnstoBaseUSercs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByID",
                table: "BaseUser",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "BaseUser",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "BaseUser",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "BaseUser",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "BaseUser");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "BaseUser");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "BaseUser");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "BaseUser");
        }
    }
}
