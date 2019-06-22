using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Laborator2.Migrations
{
    public partial class AddUserRoleStartDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UserRoleStartDate",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRoleStartDate",
                table: "Users");
        }
    }
}
