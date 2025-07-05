using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MusicalMemory.UI
{
    public class ScoreCountUI : MonoBehaviour
    {
        private int currentScore = 0;
        private int highScore = 0;

        [SerializeField] private TextMeshProUGUI tmp_Score = null;

        public UnityEvent<int> OnScoreUpdated;

        public void UpdateScore(int _score)
        {
            currentScore = _score;

            UpdateScoreTexts();

            OnScoreUpdated?.Invoke(_score);
        }

        private void UpdateScoreTexts()
        {
            StringBuilder scoreBuilder = new StringBuilder();
            scoreBuilder.Append(currentScore);

            if (highScore > currentScore)
            {
                scoreBuilder.Append($"/{highScore}");
            }

            tmp_Score.SetText(scoreBuilder.ToString());
        }

        public void UpdateHighScore(int _highScore)
        {
            highScore = _highScore;

            UpdateScoreTexts();
        }
    }
}