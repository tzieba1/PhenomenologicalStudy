using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhenomenologicalStudy.API.Migrations
{
  public partial class init : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "AspNetRoles",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetRoles", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUsers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            BirthDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUsers", x => x.Id);
          });

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

      migrationBuilder.CreateTable(
          name: "AspNetRoleClaims",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "AspNetRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserClaims",
          columns: table => new
          {
            Id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserLogins",
          columns: table => new
          {
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
            table.ForeignKey(
                      name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserRoles",
          columns: table => new
          {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
            table.ForeignKey(
                      name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                      column: x => x.RoleId,
                      principalTable: "AspNetRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "AspNetUserTokens",
          columns: table => new
          {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            table.ForeignKey(
                      name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Children",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            DateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
            Gender = table.Column<string>(type: "nvarchar(1)", nullable: false),
            UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Children", x => x.Id);
            table.ForeignKey(
                      name: "FK_Children_AspNetUsers_UsersId",
                      column: x => x.UsersId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "Permissions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Camera = table.Column<bool>(type: "bit", nullable: false),
            PictureLibrary = table.Column<bool>(type: "bit", nullable: false),
            FileSystem = table.Column<bool>(type: "bit", nullable: false),
            Microphone = table.Column<bool>(type: "bit", nullable: false),
            Badges = table.Column<bool>(type: "bit", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Permissions", x => x.Id);
            table.ForeignKey(
                      name: "FK_Permissions_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Reflections",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
            UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Reflections", x => x.Id);
            table.ForeignKey(
                      name: "FK_Reflections_AspNetUsers_UsersId",
                      column: x => x.UsersId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "RefreshTokens",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
            JwtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            IsUsed = table.Column<bool>(type: "bit", nullable: false),
            IsRevoked = table.Column<bool>(type: "bit", nullable: false),
            CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
            ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_RefreshTokens", x => x.Id);
            table.ForeignKey(
                      name: "FK_RefreshTokens_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Emotions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Intensity = table.Column<int>(type: "int", nullable: true),
            ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Emotions", x => x.Id);
            table.ForeignKey(
                      name: "FK_Emotions_Children_ChildId",
                      column: x => x.ChildId,
                      principalTable: "Children",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "Captures",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ReflectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Captures", x => x.Id);
            table.ForeignKey(
                      name: "FK_Captures_Reflections_ReflectionID",
                      column: x => x.ReflectionID,
                      principalTable: "Reflections",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Comments",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ReflectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
            CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
            UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Comments", x => x.Id);
            table.ForeignKey(
                      name: "FK_Comments_Reflections_ReflectionID",
                      column: x => x.ReflectionID,
                      principalTable: "Reflections",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ChildrenEmotions",
          columns: table => new
          {
            ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ReflectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            EmotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
            UpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ChildrenEmotions", x => new { x.ChildId, x.ReflectionId });
            table.ForeignKey(
                      name: "FK_ChildrenEmotions_Children_ChildId",
                      column: x => x.ChildId,
                      principalTable: "Children",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_ChildrenEmotions_Emotions_EmotionId",
                      column: x => x.EmotionId,
                      principalTable: "Emotions",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_ChildrenEmotions_Reflections_ReflectionId",
                      column: x => x.ReflectionId,
                      principalTable: "Reflections",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_AspNetRoleClaims_RoleId",
          table: "AspNetRoleClaims",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "RoleNameIndex",
          table: "AspNetRoles",
          column: "NormalizedName",
          unique: true,
          filter: "[NormalizedName] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUserClaims_UserId",
          table: "AspNetUserClaims",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUserLogins_UserId",
          table: "AspNetUserLogins",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_AspNetUserRoles_RoleId",
          table: "AspNetUserRoles",
          column: "RoleId");

      migrationBuilder.CreateIndex(
          name: "EmailIndex",
          table: "AspNetUsers",
          column: "NormalizedEmail");

      migrationBuilder.CreateIndex(
          name: "UserNameIndex",
          table: "AspNetUsers",
          column: "NormalizedUserName",
          unique: true,
          filter: "[NormalizedUserName] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_Captures_ReflectionID",
          table: "Captures",
          column: "ReflectionID");

      migrationBuilder.CreateIndex(
          name: "IX_Children_UsersId",
          table: "Children",
          column: "UsersId");

      migrationBuilder.CreateIndex(
          name: "IX_ChildrenEmotions_EmotionId",
          table: "ChildrenEmotions",
          column: "EmotionId");

      migrationBuilder.CreateIndex(
          name: "IX_ChildrenEmotions_ReflectionId",
          table: "ChildrenEmotions",
          column: "ReflectionId");

      migrationBuilder.CreateIndex(
          name: "IX_Comments_ReflectionID",
          table: "Comments",
          column: "ReflectionID");

      migrationBuilder.CreateIndex(
          name: "IX_Emotions_ChildId",
          table: "Emotions",
          column: "ChildId");

      migrationBuilder.CreateIndex(
          name: "IX_Permissions_UserId",
          table: "Permissions",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Reflections_UsersId",
          table: "Reflections",
          column: "UsersId");

      migrationBuilder.CreateIndex(
          name: "IX_RefreshTokens_UserId",
          table: "RefreshTokens",
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

      // Trigger to handle Created and Updated times for ChildrenEmotions table that will reset these two fields when the row is updated
      migrationBuilder.Sql("CREATE OR REPLACE TRIGGER dbo.SetUpdatedTimeAndCreationTimeChildrenEmotions " +
                           "ON dbo.ChildrenEmotions " +
                           "AFTER UPDATE AS " +
                           "BEGIN IF NOT UPDATE(UpdatedTime) " +
                              "BEGIN UPDATE new " +
                                "SET UpdatedTime = CURRENT_TIMESTAMP, CreationTime = d.CreationTime " +
                                "FROM ChildrenEmotions new " +
                                "INNER JOIN inserted i ON new.id = i.id " +
                                "INNER JOIN deleted d ON new.id = d.id " +
                              "END " +
                           "END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "AspNetRoleClaims");

      migrationBuilder.DropTable(
          name: "AspNetUserClaims");

      migrationBuilder.DropTable(
          name: "AspNetUserLogins");

      migrationBuilder.DropTable(
          name: "AspNetUserRoles");

      migrationBuilder.DropTable(
          name: "AspNetUserTokens");

      migrationBuilder.DropTable(
          name: "Captures");

      migrationBuilder.DropTable(
          name: "ChildrenEmotions");

      migrationBuilder.DropTable(
          name: "Comments");

      migrationBuilder.DropTable(
          name: "ErrorMessages");

      migrationBuilder.DropTable(
          name: "Permissions");

      migrationBuilder.DropTable(
          name: "RefreshTokens");

      migrationBuilder.DropTable(
          name: "AspNetRoles");

      migrationBuilder.DropTable(
          name: "Emotions");

      migrationBuilder.DropTable(
          name: "Reflections");

      migrationBuilder.DropTable(
          name: "Children");

      migrationBuilder.DropTable(
          name: "AspNetUsers");

      migrationBuilder.Sql("DROP TRIGGER dbo.SetUpdatedAndCreatedReflections");

      migrationBuilder.Sql("DROP TRIGGER dbo.SetUpdatedTimeAndCreationTimeComments");

      migrationBuilder.Sql("DROP TRIGGER ddbo.SetUpdatedTimeAndCreationTimeChildrenEmotions");

    }
  }
}
