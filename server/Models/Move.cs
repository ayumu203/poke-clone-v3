using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Move
    {
        [Key]
        public int MoveId { get; set; }

        [Required]
        [StringLength(63)]
        public string Category { get; set; } = string.Empty;
        [Required]
        [StringLength(31)]
        public string Type { get; set; } = string.Empty;
        [Required]
        [StringLength(63)]
        public string DamageClass { get; set; } = string.Empty;

        [Required]
        [Range(0, 255)]
        public int Power { get; set; }
        [Required]
        [Range(0, 40)]
        public int Pp { get; set; }
        [Required]
        [Range(0, 100)]
        public int Accuracy { get; set; }
        [Required]
        [Range(-7, 7)]
        public int Priority { get; set; }
        [Required]
        [Range(-6, 6)]
        public int Attack { get; set; }
        [Required]
        [Range(-6, 6)]
        public int Defense { get; set; }
        [Required]
        [Range(-6, 6)]
        public int SpecialAttack { get; set; }
        [Required]
        [Range(-6, 6)]
        public int SpecialDefense { get; set; }
        [Required]
        [Range(-6, 6)]
        public int Speed { get; set; }
        [Required]
        [Range(-6, 6)]
        public int AccuracyChange { get; set; }
        [Range(0, 100)]
        public int RankChance { get; set; }
        [StringLength(63)]
        public string? Ailment { get; set; }
        [Range(0, 100)]
        public int AilmentChance { get; set; }
        [Range(0, 100)]
        public int Healing { get; set; }
        [Range(0, 100)]
        public int Draining { get; set; }

        // public virtual ICollection<PokemonMove> PokemonMoves { get; set; } = new List<PokemonMove>();
    }
}