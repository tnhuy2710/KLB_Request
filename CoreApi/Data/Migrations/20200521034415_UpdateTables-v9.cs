using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreApi.Migrations
{
    public partial class UpdateTablesv9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Confirm",
                table: "FormSteps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Confirm",
                table: "Forms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirm",
                table: "FormSteps");

            migrationBuilder.DropColumn(
                name: "Confirm",
                table: "Forms");
        }
    }
}
