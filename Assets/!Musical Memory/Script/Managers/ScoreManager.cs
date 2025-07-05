using DG.Tweening;
using IA.ScriptableEvent.Channel;
using IA.Utils;
using MusicalMemory.CandleSystem;
using MusicalMemory.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace MusicalMemory.ScoreSystem
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private Tween progressTween;
        [Header("Tween Properties")]
        [SerializeField] private TweenProperty progressTweenProperty = new TweenProperty(0.19f, Ease.InOutBounce);


        [Header("Callbacks")]
        [SerializeField] private Int32ValueEventChannel currentScore_EventChannel;
        [SerializeField] private Int32ValueEventChannel highScore_EventChannel;
        [SerializeField] private FloatValueEventChannel scorePercent_EventChannel;
        [SerializeField] private FloatValueEventChannel scorePercentWithAnimation_EventChannel;
        private UnityAction onScoreUpdated;

        private void Start()
        {
            CandleManager.Instance.OnRemainedPropertyChanged.AddListener(OnRemainedPropertyChanged);
            ResetCurrentScore();
            ResetScorePercent();

            ParticleManager.Instance.OnFireParticleFinished.AddListener(OnFireParticleFinished);
            ParticleManager.Instance.OnGhostParticleFinished.AddListener(OnGhostParticleFinished);
        }

        public void ResetCurrentScore() => currentScore_EventChannel.SetValue(0).RaiseEvent();
        public void ResetScorePercent()
        {
            scorePercent_EventChannel.SetValue(0f).RaiseEvent();
            scorePercentWithAnimation_EventChannel.SetValue(0).RaiseEvent();
        }

        public void UpdateScore(UnityAction _onScoreUpdated)
        {
            onScoreUpdated = _onScoreUpdated;
        }

        private void UpdateScorePercentAnimation(UnityAction _onAnimationCompleted = null)
        {
            float currentPercent = scorePercentWithAnimation_EventChannel.GetValue;
            float targetPercent = scorePercent_EventChannel.GetValue;

            UpdateScoreWithAnimation(currentPercent, targetPercent,
            _onUpdate: (value) => scorePercentWithAnimation_EventChannel.SetValue(value).RaiseEvent(),
            _animationCompleted: _onAnimationCompleted);
        }

        private void UpdateScoreWithAnimation(float _currentPercent, float _targetPercent, UnityAction<float> _onUpdate = null, UnityAction _animationCompleted = null)
        {
            if (_targetPercent > _currentPercent)
            {
                if (progressTween == null)
                {
                    progressTween = DOTween.To(
                        getter: () => _currentPercent,
                        setter: (value) => _currentPercent = value,
                        endValue: _targetPercent,
                        duration: progressTweenProperty.Duration).
                        SetEase(progressTweenProperty.EaseType).
                        OnUpdate(() => _onUpdate?.Invoke(_currentPercent)).
                        OnComplete(() =>
                        {
                            _animationCompleted?.Invoke();
                            progressTween = null;
                        });
                }
                else
                {
                    _currentPercent = _targetPercent;
                    progressTween = null;
                    _animationCompleted?.Invoke();
                    _onUpdate?.Invoke(_currentPercent);
                }
            }
            else
            {
                _currentPercent = _targetPercent;
                _onUpdate?.Invoke(_currentPercent);
            }
        }

        private void OnRemainedPropertyChanged(RemainedCandleProperty _remainedProperty)
        {
            Vector3 firePoint = Vector3.zero;
            if (_remainedProperty.SelectedCandle != null) firePoint = _remainedProperty.SelectedCandle.FirePoint.position;

            ParticleManager.Instance.PlayFireParticle(firePoint);

            float targetPercent = ((float)_remainedProperty.RemainedIndex / (float)_remainedProperty.MaxCount) * 100f;

            scorePercent_EventChannel.SetValue(targetPercent);
        }

        private void OnFireParticleFinished()
        {
            scorePercent_EventChannel.RaiseEvent();

            UpdateScorePercentAnimation(_onAnimationCompleted: () =>
            {
                if (scorePercent_EventChannel.GetValue >= 100f)
                {
                    ResetScorePercent();
                    ParticleManager.Instance.PlayGhostParticle();
                }
            });
        }

        private void OnGhostParticleFinished()
        {
            int nextScore = currentScore_EventChannel.GetValue + 1;
            currentScore_EventChannel.SetValue(nextScore).RaiseEvent();

            if (nextScore > highScore_EventChannel.GetValue)
            {
                highScore_EventChannel.SetValue(nextScore).RaiseEvent();
            }

            onScoreUpdated?.Invoke();
        }
    }
}
