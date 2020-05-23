using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoreApi.Migrations
{
    public partial class UpdateTablesv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StepId",
                table: "UserFormAssigns",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserFormAssigns_StepId",
                table: "UserFormAssigns",
                column: "StepId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFormAssigns_FormSteps_StepId",
                table: "UserFormAssigns",
                column: "StepId",
                principalTable: "FormSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFormAssigns_FormSteps_StepId",
                table: "UserFormAssigns");

            migrationBuilder.DropIndex(
                name: "IX_UserFormAssigns_StepId",
                table: "UserFormAssigns");

            migrationBuilder.DropColumn(
                name: "StepId",
                table: "UserFormAssigns");
        }
    }
}
