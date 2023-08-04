using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chrono.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTaskListOptionsDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskListOptions_TaskLists_TaskListId",
                table: "TaskListOptions");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskListOptions_TaskLists_TaskListId",
                table: "TaskListOptions",
                column: "TaskListId",
                principalTable: "TaskLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskListOptions_TaskLists_TaskListId",
                table: "TaskListOptions");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskListOptions_TaskLists_TaskListId",
                table: "TaskListOptions",
                column: "TaskListId",
                principalTable: "TaskLists",
                principalColumn: "Id");
        }
    }
}
