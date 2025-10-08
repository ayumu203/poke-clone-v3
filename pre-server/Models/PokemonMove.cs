namespace server.Models
{
    public class PokemonMove
    {
        /// <summary>
        /// 複合キーとするため、Key属性は付与しない
        /// </summary>
        public int PokemonSpeciesId { get; set; }
        public int MoveId { get; set; }
        public virtual PokemonSpecies PokemonSpecies { get; set; } = null!;
        public virtual Move Move { get; set; } = null!;
    }
}