using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chrono.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCategory_Categories_CategoryId",
                table: "TaskCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCategory_Tasks_TaskId",
                table: "TaskCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCategory",
                table: "TaskCategory");

            migrationBuilder.RenameTable(
                name: "TaskCategory",
                newName: "TaskCategories");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCategory_CategoryId",
                table: "TaskCategories",
                newName: "IX_TaskCategories_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCategories",
                table: "TaskCategories",
                columns: new[] { "TaskId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCategories_Categories_CategoryId",
                table: "TaskCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCategories_Tasks_TaskId",
                table: "TaskCategories",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCategories_Categories_CategoryId",
                table: "TaskCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCategories_Tasks_TaskId",
                table: "TaskCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCategories",
                table: "TaskCategories");

            migrationBuilder.RenameTable(
                name: "TaskCategories",
                newName: "TaskCategory");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCategories_CategoryId",
                table: "TaskCategory",
                newName: "IX_TaskCategory_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCategory",
                table: "TaskCategory",
                columns: new[] { "TaskId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCategory_Categories_CategoryId",
                table: "TaskCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCategory_Tasks_TaskId",
                table: "TaskCategory",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
