using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JogMy.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverPhotoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPhotoPath",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPhotoPath",
                table: "AspNetUsers");
        }
    }
}
