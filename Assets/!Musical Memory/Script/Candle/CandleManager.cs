using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.Utils;
using UnityEngine.Events;
using IA.ScriptableEvent.Channel;

namespace MusicalMemory.CandleSystem
{
    public class CandleManager : Singleton<CandleManager>
    {
        [SerializeField] private List<Candle> candles = new List<Candle>();

#if UNITY_EDITOR
        [IA.Attributes.ReadOnly]
#endif
        [SerializeField] private List<int> remainedCamdlesID = new List<int>();

        [Header("On Startup")]
        [SerializeField] private float showIntroAfterSecond = 1f;
        [SerializeField] private float oneCandleEnableDurationOnIntro = 0.2f;

        [Header("On Game")]
        [SerializeField] private float candleDeactivateAfterSecond = 0.3f;
        private RemainedCandleProperty remainedProperty;

        [Space]

        [Header("Event Callbacks")]
        [Tooltip("Callback: Success for ONE candle when Player Click to Correct Candle.")]
        public UnityEvent<RemainedCandleProperty> OnRemainedCandleSuccess;

        [Tooltip("Callback: Failure for ONE candle when Player Click to Wrong Candle.")]
        public UnityEvent<RemainedCandleProperty> OnRemainedCandleFailure;

        public UnityEvent<RemainedCandleProperty> OnRemainedPropertyChanged;


        #region Builtin Functions

        private void Start()
        {
            SetupClickCallback();
        }

        private void SetupClickCallback()
        {
            foreach (Candle candle in candles)
            {
                candle.OnMouseClicked.AddListener(OnMouseClickedToCandle);
            }
        }

        #endregion

        #region Custom Functions

        public void ClearRemainedCandlesID() => remainedCamdlesID.Clear();

        // Candle Clicked by Player
        private void OnMouseClickedToCandle(Candle selectedCandle)
        {
            if (remainedCamdlesID.Count > 0 && remainedProperty.RemainedIndex < remainedProperty.MaxCount)
            {
                // Set Selected Candle in Property
                remainedProperty.SelectedCandle = selectedCandle;

                // Check Selected Candle is first ID in List (that mean Player is Selected Correct Candle)
                if (remainedCamdlesID[remainedProperty.RemainedIndex] == selectedCandle.CandleID)
                {
                    // Update RemainedCount
                    remainedProperty.IncreaseRemainedCount();

                    // Callback: Remained Property Changed
                    RunCallback(OnRemainedPropertyChanged);
                }
                else
                {
                    // Run Candle Failure State
                    RunCallback(OnRemainedCandleFailure);
                }

                // Blow out the candle
                selectedCandle.DisableCandle(_onCompleted: (_candle) => OnCandleWentOutCompleted(_candle));
                selectedCandle.SetCandleClickable(false);
            }
        }

        private void OnCandleWentOutCompleted(Candle _disabledCandle)
        {
            // Lit the Candle again
            EnableCandleRoutine(_disabledCandle, candleHandler: (_candle) => OnCandleLitCompleted(_candle));
        }

        private void OnCandleLitCompleted(Candle _litCandle)
        {
            _litCandle.SetCandleClickable(true);

            RunCallback(OnRemainedCandleSuccess);
        }

        // Callback
        private void RunCallback(UnityEvent<RemainedCandleProperty> _callback) => _callback?.Invoke(remainedProperty);

        /// <summary>
        /// Play Intro
        /// </summary>
        /// <returns></returns>
#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Intro", isIEnumerator: true)]
#endif
        public IEnumerator IntroductionRoutine(UnityAction _onCompleted = null)
        {
            SetAllCandlesState(_isEnabled: false, _useTween: false, _candlesClickable: false);

            // Enable Candles Left to Right one-by-one
            yield return new WaitForSeconds(showIntroAfterSecond);

            int i = 0;
            int candlesCount = candles.Count;

            while (i < candlesCount)
            {
                candles[i].SetCandleVisualEnabled(true);

                yield return new WaitForSeconds(oneCandleEnableDurationOnIntro);

                i++;
            }

            SetAllCandlesState(_isEnabled: true, _useTween: false, _candlesClickable: true, _onCompleted);
        }


        /// <summary>
        ///  Begin to Play
        /// </summary>
#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Play Candle", isIEnumerator: true)]
#endif
        public IEnumerator PlayRandomCandleRoutine(UnityAction _onCompleted = null)
        {
            // Disable Clickable state of Candles
            SetAllCandlesState(_isEnabled: true, _useTween: false, _candlesClickable: false);

            int i = 0;

            while (i < remainedCamdlesID.Count)
            {
                // Checkmark for current candle when Re-Enabled 
                bool _isCurrentCandleReEnabled = false;

                // 1# Disable Current Random Candle and wait for Disabling
                Candle remainedCandle = candles[remainedCamdlesID[i]];

                remainedCandle.DisableCandle(_onCompleted: (_disabledCandle) =>
                EnableCandleRoutine(_disabledCandle,
                (_enabledCandle) => _isCurrentCandleReEnabled = true));

                yield return new WaitUntil(() => _isCurrentCandleReEnabled);

                i++;
            }

            // Checkmark for Random candle when Re-Enabled 
            bool isRandomCandleReEnabled = false;

            // 1# Disable Random Candle and wait for Disabling
            Candle randomCandle = SetRandomCandleState(_isEnabled: false, _onCompleted: (_candle) =>
            {
                // 2# Re-Enable Random Candle and wait to Complete for assign Checkmark
                EnableCandleRoutine(_candle, (_lastCandle) => isRandomCandleReEnabled = true);
            });

            remainedCamdlesID.Add(randomCandle.CandleID);

            // Update Remained Property
            remainedProperty = new RemainedCandleProperty(remainedCamdlesID.Count);

            yield return new WaitUntil(() => isRandomCandleReEnabled);

            // Enable Clickable state of Candles
            SetAllCandlesState(_isEnabled: true, _useTween: false, _candlesClickable: true, _onCompleted);
        }

        // Waits to enable the candle
        private void EnableCandleRoutine(Candle _targetCandle, Candle.CandleHandler candleHandler) =>
        new WaitForSeconds(candleDeactivateAfterSecond).
            EventRoutine(() => _targetCandle.EnableCandle(_onCompleted: candleHandler)).
            StartCoroutine(this);

        private void SetAllCandlesState(bool _isEnabled, bool _useTween, bool _candlesClickable = false, UnityAction _allCandlesCompleted = null)
        {
            int completedCandlesCount = 0;

            foreach (var candle in candles)
            {
                candle.SetCandleVisualEnabled(_isEnabled, _useTween, _onCompleted: (_candle) =>
                {
                    completedCandlesCount++;

                    if (completedCandlesCount >= candles.Count)
                    {
                        _allCandlesCompleted?.Invoke();
                    }
                });

                candle.SetCandleClickable(_enabled: _candlesClickable);
            }
        }

        private Candle SetRandomCandleState(bool _isEnabled, Candle.CandleHandler _onCompleted)
        {
            Candle randomCandle = candles.GetRandomItem();
            randomCandle.DisableCandle(_isEnabled, _onCompleted: _onCompleted);

            return randomCandle;
        }

        #endregion

        #region Editor Functions
#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Update Candles ID")]
        public void UpdateCandlesID()
        {
            for (int i = 0; i < candles.Count; i++)
            {
                candles[i].CandleID = i;
                UnityEditor.EditorUtility.SetDirty(candles[i]);
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        #endregion
    }
}