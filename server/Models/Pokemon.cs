using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Pokemon
    {
        [Key]
        public int PokemonId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Level { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Exp { get; set; }
        [Required]
        public string PlayerId { get; set; } = string.Empty;
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; } = null!;
        [Required]
        public int PokemonSpeciesId { get; set; }
        [ForeignKey("PokemonSpeciesId")]
        public virtual PokemonSpecies PokemonSpecies { get; set; } = null!;

        public int? Move1Id { get; set; }
        [ForeignKey("Move1Id")]
        public virtual Move? Move1 { get; set; }
        public int? Move2Id { get; set; }
        [ForeignKey("Move2Id")]
        public virtual Move? Move2 { get; set; }
        public int? Move3Id { get; set; }
        [ForeignKey("Move3Id")]
        public virtual Move? Move3 { get; set; }
        public int? Move4Id { get; set; }
        [ForeignKey("Move4Id")]
        public virtual Move? Move4 { get; set; }
    }
}