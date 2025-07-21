using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.ProTrack.Migrations
{
    /// <inheritdoc />
    public partial class addingHistoryTablesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedUserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedByUserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedUserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedByUserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectHistories");

            migrationBuilder.DropTable(
                name: "TaskHistories");
        }
    }
}
