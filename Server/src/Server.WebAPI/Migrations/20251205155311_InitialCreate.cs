using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Move",
                columns: table => new
                {
                    moveId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    type = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DamageClass = table.Column<int>(type: "int", nullable: false),
                    power = table.Column<int>(type: "int", nullable: false),
                    accuracy = table.Column<int>(type: "int", nullable: false),
                    pp = table.Column<int>(type: "int", nullable: false),
                    priority = table.Column<int>(type: "int", nullable: false),
                    target = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    statChance = table.Column<int>(type: "int", nullable: false),
                    ailment = table.Column<int>(type: "int", maxLength: 63, nullable: false),
                    ailmentChance = table.Column<int>(type: "int", nullable: false),
                    healing = table.Column<int>(type: "int", nullable: false),
                    drain = table.Column<int>(type: "int", nullable: false),
                    critRate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Move", x => x.moveId);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    iconUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.playerId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerParty",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerParty", x => x.playerId);
                });

            migrationBuilder.CreateTable(
                name: "PokemonSpecies",
                columns: table => new
                {
                    pokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    frontImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    backImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    type1 = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    type2 = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: true),
                    evolveLevel = table.Column<int>(type: "int", nullable: false),
                    baseHp = table.Column<int>(type: "int", nullable: false),
                    baseAttack = table.Column<int>(type: "int", nullable: false),
                    baseDefense = table.Column<int>(type: "int", nullable: false),
                    baseSpecialAttack = table.Column<int>(type: "int", nullable: false),
                    baseSpecialDefense = table.Column<int>(type: "int", nullable: false),
                    baseSpeed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonSpecies", x => x.pokemonSpeciesId);
                });

            migrationBuilder.CreateTable(
                name: "MoveStatChange",
                columns: table => new
                {
                    MoveId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    change = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveStatChange", x => new { x.MoveId, x.Id });
                    table.ForeignKey(
                        name: "FK_MoveStatChange_Move_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Move",
                        principalColumn: "moveId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pokemon",
                columns: table => new
                {
                    pokemonId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    pokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    exp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemon", x => x.pokemonId);
                    table.ForeignKey(
                        name: "FK_Pokemon_PokemonSpecies_pokemonSpeciesId",
                        column: x => x.pokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "pokemonSpeciesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PokemonMove",
                columns: table => new
                {
                    pokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    moveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMove", x => new { x.pokemonSpeciesId, x.moveId });
                    table.ForeignKey(
                        name: "FK_PokemonMove_Move_moveId",
                        column: x => x.moveId,
                        principalTable: "Move",
                        principalColumn: "moveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonMove_PokemonSpecies_pokemonSpeciesId",
                        column: x => x.pokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "pokemonSpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPartyPokemon",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    pokemonId = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPartyPokemon", x => new { x.playerId, x.pokemonId });
                    table.ForeignKey(
                        name: "FK_PlayerPartyPokemon_PlayerParty_playerId",
                        column: x => x.playerId,
                        principalTable: "PlayerParty",
                        principalColumn: "playerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerPartyPokemon_Pokemon_pokemonId",
                        column: x => x.pokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "pokemonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PokemonMoveInstance",
                columns: table => new
                {
                    pokemonId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    moveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMoveInstance", x => new { x.pokemonId, x.moveId });
                    table.ForeignKey(
                        name: "FK_PokemonMoveInstance_Move_moveId",
                        column: x => x.moveId,
                        principalTable: "Move",
                        principalColumn: "moveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonMoveInstance_Pokemon_pokemonId",
                        column: x => x.pokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "pokemonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPartyPokemon_pokemonId",
                table: "PlayerPartyPokemon",
                column: "pokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_pokemonSpeciesId",
                table: "Pokemon",
                column: "pokemonSpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMove_moveId",
                table: "PokemonMove",
                column: "moveId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoveInstance_moveId",
                table: "PokemonMoveInstance",
                column: "moveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoveStatChange");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "PlayerPartyPokemon");

            migrationBuilder.DropTable(
                name: "PokemonMove");

            migrationBuilder.DropTable(
                name: "PokemonMoveInstance");

            migrationBuilder.DropTable(
                name: "PlayerParty");

            migrationBuilder.DropTable(
                name: "Move");

            migrationBuilder.DropTable(
                name: "Pokemon");

            migrationBuilder.DropTable(
                name: "PokemonSpecies");
        }
    }
}
