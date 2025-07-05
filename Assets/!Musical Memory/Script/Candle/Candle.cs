using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using MusicalMemory.Utils;
using IA.ScriptableEvent.Channel;

namespace MusicalMemory.CandleSystem
{
    public class Candle : MonoBehaviour
    {
#if UNITY_EDITOR
        [IA.Attributes.ReadOnly]
#endif
        public int CandleID = 0;

        [SerializeField] private SpriteRenderer candleRenderer = null;
        [SerializeField] private Collider2D candleCollider2D = null;
        [SerializeField] private AudioClipValueEventChannel soundEventChannel = null;

        public Transform FirePoint;


        [Header("Candle Visual Change Properties")]
        [SerializeField] private TweenProperty candleSwitchON = new TweenProperty(0.5f, Ease.OutElastic);
        [SerializeField] private TweenProperty candleSwitchOFF = new TweenProperty(0.2f, Ease.Linear);

        private Color candleEnableColor = Color.white;
        private Color candleDisableColor = Color.white * 0;

        public delegate void CandleHandler(Candle _candle);

        #region Event Callbacks
        public UnityEvent<Candle> OnMouseClicked;

        #endregion

        #region Builtin Fucntions
        public void SetCandleClickable(bool _enabled) => candleCollider2D.enabled = _enabled;

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClicked?.Invoke(this);
            }
        }

        #endregion

#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Enable Candle", parameters: new object[] { true, null })]
#endif
        public void EnableCandle(bool _useTweenAnimation = true, CandleHandler _onCompleted = null)
        {
            SetCandleVisualEnabled(_isEnabled: true, _useTween: _useTweenAnimation, _onCompleted);
        }

#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Disable Candle", parameters: new object[] { true, null })]
#endif
        public void DisableCandle(bool _useTweenAnimation = true, CandleHandler _onCompleted = null)
        {
            SetCandleVisualEnabled(_isEnabled: false, _useTween: _useTweenAnimation, _onCompleted);
            soundEventChannel.RaiseEvent();
        }

        public void SetCandleVisualEnabled(bool _isEnabled, bool _useTween = true, CandleHandler _onCompleted = null)
        {
            if (_isEnabled) SwitchCandleRendererColor(_useTween, candleEnableColor, candleSwitchON, _onCompleted);
            else
            {
                SwitchCandleRendererColor(_useTween, candleDisableColor, candleSwitchOFF, _onCompleted);
            }
        }

        private void SwitchCandleRendererColor(bool _useTween, Color _targetColor, TweenProperty _tweenProperty, CandleHandler _onCompleted = null)
        {
            if (_useTween)
            {
                candleRenderer.DOColor(_targetColor, _tweenProperty.Duration).
                SetEase(_tweenProperty.EaseType).OnComplete(() => _onCompleted?.Invoke(this));
            }
            else
            {
                candleRenderer.color = _targetColor;
                _onCompleted?.Invoke(this);
            }
        }
    }
}
