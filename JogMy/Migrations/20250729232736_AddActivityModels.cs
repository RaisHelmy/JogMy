using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JogMy.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    VideoPath = table.Column<string>(type: "TEXT", nullable: true),
                    Privacy = table.Column<int>(type: "INTEGER", nullable: false),
                    Distance = table.Column<double>(type: "REAL", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Route = table.Column<string>(type: "TEXT", nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityPostId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ParentCommentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityComments_ActivityComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "ActivityComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityComments_ActivityPosts_ActivityPostId",
                        column: x => x.ActivityPostId,
                        principalTable: "ActivityPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityPostId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLikes_ActivityPosts_ActivityPostId",
                        column: x => x.ActivityPostId,
                        principalTable: "ActivityPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityComments_ActivityPostId",
                table: "ActivityComments",
                column: "ActivityPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityComments_ParentCommentId",
                table: "ActivityComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityComments_UserId",
                table: "ActivityComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLikes_ActivityPostId_UserId",
                table: "ActivityLikes",
                columns: new[] { "ActivityPostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLikes_UserId",
                table: "ActivityLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPosts_UserId",
                table: "ActivityPosts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityComments");

            migrationBuilder.DropTable(
                name: "ActivityLikes");

            migrationBuilder.DropTable(
                name: "ActivityPosts");
        }
    }
}
