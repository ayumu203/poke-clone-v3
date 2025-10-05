using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using server.Models.Game;

namespace server.Models.Core
{
    public class Player
    {
        [Key]
        [StringLength(255)]
        public string PlayerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "プレイヤー名は必須です.")]
        [StringLength(63, ErrorMessage = "プレイヤ名の長さ上限を超えています.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "プロフィール画像のURLは必須です.")]
        [StringLength(255)]
        [Url(ErrorMessage = "プロフィール画像のURLが不正です.")]
        public string IconUrl { get; set; } = string.Empty;
        // AD関係のフィールド
        [Required]
        [StringLength(100)]
        public string AzureObjectId { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public virtual ICollection<PlayerParty> PlayerParties { get; set; } = new List<PlayerParty>();
    }
}