using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoreApi.Migrations
{
    public partial class UpdateTablesv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFormSteps");

            migrationBuilder.CreateTable(
                name: "UserFormAssigns",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmpCode = table.Column<string>(nullable: false),
                    UserFormId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormAssigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFormAssigns_UserForms_UserFormId",
                        column: x => x.UserFormId,
                        principalTable: "UserForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFormAssigns_UserFormId",
                table: "UserFormAssigns",
                column: "UserFormId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFormAssigns");

            migrationBuilder.CreateTable(
                name: "UserFormSteps",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    FormStepId = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormSteps", x => new { x.UserId, x.FormStepId });
                    table.ForeignKey(
                        name: "FK_UserFormSteps_FormSteps_FormStepId",
                        column: x => x.FormStepId,
                        principalTable: "FormSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFormSteps_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFormSteps_FormStepId",
                table: "UserFormSteps",
                column: "FormStepId");
        }
    }
}
