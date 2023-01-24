using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace RainyDay.UI
{
    public class FloodRateDisplayer : MonoBehaviour
    {
        [SerializeField] RectTransform gauge;
        [SerializeField] TextMeshProUGUI text;

        public void OnFloodRateChange(float floodRate)
        {
            gauge.DOAnchorMin(new Vector2(0, Mathf.Lerp(-1, 0, floodRate / 100f)), 0.3f);
            gauge.DOAnchorMax(new Vector2(1, Mathf.Lerp(0, 1, floodRate / 100f)), 0.3f);
            text.text = $"{floodRate} / 100";
        }
    }
}
