using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class PlayerParty
    {
        /// <summary>
        /// 複合キーにするため、Key属性は使用しない
        /// </summary>
        public string PlayerId { get; set; } = string.Empty;
        public int PokemonId { get; set; }

        [Required]
        [Range(1, 6)]
        public int SlotIndex { get; set; }

        public virtual Player Player { get; set; } = null!;
        public virtual Pokemon Pokemon { get; set; } = null!;
    }
}