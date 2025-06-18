using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.ProTrack.Migrations
{
    /// <inheritdoc />
    public partial class modifyingProjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_AssignedUserId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "Projects",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_AssignedUserId",
                table: "Projects",
                newName: "IX_Projects_ManagerId");

            migrationBuilder.AlterColumn<int>(
                name: "UserRole",
                table: "ProjectUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId1",
                table: "ProjectUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectId1",
                table: "ProjectUsers",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_ManagerId",
                table: "Projects",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Projects_ProjectId1",
                table: "ProjectUsers",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_ManagerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Projects_ProjectId1",
                table: "ProjectUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUsers_ProjectId1",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "ProjectUsers");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Projects",
                newName: "AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ManagerId",
                table: "Projects",
                newName: "IX_Projects_AssignedUserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserRole",
                table: "ProjectUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_AssignedUserId",
                table: "Projects",
                column: "AssignedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
