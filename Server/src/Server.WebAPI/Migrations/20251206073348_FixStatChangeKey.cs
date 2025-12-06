using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixStatChangeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MoveStatChange",
                table: "MoveStatChange");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MoveStatChange");

            migrationBuilder.AlterColumn<string>(
                name: "stat",
                table: "MoveStatChange",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoveStatChange",
                table: "MoveStatChange",
                columns: new[] { "MoveId", "stat" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MoveStatChange",
                table: "MoveStatChange");

            migrationBuilder.AlterColumn<string>(
                name: "stat",
                table: "MoveStatChange",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MoveStatChange",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoveStatChange",
                table: "MoveStatChange",
                columns: new[] { "MoveId", "Id" });
        }
    }
}
