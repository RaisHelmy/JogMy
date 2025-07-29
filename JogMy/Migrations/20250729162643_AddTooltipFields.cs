using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JogMy.Migrations
{
    /// <inheritdoc />
    public partial class AddTooltipFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BestTimeToJog",
                table: "JoggingTracks",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomDifficulty",
                table: "JoggingTracks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "JoggingTracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRestrooms",
                table: "JoggingTracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWaterFountains",
                table: "JoggingTracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWellLit",
                table: "JoggingTracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SafetyNotes",
                table: "JoggingTracks",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialFeatures",
                table: "JoggingTracks",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurfaceType",
                table: "JoggingTracks",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestTimeToJog",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "CustomDifficulty",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "HasRestrooms",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "HasWaterFountains",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "IsWellLit",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "SafetyNotes",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "SpecialFeatures",
                table: "JoggingTracks");

            migrationBuilder.DropColumn(
                name: "SurfaceType",
                table: "JoggingTracks");
        }
    }
}
