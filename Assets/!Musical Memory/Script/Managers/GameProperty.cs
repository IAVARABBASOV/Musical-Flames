using UnityEngine;

namespace MusicalMemory
{
    [System.Serializable]
    public class GameProperty
    {
        [SerializeField] private float gameStartDelay_AfterIntro = 1f;
        [SerializeField] private float gameStartDelay_OnNextLevel = 2f;
        [SerializeField] private float gameStartDelay_AfterTryAgain = 2f;

        public float GetGameStartDelay_AfterIntro => gameStartDelay_AfterIntro;
        public float GetGameStartDelay_OnNextLevel => gameStartDelay_OnNextLevel;
        public float GetGameStartDelay_AfterTryAgain => gameStartDelay_AfterTryAgain;

    }
}