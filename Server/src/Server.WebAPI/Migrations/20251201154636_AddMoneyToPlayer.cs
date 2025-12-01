using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMoneyToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Money",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Money",
                table: "Player");
        }
    }
}
