using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace RainyDay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PopupPanel : MonoBehaviour
    {
        [SerializeField] float startScale = 0.8f;

        public async UniTaskVoid Show(float time = 0.1f)
        {
            GetComponent<RectTransform>().localScale = startScale * Vector3.one;
            GetComponent<CanvasGroup>().alpha = 0;
            gameObject.SetActive(true);

            await DOTween.Sequence()
                .Append(GetComponent<RectTransform>().DOScale(1, time))
                .Join(GetComponent<CanvasGroup>().DOFade(1, time))
                .Play()
                .AsyncWaitForCompletion();
        }

        public async UniTask Hide(float time = 0.1f)
        {
            await DOTween.Sequence()
                .Append(GetComponent<RectTransform>().DOScale(startScale, time))
                .Join(GetComponent<CanvasGroup>().DOFade(0, time))
                .AppendCallback(() => { gameObject.SetActive(false); })
                .Play()
                .AsyncWaitForCompletion();
        }    
    }
}
