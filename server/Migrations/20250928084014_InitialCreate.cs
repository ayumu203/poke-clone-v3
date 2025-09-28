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
                name: "PokemonSpecies",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FrontImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvolveLevel = table.Column<int>(type: "int", nullable: false),
                    BaseHp = table.Column<int>(type: "int", nullable: false),
                    BaseAttack = table.Column<int>(type: "int", nullable: false),
                    BaseDefence = table.Column<int>(type: "int", nullable: false),
                    BaseSpecialAttack = table.Column<int>(type: "int", nullable: false),
                    BaseSpecialDefence = table.Column<int>(type: "int", nullable: false),
                    BaseSpeed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonSpecies", x => x.PokemonId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonSpecies");
        }
    }
}
