namespace server.Models.Basics
{
    public class PokemonMove
    {
        public int PokemonSpeciesId { get; set; }
        public int MoveId { get; set; }
        public virtual PokemonSpecies PokemonSpecies { get; set; } = null!;
        public virtual Move Move { get; set; } = null!;
    }
}