using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class PokemonSpecies
    {
        [Key] 
        public int PokemonId { get; set; }

        [Required]
        [StringLength(50)] 
        public string Name { get; set; }
        public string FrontImage { get; set; }
        public string BackImage { get; set; }

        [Required]
        public string Type1 { get; set; }
        public string? Type2 { get; set; } 
        public int EvolveLevel { get; set; }
        public int BaseHp { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefence { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefence { get; set; }
        public int BaseSpeed { get; set; }

        // public IList<Move> MoveList { get; set; } = new List<Move>();
    }
}