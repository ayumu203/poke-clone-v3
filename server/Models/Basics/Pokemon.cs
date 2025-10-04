using server.interfaces;
using server.Interfaces;
using server.Models.Basics.Battle;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Basics
{
    public class Pokemon
    {
        [Key]
        public int PokemonId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Level { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Exp { get; set; }
        
        [NotMapped]
        public int CurrentHp { get; set; }

        [NotMapped]
        public string? Ailment { get; set; } 

        [NotMapped]
        public IRank Rank { get; set; }　= new Rank();

        [NotMapped]
        public IVolatileStatus VolatileStatus { get; set; } = new VolatileStatus();


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
