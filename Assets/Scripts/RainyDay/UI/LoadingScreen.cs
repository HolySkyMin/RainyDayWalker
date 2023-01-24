using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RainyDay.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] CanvasGroup panel;
        [SerializeField] float fadeDuration = 0.5f;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            API.Scene.SetTransitionDelay(Mathf.RoundToInt(fadeDuration * 1000));
            API.Scene.ChangeStarted += OnChangeStart;
            API.Scene.ChangeEnded += OnChangeEnd;
        }

        public void OnChangeStart(bool dontAnimate = false)
        {
            panel.gameObject.SetActive(true);
            if (dontAnimate)
                panel.alpha = 1;
            else
            {
                panel.alpha = 0;
                panel.DOFade(1, fadeDuration);
            }
            
        }

        public void OnChangeEnd(bool dontAnimate = false)
        {
            if (dontAnimate)
                panel.gameObject.SetActive(false);
            else
            {
                panel.gameObject.SetActive(true);
                panel.alpha = 1;
                panel.DOFade(0, fadeDuration).OnComplete(() => { panel.gameObject.SetActive(false); });
            }
        }
    }
}
