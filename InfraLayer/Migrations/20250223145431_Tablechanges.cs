using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfraLayer.Migrations
{
    public partial class Tablechanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_TaskAssignedToId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskAssignedToId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TaskAssignedToId",
                table: "Tasks",
                newName: "TaskSize");

            migrationBuilder.AddColumn<DateTime>(
                name: "UserDOB",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TaskAssignedToUserId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaskStatus",
                table: "Tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDOB",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaskAssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskStatus",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TaskSize",
                table: "Tasks",
                newName: "TaskAssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskAssignedToId",
                table: "Tasks",
                column: "TaskAssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_TaskAssignedToId",
                table: "Tasks",
                column: "TaskAssignedToId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
