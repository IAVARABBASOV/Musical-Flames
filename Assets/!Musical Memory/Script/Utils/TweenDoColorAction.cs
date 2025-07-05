using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace MusicalMemory.Utils
{
    public class TweenDoColorAction : MonoBehaviour
    {
        public Color StartColor;
        public Color EndColor;
        public float Duration = 0.5f;
        public Ease ColorChangeEase = Ease.Linear;

        public UnityEvent<Color> OnColorChange;
        public UnityEvent OnActionCompleted;
        public UnityEvent OnReverseCompleted;


#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Do Action")]
#endif
        public void DoAction()
        {
            ColorChangeTween(StartColor, EndColor, OnActionCompleted);
        }

#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Do Reverse")]
#endif
        public void DoReverse()
        {
            ColorChangeTween(EndColor, StartColor, OnReverseCompleted);
        }

#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Update Color to Start Color")]
#endif
        public void UpdateColorToStartColor()
        {
            OnColorChange?.Invoke(StartColor);
        }

#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Update Color to End Color")]
#endif
        public void UpdateColorToEndColor()
        {
            OnColorChange?.Invoke(EndColor);
        }

        private void ColorChangeTween(Color _startColor, Color _endColor, UnityEvent _onCompleted)
        {
            Color currentColor = _startColor;
            DOTween.To(getter: () => currentColor,
            setter: (resultColor) => currentColor = resultColor,
            _endColor, Duration).SetEase(ColorChangeEase).
            OnUpdate(() => OnColorChange?.Invoke(currentColor)).
            OnComplete(() => _onCompleted?.Invoke());
        }
    }
}