using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendenceApi.Migrations
{
    public partial class ScheduleIdAddedToLessons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "Lessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ScheduleId",
                table: "Lessons",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Schedules_ScheduleId",
                table: "Lessons",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Schedules_ScheduleId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_ScheduleId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Lessons");
        }
    }
}
