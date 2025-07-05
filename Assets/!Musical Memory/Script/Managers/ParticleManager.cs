using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using IA.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace MusicalMemory.ScoreSystem
{
    public class ParticleManager : Singleton<ParticleManager>
    {
        [SerializeField] private Camera cameraReference = null;
        [SerializeField] private ParticleImage fireParticle = null;
        [SerializeField] private ParticleImage ghostParticle = null;

        [Header("UI Particle Properties")]
        [SerializeField] private RectTransform fireParticleRectTransform = null;

        [Header("Callbacks")]
        public UnityEvent OnGhostParticlePlay;
        public UnityEvent OnFireParticleFinished;
        public UnityEvent OnGhostParticleFinished;

        private void Start()
        {
            fireParticle.onFirstParticleFinished.AddListener(() => OnFireParticleFinished?.Invoke());
            ghostParticle.onFirstParticleFinished.AddListener(() => OnGhostParticleFinished?.Invoke());
        }

        public void PlayFireParticle(Vector3 worldPosition)
        {
            // Step 2: Convert the world position to screen position
            Vector3 screenPosition = cameraReference.WorldToScreenPoint(worldPosition);

            // Step 3: Convert the screen position to viewport position (normalized)
            Vector3 viewportPosition = cameraReference.ScreenToViewportPoint(screenPosition);

            // Step 4: Calculate the UI position based on the viewport position
            // Here we assume the UI element is a child of a full-screen UI panel or canvas
            // and the RectTransform anchors are set appropriately to stretch with the screen.
            fireParticleRectTransform.anchorMin = viewportPosition;
            fireParticleRectTransform.anchorMax = viewportPosition;
            fireParticleRectTransform.anchoredPosition = Vector2.zero;

            fireParticle.gameObject.SetActive(true);
        }

        public void PlayGhostParticle()
        {
            OnGhostParticlePlay?.Invoke();
            ghostParticle.gameObject.SetActive(true);
        }
    }
}