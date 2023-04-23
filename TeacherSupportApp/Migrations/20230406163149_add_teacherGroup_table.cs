using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherSupportApp.Migrations
{
    /// <inheritdoc />
    public partial class add_teacherGroup_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherGroup_Groups_GroupId",
                table: "TeacherGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherGroup_Teachers_TeacherId",
                table: "TeacherGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherGroup",
                table: "TeacherGroup");

            migrationBuilder.RenameTable(
                name: "TeacherGroup",
                newName: "TeacherGroups");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherGroup_GroupId",
                table: "TeacherGroups",
                newName: "IX_TeacherGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherGroups",
                table: "TeacherGroups",
                columns: new[] { "TeacherId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherGroups_Groups_GroupId",
                table: "TeacherGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherGroups_Teachers_TeacherId",
                table: "TeacherGroups",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherGroups_Groups_GroupId",
                table: "TeacherGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherGroups_Teachers_TeacherId",
                table: "TeacherGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherGroups",
                table: "TeacherGroups");

            migrationBuilder.RenameTable(
                name: "TeacherGroups",
                newName: "TeacherGroup");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherGroups_GroupId",
                table: "TeacherGroup",
                newName: "IX_TeacherGroup_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherGroup",
                table: "TeacherGroup",
                columns: new[] { "TeacherId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherGroup_Groups_GroupId",
                table: "TeacherGroup",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherGroup_Teachers_TeacherId",
                table: "TeacherGroup",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
