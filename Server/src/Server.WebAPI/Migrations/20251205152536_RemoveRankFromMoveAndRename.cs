using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRankFromMoveAndRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rankAccuracy",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankAttack",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankDefence",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankEvasion",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankSpecialAttack",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankSpecialDefence",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankSpeed",
                table: "Move");

            migrationBuilder.RenameColumn(
                name: "rankTarget",
                table: "Move",
                newName: "target");

            migrationBuilder.RenameColumn(
                name: "rankChance",
                table: "Move",
                newName: "statChance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "target",
                table: "Move",
                newName: "rankTarget");

            migrationBuilder.RenameColumn(
                name: "statChance",
                table: "Move",
                newName: "rankChance");

            migrationBuilder.AddColumn<int>(
                name: "rankAccuracy",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rankAttack",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rankDefence",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rankEvasion",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rankSpecialAttack",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rankSpecialDefence",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rankSpeed",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
