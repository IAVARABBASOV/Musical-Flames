using DG.Tweening;

namespace MusicalMemory.Utils
{
    [System.Serializable]
    public struct TweenProperty
    {
        public float Duration;
        public Ease EaseType;

        public TweenProperty(float _duration, Ease _ease)
        {
            Duration = _duration;
            EaseType = _ease;
        }
    }
}
