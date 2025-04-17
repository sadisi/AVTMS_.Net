using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVTMS.Migrations
{
    /// <inheritdoc />
    public partial class addmodifdicationstonewmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleOwner_VehicleOwnerId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleOwnerId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleOwner",
                table: "VehicleOwner");

            migrationBuilder.DropColumn(
                name: "VehicleOwnerId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "VehicleOwner");

            migrationBuilder.AddColumn<string>(
                name: "VehicleOwnerNIC",
                table: "Vehicles",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NIC",
                table: "VehicleOwner",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleOwner",
                table: "VehicleOwner",
                column: "NIC");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleOwnerNIC",
                table: "Vehicles",
                column: "VehicleOwnerNIC");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleOwner_VehicleOwnerNIC",
                table: "Vehicles",
                column: "VehicleOwnerNIC",
                principalTable: "VehicleOwner",
                principalColumn: "NIC",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleOwner_VehicleOwnerNIC",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleOwnerNIC",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleOwner",
                table: "VehicleOwner");

            migrationBuilder.DropColumn(
                name: "VehicleOwnerNIC",
                table: "Vehicles");

            migrationBuilder.AddColumn<int>(
                name: "VehicleOwnerId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "NIC",
                table: "VehicleOwner",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "VehicleOwner",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleOwner",
                table: "VehicleOwner",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleOwnerId",
                table: "Vehicles",
                column: "VehicleOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleOwner_VehicleOwnerId",
                table: "Vehicles",
                column: "VehicleOwnerId",
                principalTable: "VehicleOwner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
