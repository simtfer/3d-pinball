using System;
using UnityEngine;

namespace DouyinPinball.Core
{
    public sealed class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int comboStep = 1;
        [SerializeField] private int maxCombo = 5;

        public event Action<int> ScoreChanged;
        public event Action<int> ComboChanged;

        public int Score { get; private set; }
        public int Combo { get; private set; } = 1;

        public void ResetScore()
        {
            Score = 0;
            Combo = 1;
            ScoreChanged?.Invoke(Score);
            ComboChanged?.Invoke(Combo);
        }

        public void AddScore(int baseValue)
        {
            Score += Mathf.Max(0, baseValue) * Combo;
            ScoreChanged?.Invoke(Score);
        }

        public void IncreaseCombo()
        {
            Combo = Mathf.Clamp(Combo + comboStep, 1, maxCombo);
            ComboChanged?.Invoke(Combo);
        }

        public void ResetCombo()
        {
            Combo = 1;
            ComboChanged?.Invoke(Combo);
        }
    }
}
