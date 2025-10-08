using server.Interfaces;
using System.Collections.Generic;
using System;
namespace server.Services
{
    public class TypeEffectivenessManager : ITypeEffectivenessManager
    {
        private readonly Dictionary<string, Dictionary<string, double>> _typeChart = new()
        {
            { "Normal", new Dictionary<string, double> { { "Fighter", 2.0 } } }
        };
        public double GetMuliplier(string attackType, string defenseType1, string? defenseType2 = null)
        {
            if(string.IsNullOrEmpty(attackType) || string.IsNullOrEmpty(defenseType1))
            {
                throw new ArgumentException("攻撃タイプまたは防御タイプが指定されていません.");
            }
            double multiplier = 1.0;
            if (_typeChart.TryGetValue(attackType, out var effectiveness) && effectiveness.TryGetValue(defenseType1, out var mult1))
            {
                multiplier *= mult1;
            }
            if (!string.IsNullOrEmpty(defenseType2) && _typeChart.TryGetValue(attackType, out var effectiveness2) && effectiveness2.TryGetValue(defenseType2, out var mult2))
            {
                multiplier *= mult2;
            }
            return 1.0; 
        }
    }
}