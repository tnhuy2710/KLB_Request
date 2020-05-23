using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreApi.Migrations
{
    public partial class UpdateTablesv7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorEmpCode",
                table: "UserFormLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetEmpCode",
                table: "UserFormLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorEmpCode",
                table: "UserFormLogs");

            migrationBuilder.DropColumn(
                name: "TargetEmpCode",
                table: "UserFormLogs");
        }
    }
}
