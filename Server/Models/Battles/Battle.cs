using System.ComponentModel.DataAnnotations;
using server.Models.Core;
using server.Models.Enums;

namespace server.Models.Battles;

/// <summary>
/// バトル
/// </summary>
public class Battle
{
    [Key]
    public int BattleId { get; set; }
    
    public int Turn { get; set; } = 0;
    
    public List<TurnResult> TurnResults { get; set; } = new List<TurnResult>();
    
    public int Player1ActivePokemonIndex { get; set; } = 0;
    
    public int Player2ActivePokemonIndex { get; set; } = 0;
    
    public List<Pokemon> Player1Party { get; set; } = new List<Pokemon>();
    
    public List<Pokemon> Player2Party { get; set; } = new List<Pokemon>();
    
    public GameState GameState { get; set; } = GameState.WaitingForActions;
    
    // メソッド
    public Battle StartBattle()
    {
        GameState = GameState.WaitingForActions;
        Turn = 1;
        return this;
    }
    
    public TurnResult ExecuteTurn(PlayerAction action1, PlayerAction action2)
    {
        // TODO: ターン実行ロジック実装
        Turn++;
        return new TurnResult();
    }
}
