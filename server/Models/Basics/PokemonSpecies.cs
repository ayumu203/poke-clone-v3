using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace server.Models.Basics
{
    public class PokemonSpecies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PokemonSpeciesId { get; set; }

        [Required]
        [StringLength(63)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        [Url]
        public string FrontImage { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        [Url]
        public string BackImage { get; set; } = string.Empty;
        [Required]
        [StringLength(31)]
        public string Type1 { get; set; } = string.Empty;
        [StringLength(31)]
        public string? Type2 { get; set; } = string.Empty;
        [Range(1, 100)]
        public int? EvolveLevel { get; set; }
        [Range(1, 255)]
        public int BaseHp { get; set; }
        [Range(1, 255)]
        public int BaseAttack { get; set; }
        [Range(1, 255)]
        public int BaseDefense { get; set; }
        [Range(1, 255)]
        public int BaseSpecialAttack { get; set; }
        [Range(1, 255)]
        public int BaseSpecialDefense { get; set; }
        [Range(1, 255)]
        public int BaseSpeed { get; set; }
        public ICollection<Move> Moves { get; private set; } = new List<Move>();
    }
}