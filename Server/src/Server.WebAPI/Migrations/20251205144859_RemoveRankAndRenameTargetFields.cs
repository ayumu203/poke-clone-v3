using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRankAndRenameTargetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerParty_Player_playerId",
                table: "PlayerParty");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPartyPokemon_PlayerParty_playerPartyId",
                table: "PlayerPartyPokemon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerPartyPokemon",
                table: "PlayerPartyPokemon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerParty",
                table: "PlayerParty");

            migrationBuilder.DropIndex(
                name: "IX_PlayerParty_playerId",
                table: "PlayerParty");

            migrationBuilder.DropColumn(
                name: "playerPartyId",
                table: "PlayerPartyPokemon");

            migrationBuilder.DropColumn(
                name: "playerPartyId",
                table: "PlayerParty");

            migrationBuilder.DropColumn(
                name: "rankAccuracy",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankAttack",
                table: "Move");

            migrationBuilder.DropColumn(
                name: "rankChance",
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

            migrationBuilder.RenameColumn(
                name: "rankTarget",
                table: "Move",
                newName: "target");

            migrationBuilder.RenameColumn(
                name: "rankSpeed",
                table: "Move",
                newName: "statChance");

            migrationBuilder.AddColumn<string>(
                name: "playerId",
                table: "PlayerPartyPokemon",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerPartyPokemon",
                table: "PlayerPartyPokemon",
                columns: new[] { "playerId", "pokemonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerParty",
                table: "PlayerParty",
                column: "playerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPartyPokemon_PlayerParty_playerId",
                table: "PlayerPartyPokemon",
                column: "playerId",
                principalTable: "PlayerParty",
                principalColumn: "playerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerPartyPokemon_PlayerParty_playerId",
                table: "PlayerPartyPokemon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerPartyPokemon",
                table: "PlayerPartyPokemon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerParty",
                table: "PlayerParty");

            migrationBuilder.DropColumn(
                name: "playerId",
                table: "PlayerPartyPokemon");

            migrationBuilder.RenameColumn(
                name: "target",
                table: "Move",
                newName: "rankTarget");

            migrationBuilder.RenameColumn(
                name: "statChance",
                table: "Move",
                newName: "rankSpeed");

            migrationBuilder.AddColumn<int>(
                name: "playerPartyId",
                table: "PlayerPartyPokemon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "playerPartyId",
                table: "PlayerParty",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

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
                name: "rankChance",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerPartyPokemon",
                table: "PlayerPartyPokemon",
                columns: new[] { "playerPartyId", "pokemonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerParty",
                table: "PlayerParty",
                column: "playerPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerParty_playerId",
                table: "PlayerParty",
                column: "playerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerParty_Player_playerId",
                table: "PlayerParty",
                column: "playerId",
                principalTable: "Player",
                principalColumn: "playerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerPartyPokemon_PlayerParty_playerPartyId",
                table: "PlayerPartyPokemon",
                column: "playerPartyId",
                principalTable: "PlayerParty",
                principalColumn: "playerPartyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
