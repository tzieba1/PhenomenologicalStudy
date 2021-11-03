using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhenomenologicalStudy.API.Migrations
{
    public partial class XmlSerialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Reflections",
                newName: "UpdatedTime");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Reflections",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Comments",
                newName: "UpdatedTime");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Comments",
                newName: "CreationTime");

      migrationBuilder.AddColumn<DateTimeOffset>(
          name: "CreationTime",
          table: "ChildEmotions",
          type: "datetimeoffset",
          nullable: false,
          defaultValueSql: "SYSDATETIMEOFFSET()");


      migrationBuilder.AddColumn<DateTimeOffset>(
          name: "UpdatedTime",
          table: "ChildEmotions",
          type: "datetimeoffset",
          nullable: false,
          defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.CreateTable(
                name: "ErrorMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorMessages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorMessages");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "ChildEmotions");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "ChildEmotions");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Reflections",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Reflections",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Comments",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Comments",
                newName: "Created");
        }
    }
}
