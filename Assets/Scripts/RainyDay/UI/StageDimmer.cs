using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RainyDay.UI
{
    public class StageDimmer : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
            GetComponent<CanvasGroup>().DOFade(1, 1f);
        }

        public void Hide()
        {
            DOTween.Sequence()
                .Append(GetComponent<CanvasGroup>().DOFade(0, 1f))
                .AppendCallback(() => { gameObject.SetActive(false); })
                .Play();
        }
    }
}
