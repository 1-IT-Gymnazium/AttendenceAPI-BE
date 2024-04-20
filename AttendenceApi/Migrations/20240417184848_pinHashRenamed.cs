using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendenceApi.Migrations
{
    public partial class pinHashRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PinHash",
                table: "AspNetUsers",
                newName: "ParentPin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentPin",
                table: "AspNetUsers",
                newName: "PinHash");
        }
    }
}
