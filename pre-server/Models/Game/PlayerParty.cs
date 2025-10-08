using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using server.Models.Core;

namespace server.Models.Game
{
    public class PlayerParty
    {
        public string PlayerId { get; set; } = string.Empty;
        public int PokemonId { get; set; }

        [Required]
        [Range(1, 6)]
        public int SlotIndex { get; set; }

        public virtual Player Player { get; set; } = null!;
        public virtual Pokemon Pokemon { get; set; } = null!;
    }
}