using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoreApi.Migrations
{
    public partial class RenameFromPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormPermissions_Forms_FormId",
                table: "FormPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserForms_FormPermissions_CurrentStepId",
                table: "UserForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormPermissions",
                table: "FormPermissions");

            migrationBuilder.RenameTable(
                name: "FormPermissions",
                newName: "FormSteps");

            migrationBuilder.RenameIndex(
                name: "IX_FormPermissions_FormId",
                table: "FormSteps",
                newName: "IX_FormSteps_FormId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormSteps",
                table: "FormSteps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSteps_Forms_FormId",
                table: "FormSteps",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserForms_FormSteps_CurrentStepId",
                table: "UserForms",
                column: "CurrentStepId",
                principalTable: "FormSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSteps_Forms_FormId",
                table: "FormSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_UserForms_FormSteps_CurrentStepId",
                table: "UserForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormSteps",
                table: "FormSteps");

            migrationBuilder.RenameTable(
                name: "FormSteps",
                newName: "FormPermissions");

            migrationBuilder.RenameIndex(
                name: "IX_FormSteps_FormId",
                table: "FormPermissions",
                newName: "IX_FormPermissions_FormId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormPermissions",
                table: "FormPermissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormPermissions_Forms_FormId",
                table: "FormPermissions",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserForms_FormPermissions_CurrentStepId",
                table: "UserForms",
                column: "CurrentStepId",
                principalTable: "FormPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
