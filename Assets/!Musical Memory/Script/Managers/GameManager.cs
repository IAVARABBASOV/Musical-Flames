using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.Utils;
using MusicalMemory.CandleSystem;
using IA.ScriptableEvent.Channel;
using MusicalMemory.ScoreSystem;

namespace MusicalMemory
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameProperty gameProperty = new GameProperty();

        [Header("Callbacks")]
        public Int32ValueEventChannel OnCandleRoutineWorking;
        public Int32ValueEventChannel OnCandleRoutineCompleted;
        public Int32ValueEventChannel OnGameOver;

        private Coroutine startGameCoroutine;

        public void PlayIntro()
        {
            // Play Candle Intro
            CandleManager.Instance.IntroductionRoutine(IntroCompletedCallback).StartCoroutine(this);
        }

        private void IntroCompletedCallback()
        {
            // Setup (Success/Failure) Callbacks
            CandleManager.Instance.OnRemainedCandleSuccess.AddListener(RemainedCandleSuccessCallback);
            CandleManager.Instance.OnRemainedCandleFailure.AddListener(RemainedCandleFailureCallback);

            StartGameAfterDelay(gameProperty.GetGameStartDelay_AfterIntro);
        }

        #region Game

        public void StartGameAfterDelay(float _delayInSecond)
        {
            if (startGameCoroutine == null)
            {
                OnCandleRoutineWorking.RaiseEvent();

                // Start Game after Sec.
                startGameCoroutine = new WaitForSeconds(_delayInSecond).EventRoutine(StartGame).StartCoroutine(this);
            }
        }

        public void StartGame()
        {
            CandleManager.Instance.PlayRandomCandleRoutine(RandomCandleRoutineCompleted).StartCoroutine(this);

            // Release Coroutine
            startGameCoroutine = null;
        }

        public void TryAgainGame()
        {
            ScoreManager.Instance.ResetScorePercent();
            ScoreManager.Instance.ResetCurrentScore();

            StartGameAfterDelay(gameProperty.GetGameStartDelay_AfterTryAgain);
        }

        private void RandomCandleRoutineCompleted()
        {
            OnCandleRoutineCompleted.RaiseEvent();
        }

        private void RemainedCandleSuccessCallback(RemainedCandleProperty _remainedProperty)
        {
            if (_remainedProperty.RemainedIndex >= _remainedProperty.MaxCount)
            {
                //StartGameAfterDelay(gameProperty.GetGameStartDelay_OnNextLevel);
                ScoreManager.Instance.UpdateScore(_onScoreUpdated: ScoreUpdatedCallback);
            }
        }

        private void ScoreUpdatedCallback()
        {
            StartGameAfterDelay(gameProperty.GetGameStartDelay_OnNextLevel);
        }

        private void RemainedCandleFailureCallback(RemainedCandleProperty _remainedProperty)
        {
            //  IsGameRunning = false;

            CandleManager.Instance.ClearRemainedCandlesID();

            // Game Over Callback
            OnGameOver.RaiseEvent();
        }

        #endregion
    }
}