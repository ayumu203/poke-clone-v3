using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithManualPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "PokemonSpecies",
                columns: table => new
                {
                    PokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    FrontImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BackImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type1 = table.Column<string>(type: "nvarchar(31)", maxLength: 31, nullable: false),
                    Type2 = table.Column<string>(type: "nvarchar(31)", maxLength: 31, nullable: true),
                    EvolveLevel = table.Column<int>(type: "int", nullable: true),
                    BaseHp = table.Column<int>(type: "int", nullable: false),
                    BaseAttack = table.Column<int>(type: "int", nullable: false),
                    BaseDefense = table.Column<int>(type: "int", nullable: false),
                    BaseSpecialAttack = table.Column<int>(type: "int", nullable: false),
                    BaseSpecialDefense = table.Column<int>(type: "int", nullable: false),
                    BaseSpeed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonSpecies", x => x.PokemonSpeciesId);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    MoveId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(31)", maxLength: 31, nullable: false),
                    DamageClass = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Pp = table.Column<int>(type: "int", nullable: false),
                    Accuracy = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    Defense = table.Column<int>(type: "int", nullable: false),
                    SpecialAttack = table.Column<int>(type: "int", nullable: false),
                    SpecialDefense = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    AccuracyChange = table.Column<int>(type: "int", nullable: false),
                    RankChance = table.Column<int>(type: "int", nullable: false),
                    Ailment = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: true),
                    AilmentChance = table.Column<int>(type: "int", nullable: false),
                    Healing = table.Column<int>(type: "int", nullable: false),
                    Draining = table.Column<int>(type: "int", nullable: false),
                    PokemonSpeciesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.MoveId);
                    table.ForeignKey(
                        name: "FK_Moves_PokemonSpecies_PokemonSpeciesId",
                        column: x => x.PokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "PokemonSpeciesId");
                });

            migrationBuilder.CreateTable(
                name: "PokemonMoves",
                columns: table => new
                {
                    PokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    MoveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMoves", x => new { x.PokemonSpeciesId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_PokemonMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonMoves_PokemonSpecies_PokemonSpeciesId",
                        column: x => x.PokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "PokemonSpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pokemons",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    PokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    Move1Id = table.Column<int>(type: "int", nullable: true),
                    Move2Id = table.Column<int>(type: "int", nullable: true),
                    Move3Id = table.Column<int>(type: "int", nullable: true),
                    Move4Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemons", x => x.PokemonId);
                    table.ForeignKey(
                        name: "FK_Pokemons_Moves_Move1Id",
                        column: x => x.Move1Id,
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemons_Moves_Move2Id",
                        column: x => x.Move2Id,
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemons_Moves_Move3Id",
                        column: x => x.Move3Id,
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemons_Moves_Move4Id",
                        column: x => x.Move4Id,
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pokemons_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pokemons_PokemonSpecies_PokemonSpeciesId",
                        column: x => x.PokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "PokemonSpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerParties",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    SlotIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerParties", x => new { x.PlayerId, x.PokemonId });
                    table.ForeignKey(
                        name: "FK_PlayerParties_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerParties_Pokemons_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemons",
                        principalColumn: "PokemonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Moves_PokemonSpeciesId",
                table: "Moves",
                column: "PokemonSpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerParties_PokemonId",
                table: "PlayerParties",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMoves_MoveId",
                table: "PokemonMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_Move1Id",
                table: "Pokemons",
                column: "Move1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_Move2Id",
                table: "Pokemons",
                column: "Move2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_Move3Id",
                table: "Pokemons",
                column: "Move3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_Move4Id",
                table: "Pokemons",
                column: "Move4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_PlayerId",
                table: "Pokemons",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_PokemonSpeciesId",
                table: "Pokemons",
                column: "PokemonSpeciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerParties");

            migrationBuilder.DropTable(
                name: "PokemonMoves");

            migrationBuilder.DropTable(
                name: "Pokemons");

            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PokemonSpecies");
        }
    }
}
