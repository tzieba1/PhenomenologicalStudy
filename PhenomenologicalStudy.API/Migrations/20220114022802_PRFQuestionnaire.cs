using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhenomenologicalStudy.API.Migrations
{
    public partial class PRFQuestionnaire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Statement1 = table.Column<int>(type: "int", nullable: false),
                    Statement2 = table.Column<int>(type: "int", nullable: false),
                    Statement3 = table.Column<int>(type: "int", nullable: false),
                    Statement4 = table.Column<int>(type: "int", nullable: false),
                    Statement5 = table.Column<int>(type: "int", nullable: false),
                    Statement6 = table.Column<int>(type: "int", nullable: false),
                    Statement7 = table.Column<int>(type: "int", nullable: false),
                    Statement8 = table.Column<int>(type: "int", nullable: false),
                    Statement9 = table.Column<int>(type: "int", nullable: false),
                    Statement10 = table.Column<int>(type: "int", nullable: false),
                    Statement11 = table.Column<int>(type: "int", nullable: false),
                    Statement12 = table.Column<int>(type: "int", nullable: false),
                    Statement13 = table.Column<int>(type: "int", nullable: false),
                    Statement14 = table.Column<int>(type: "int", nullable: false),
                    Statement15 = table.Column<int>(type: "int", nullable: false),
                    Statement16 = table.Column<int>(type: "int", nullable: false),
                    Statement17 = table.Column<int>(type: "int", nullable: false),
                    Statement18 = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questionnaires_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_UserId",
                table: "Questionnaires",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questionnaires");
        }
    }
}
