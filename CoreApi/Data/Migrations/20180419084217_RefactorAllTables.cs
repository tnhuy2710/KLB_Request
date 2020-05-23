using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoreApi.Migrations
{
    public partial class RefactorAllTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFormDatas");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowSendEmail",
                table: "FormPermissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserFormLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserFormId = table.Column<long>(nullable: false),
                    StepId = table.Column<string>(nullable: false),
                    Action = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserForms",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FormId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    InputValues = table.Column<string>(nullable: true),
                    CurrentStepId = table.Column<string>(nullable: false),
                    AvailableFrom = table.Column<DateTimeOffset>(nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserForms_FormPermissions_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "FormPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserForms_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserForms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserForms_CurrentStepId",
                table: "UserForms",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_UserForms_FormId",
                table: "UserForms",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_UserForms_UserId",
                table: "UserForms",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFormLogs");

            migrationBuilder.DropTable(
                name: "UserForms");

            migrationBuilder.DropColumn(
                name: "IsAllowSendEmail",
                table: "FormPermissions");

            migrationBuilder.CreateTable(
                name: "UserFormDatas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    FormId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFormDatas_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFormDatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFormDatas_FormId",
                table: "UserFormDatas",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFormDatas_UserId",
                table: "UserFormDatas",
                column: "UserId");
        }
    }
}
