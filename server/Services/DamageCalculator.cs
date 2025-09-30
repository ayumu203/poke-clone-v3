using server.Interfaces;
using server.Models;
using System;

namespace server.Services
{
    public class DamageCalculator : IDamageCalculator
    {
        private readonly ITypeEffectivenessManager _typeEffectivenessManager;
        public DamageCalculator(ITypeEffectivenessManager typeEffectivenessManager)
        {
            _typeEffectivenessManager = typeEffectivenessManager ?? throw new ArgumentNullException(nameof(typeEffectivenessManager));
        }

        public int CalculateDamage(Pokemon attacker, Pokemon defender, Move move)
        {
            if (attacker == null) throw new ArgumentNullException(nameof(attacker));
            if (defender == null) throw new ArgumentNullException(nameof(defender));
            if (move == null) throw new ArgumentNullException(nameof(move));
            if (attacker.PokemonSpecies == null || defender.PokemonSpecies == null)
                throw new ArgumentException("ポケモンの種族情報が存在しません.");

            // この辺は適当
            // 今後しっかりと実装をする

            // 基本ダメージ計算式
            double levelFactor = (2.0 * attacker.Level) / 5.0 + 2.0;
            double attackStat = ( move.DamageClass == "Special" ) ? attacker.PokemonSpecies.BaseSpecialAttack : attacker.PokemonSpecies.BaseAttack;
            double defenseStat = ( move.DamageClass == "Special" ) ? defender.PokemonSpecies.BaseSpecialDefense : defender.PokemonSpecies.BaseDefense;
            double baseDamage = ((levelFactor * move.Power * (attackStat / defenseStat)) / 50.0) + 2.0;

            // タイプ相性の計算
            double typeMultiplier = _typeEffectivenessManager.GetMuliplier(move.Type, defender.PokemonSpecies.Type1, defender.PokemonSpecies.Type2);

            // ランダム要素（85%から100%の間）
            Random rand = new Random();
            double randomFactor = rand.Next(85, 101) / 100.0;

            // 最終ダメージ計算
            double totalDamage = baseDamage * typeMultiplier * randomFactor;

            return Math.Max(1, (int)Math.Floor(totalDamage)); // ダメージは最低1
        }
    }
}