using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "PokemonSpecies",
                columns: table => new
                {
                    PokemonSpeciesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FrontImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type1 = table.Column<int>(type: "int", nullable: false),
                    Type2 = table.Column<int>(type: "int", nullable: true),
                    EvolveLevel = table.Column<int>(type: "int", nullable: false),
                    BaseHP = table.Column<int>(type: "int", nullable: false),
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
                    MoveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DamageClass = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Accuracy = table.Column<int>(type: "int", nullable: false),
                    PP = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Rank_Attack = table.Column<int>(type: "int", nullable: false),
                    Rank_Defense = table.Column<int>(type: "int", nullable: false),
                    Rank_SpecialAttack = table.Column<int>(type: "int", nullable: false),
                    Rank_SpecialDefense = table.Column<int>(type: "int", nullable: false),
                    Rank_Speed = table.Column<int>(type: "int", nullable: false),
                    Rank_Accuracy = table.Column<int>(type: "int", nullable: false),
                    Rank_Evasion = table.Column<int>(type: "int", nullable: false),
                    StatTarget = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatChance = table.Column<int>(type: "int", nullable: false),
                    Ailment = table.Column<int>(type: "int", nullable: false),
                    AilmentChance = table.Column<int>(type: "int", nullable: false),
                    Healing = table.Column<int>(type: "int", nullable: false),
                    Drain = table.Column<int>(type: "int", nullable: false),
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
                name: "Pokemons",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    CurrentHP = table.Column<int>(type: "int", nullable: false),
                    Ailment = table.Column<int>(type: "int", nullable: false),
                    PokemonSpeciesId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rank_Attack = table.Column<int>(type: "int", nullable: false),
                    Rank_Defense = table.Column<int>(type: "int", nullable: false),
                    Rank_SpecialAttack = table.Column<int>(type: "int", nullable: false),
                    Rank_SpecialDefense = table.Column<int>(type: "int", nullable: false),
                    Rank_Speed = table.Column<int>(type: "int", nullable: false),
                    Rank_Accuracy = table.Column<int>(type: "int", nullable: false),
                    Rank_Evasion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemons", x => x.PokemonId);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearnedMoves",
                columns: table => new
                {
                    LearnedMoveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentPP = table.Column<int>(type: "int", nullable: false),
                    MoveId = table.Column<int>(type: "int", nullable: false),
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnedMoves", x => x.LearnedMoveId);
                    table.ForeignKey(
                        name: "FK_LearnedMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LearnedMoves_Pokemons_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemons",
                        principalColumn: "PokemonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnedMoves_MoveId",
                table: "LearnedMoves",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnedMoves_PokemonId",
                table: "LearnedMoves",
                column: "PokemonId");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_PokemonSpeciesId",
                table: "Moves",
                column: "PokemonSpeciesId");

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
                name: "LearnedMoves");

            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "Pokemons");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PokemonSpecies");
        }
    }
}
