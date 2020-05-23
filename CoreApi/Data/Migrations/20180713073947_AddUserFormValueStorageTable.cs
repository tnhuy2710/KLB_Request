using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreApi.Migrations
{
    public partial class AddUserFormValueStorageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFormValueStorages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserFormId = table.Column<long>(nullable: false),
                    RowNumber = table.Column<int>(nullable: false),
                    A = table.Column<string>(nullable: true),
                    B = table.Column<string>(nullable: true),
                    C = table.Column<string>(nullable: true),
                    D = table.Column<string>(nullable: true),
                    E = table.Column<string>(nullable: true),
                    F = table.Column<string>(nullable: true),
                    G = table.Column<string>(nullable: true),
                    H = table.Column<string>(nullable: true),
                    I = table.Column<string>(nullable: true),
                    J = table.Column<string>(nullable: true),
                    K = table.Column<string>(nullable: true),
                    L = table.Column<string>(nullable: true),
                    M = table.Column<string>(nullable: true),
                    N = table.Column<string>(nullable: true),
                    O = table.Column<string>(nullable: true),
                    P = table.Column<string>(nullable: true),
                    Q = table.Column<string>(nullable: true),
                    R = table.Column<string>(nullable: true),
                    S = table.Column<string>(nullable: true),
                    T = table.Column<string>(nullable: true),
                    U = table.Column<string>(nullable: true),
                    V = table.Column<string>(nullable: true),
                    W = table.Column<string>(nullable: true),
                    X = table.Column<string>(nullable: true),
                    Y = table.Column<string>(nullable: true),
                    Z = table.Column<string>(nullable: true),
                    AA = table.Column<string>(nullable: true),
                    AB = table.Column<string>(nullable: true),
                    AC = table.Column<string>(nullable: true),
                    AD = table.Column<string>(nullable: true),
                    AE = table.Column<string>(nullable: true),
                    AF = table.Column<string>(nullable: true),
                    AG = table.Column<string>(nullable: true),
                    AH = table.Column<string>(nullable: true),
                    AI = table.Column<string>(nullable: true),
                    AJ = table.Column<string>(nullable: true),
                    AK = table.Column<string>(nullable: true),
                    AL = table.Column<string>(nullable: true),
                    AM = table.Column<string>(nullable: true),
                    AN = table.Column<string>(nullable: true),
                    AO = table.Column<string>(nullable: true),
                    AP = table.Column<string>(nullable: true),
                    AQ = table.Column<string>(nullable: true),
                    AR = table.Column<string>(nullable: true),
                    AS = table.Column<string>(nullable: true),
                    AT = table.Column<string>(nullable: true),
                    AU = table.Column<string>(nullable: true),
                    AV = table.Column<string>(nullable: true),
                    AW = table.Column<string>(nullable: true),
                    AX = table.Column<string>(nullable: true),
                    AY = table.Column<string>(nullable: true),
                    AZ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormValueStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFormValueStorages_UserForms_UserFormId",
                        column: x => x.UserFormId,
                        principalTable: "UserForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFormValueStorages_UserFormId",
                table: "UserFormValueStorages",
                column: "UserFormId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFormValueStorages");
        }
    }
}
