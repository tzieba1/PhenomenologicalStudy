using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhenomenologicalStudy.API.Migrations
{
  public partial class ForeignKeyFix : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_AspNetUsers_Images_ImageId",
          table: "AspNetUsers");

      migrationBuilder.DropIndex(
          name: "IX_AspNetUsers_ImageId",
          table: "AspNetUsers");

      migrationBuilder.DropColumn(
          name: "ImageId",
          table: "AspNetUsers");

      migrationBuilder.RenameColumn(
          name: "UpdatedTime",
          table: "Comments",
          newName: "Updated");

      migrationBuilder.RenameColumn(
          name: "CreatedTime",
          table: "Comments",
          newName: "Created");

      migrationBuilder.AddColumn<byte[]>(
          name: "ProfilePicture",
          table: "AspNetUsers",
          type: "varbinary(max)",
          nullable: true);

      migrationBuilder.AlterColumn<int>(
          name: "Id",
          table: "AspNetUserClaims",
          type: "int",
          nullable: false,
          oldClrType: typeof(Guid),
          oldType: "uniqueidentifier")
          .Annotation("SqlServer:Identity", "1, 1")
          .OldAnnotation("SqlServer:Identity", "1, 1");

      migrationBuilder.AlterColumn<int>(
          name: "Id",
          table: "AspNetRoleClaims",
          type: "int",
          nullable: false,
          oldClrType: typeof(Guid),
          oldType: "uniqueidentifier")
          .Annotation("SqlServer:Identity", "1, 1")
          .OldAnnotation("SqlServer:Identity", "1, 1");

      migrationBuilder.CreateTable(
          name: "UserPermission",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UserPermission", x => x.Id);
            table.ForeignKey(
                      name: "FK_UserPermission_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_UserPermission_Permission_PermissionId",
                      column: x => x.PermissionId,
                      principalTable: "Permission",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_UserPermission_PermissionId",
          table: "UserPermission",
          column: "PermissionId");

      migrationBuilder.CreateIndex(
          name: "IX_UserPermission_UserId",
          table: "UserPermission",
          column: "UserId");

      // Trigger to handle Created and Updated times for Reflections table that will reset these two fields when the row is updated
      migrationBuilder.Sql("CREATE OR REPLACE TRIGGER dbo.SetUpdatedTimeAndCreationTimeReflections " +
                           "ON dbo.Reflections " +
                           "AFTER UPDATE AS " +
                           "BEGIN IF NOT UPDATE(UpdatedTime) " +
                              "BEGIN UPDATE new " +
                                "SET UpdatedTime = CURRENT_TIMESTAMP, CreationTime = d.CreationTime " +
                                "FROM Reflections new " +
                                "INNER JOIN inserted i ON new.id = i.id " +
                                "INNER JOIN deleted d ON new.id = d.id " +
                              "END " +
                           "END");

      // Trigger to handle Created and Updated times for Comments table that will reset these two fields when the row is updated
      migrationBuilder.Sql("CREATE OR REPLACE TRIGGER dbo.SetUpdatedTimeAndCreationTimeComments " +
                           "ON dbo.Comments " +
                           "AFTER UPDATE AS " +
                           "BEGIN IF NOT UPDATE(UpdatedTime) " +
                              "BEGIN UPDATE new " +
                                "SET UpdatedTime = CURRENT_TIMESTAMP, CreationTime = d.CreationTime " +
                                "FROM Comments new " +
                                "INNER JOIN inserted i ON new.id = i.id " +
                                "INNER JOIN deleted d ON new.id = d.id " +
                              "END " +
                           "END");

      // Trigger to handle Created and Updated times for ChildEmotions table that will reset these two fields when the row is updated
      migrationBuilder.Sql("CREATE OR REPLACE TRIGGER dbo.SetUpdatedTimeAndCreationTimeChildEmotions " +
                           "ON dbo.ChildEmotions " +
                           "AFTER UPDATE AS " +
                           "BEGIN IF NOT UPDATE(UpdatedTime) " +
                              "BEGIN UPDATE new " +
                                "SET UpdatedTime = CURRENT_TIMESTAMP, CreationTime = d.CreationTime " +
                                "FROM ChildEmotions new " +
                                "INNER JOIN inserted i ON new.id = i.id " +
                                "INNER JOIN deleted d ON new.id = d.id " +
                              "END " +
                           "END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "UserPermission");

      migrationBuilder.DropColumn(
          name: "ProfilePicture",
          table: "AspNetUsers");

      migrationBuilder.RenameColumn(
          name: "Updated",
          table: "Comments",
          newName: "UpdatedTime");

      migrationBuilder.RenameColumn(
          name: "Created",
          table: "Comments",
          newName: "CreatedTime");

      migrationBuilder.AddColumn<Guid>(
          name: "ImageId",
          table: "AspNetUsers",
          type: "uniqueidentifier",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

      migrationBuilder.AlterColumn<Guid>(
          name: "Id",
          table: "AspNetUserClaims",
          type: "uniqueidentifier",
          nullable: false,
          oldClrType: typeof(int),
          oldType: "int")
          .Annotation("SqlServer:Identity", "1, 1")
          .OldAnnotation("SqlServer:Identity", "1, 1");

      migrationBuilder.AlterColumn<Guid>(
          name: "Id",
          table: "AspNetRoleClaims",
          type: "uniqueidentifier",
          nullable: false,
          oldClrType: typeof(int),
          oldType: "int")
          .Annotation("SqlServer:Identity", "1, 1")
          .OldAnnotation("SqlServer:Identity", "1, 1");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUsers_ImageId",
          table: "AspNetUsers",
          column: "ImageId");

      migrationBuilder.AddForeignKey(
          name: "FK_AspNetUsers_Images_ImageId",
          table: "AspNetUsers",
          column: "ImageId",
          principalTable: "Images",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.Sql("DROP TRIGGER dbo.SetUpdatedAndCreatedReflections");
      migrationBuilder.Sql("DROP TRIGGER dbo.SetUpdatedAndCreatedComments");
    }
  }
}
