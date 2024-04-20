using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendenceApi.Migrations
{
    public partial class classroomremoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Classrooms_RoomId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_RoomId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "Lessons");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Lessons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_RoomId",
                table: "Lessons",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Classrooms_RoomId",
                table: "Lessons",
                column: "RoomId",
                principalTable: "Classrooms",
                principalColumn: "Id");
        }
    }
}
