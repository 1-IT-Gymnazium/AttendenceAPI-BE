using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendenceApi.Migrations
{
    public partial class parityadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Parity",
                table: "Lessons",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parity",
                table: "Lessons");
        }
    }
}
