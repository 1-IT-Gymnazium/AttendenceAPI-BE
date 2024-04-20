using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendenceApi.Migrations
{
    public partial class StartTimeAddedToSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartTimeOfLessonsInMinutes",
                table: "Schedules",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTimeOfLessonsInMinutes",
                table: "Schedules");
        }
    }
}
