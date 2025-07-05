using DG.Tweening;
using UnityEngine;

namespace MusicalMemory.Utils
{
    public class TweenDoScale : MonoBehaviour
    {
        [SerializeField] private Transform localTransform = null;

        [Header("Tween Properties")]
        [SerializeField] private Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);
        [SerializeField] private TweenProperty scaleStart = new TweenProperty(0.5f, Ease.Linear);
        [SerializeField] private TweenProperty scaleEnd = new TweenProperty(0.5f, Ease.Linear);

        private Vector3 startScale;

        private void OnEnable()
        {
            if (localTransform == null)
                localTransform = GetComponent<Transform>();

            startScale = localTransform.localScale;
        }

#if UNITY_EDITOR
        [IA.Attributes.InspectorButton("Play Scale Animation")]
#endif
        public void PlayScaleAnimation()
        {
            if (gameObject.activeInHierarchy)
                localTransform.DOScale(targetScale, scaleStart.Duration).SetEase(scaleStart.EaseType).OnComplete(ReturnToStartScale);
        }

        private void ReturnToStartScale()
        {
            localTransform.DOScale(startScale, scaleEnd.Duration).SetEase(scaleEnd.EaseType);
        }
    }
}