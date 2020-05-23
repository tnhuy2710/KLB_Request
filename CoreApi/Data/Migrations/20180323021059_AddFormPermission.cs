using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoreApi.Migrations
{
    public partial class AddFormPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormPermissions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FormId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Claims = table.Column<string>(nullable: false),
                    GroupIds = table.Column<string>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    AvailableFrom = table.Column<DateTimeOffset>(nullable: true),
                    ExpireIn = table.Column<DateTimeOffset>(nullable: true),
                    NextStepId = table.Column<string>(nullable: true),
                    PrevStepId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormPermissions_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormPermissions_FormId",
                table: "FormPermissions",
                column: "FormId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormPermissions");
        }
    }
}
