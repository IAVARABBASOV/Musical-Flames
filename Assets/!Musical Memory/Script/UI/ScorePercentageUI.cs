using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using IA.Utils;
using MusicalMemory.Utils;

namespace MusicalMemory.UI
{
    public class ScorePercentageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmp_Percentage = null;
        [SerializeField] private Image img_Fillable = null;


        private void OnEnable()
        {
            tmp_Percentage.gameObject.SetActive(false);
        }

        public void UpdatePercent(float _percent)
        {
            tmp_Percentage.gameObject.SetActive(_percent > 0);

            UpdateUI(_percent);
        }

        private void UpdateUI(float _percent)
        {
            _percent = _percent.CutLongValues(0);
            tmp_Percentage.SetText($"{_percent}%");

            img_Fillable.fillAmount = GetFillAmountFromPercent(_percent);
        }

        private float GetFillAmountFromPercent(float _percent) => (float)(_percent * 0.01f);
    }
}
