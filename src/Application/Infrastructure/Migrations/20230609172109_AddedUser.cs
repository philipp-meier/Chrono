using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chrono.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TaskLists");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TaskLists");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedById",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "TaskLists",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedById",
                table: "TaskLists",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedById",
                table: "Tasks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_LastModifiedById",
                table: "Tasks",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLists_CreatedById",
                table: "TaskLists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLists_LastModifiedById",
                table: "TaskLists",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLists_Users_CreatedById",
                table: "TaskLists",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLists_Users_LastModifiedById",
                table: "TaskLists",
                column: "LastModifiedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_LastModifiedById",
                table: "Tasks",
                column: "LastModifiedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLists_Users_CreatedById",
                table: "TaskLists");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskLists_Users_LastModifiedById",
                table: "TaskLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_LastModifiedById",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_LastModifiedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskLists_CreatedById",
                table: "TaskLists");

            migrationBuilder.DropIndex(
                name: "IX_TaskLists_LastModifiedById",
                table: "TaskLists");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastModifiedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TaskLists");

            migrationBuilder.DropColumn(
                name: "LastModifiedById",
                table: "TaskLists");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TaskLists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "TaskLists",
                type: "TEXT",
                nullable: true);
        }
    }
}
