using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    /// <summary>
    ///  プレイヤーに関するクラス
    /// </summary>
    public class Player
    {
        [Key]
        [StringLength(255)]
        public string playerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "プレイヤー名は必須です.")]
        [StringLength(63, ErrorMessage = "プレイヤ名の長さ上限を超えています.")]
        public string name { get; set; } = string.Empty;

        [Required(ErrorMessage = "プロフィール画像のURLは必須です.")]
        [StringLength(255)]
        [Url(ErrorMessage = "プロフィール画像のURLが不正です.")]
        public string IconUrl { get; set; } = string.Empty;

        public virtual ICollection<Pokemon> Pokemons { get; set; } = new List<Pokemon>();
        public virtual ICollection<PlayerParty> PlayerParties { get; set; } = new List<PlayerParty>();
    }
}