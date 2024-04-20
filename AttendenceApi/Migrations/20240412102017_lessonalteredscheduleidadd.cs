using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendenceApi.Migrations
{
    public partial class lessonalteredscheduleidadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Schedules_ScheduleId",
                table: "Lessons");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "Lessons",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "AlteredScheduleId",
                table: "Lessons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_AlteredScheduleId",
                table: "Lessons",
                column: "AlteredScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AlteredSchedules_AlteredScheduleId",
                table: "Lessons",
                column: "AlteredScheduleId",
                principalTable: "AlteredSchedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Schedules_ScheduleId",
                table: "Lessons",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AlteredSchedules_AlteredScheduleId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Schedules_ScheduleId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_AlteredScheduleId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "AlteredScheduleId",
                table: "Lessons");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "Lessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Schedules_ScheduleId",
                table: "Lessons",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
