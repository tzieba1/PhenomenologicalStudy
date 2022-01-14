using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhenomenologicalStudy.API.Migrations
{
  public partial class Badge : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "Badges",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
          Type = table.Column<int>(type: "int", nullable: false),
          Value = table.Column<int>(type: "int", nullable: false),
          Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
          UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
          CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
          UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Badges", x => x.Id);
          table.ForeignKey(
            name: "FK_Badges_AspNetUsers_UserId",
            column: x => x.UserId,
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
        });

      migrationBuilder.CreateIndex(
        name: "IX_Badges_UserId",
        table: "Badges",
        column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
        name: "Badges");
    }
  }
}
