using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVTMS.Migrations
{
    /// <inheritdoc />
    public partial class updatevehucledetectmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "VehicleDetects",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "NumberPlate",
                table: "VehicleDetects",
                newName: "license_plate");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "VehicleDetects",
                newName: "end_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "VehicleDetects",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "license_plate",
                table: "VehicleDetects",
                newName: "NumberPlate");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "VehicleDetects",
                newName: "EndTime");
        }
    }
}
