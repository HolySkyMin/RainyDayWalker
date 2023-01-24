using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace RainyDay.UI
{
    public class ChatBalloon : MonoBehaviour
    {
        [SerializeField] float startDisplacement = 20f, duration = 5f;
        [SerializeField] TextMeshProUGUI talkerText;
        [SerializeField] TextMeshProUGUI contentText;
        [SerializeField] bool playSound = true;

        public void Set(string talker, string content)
        {
            talkerText.text = talker;
            contentText.text = content;
        }

        private void Start()
        {
            var xPos = transform.position.x;

            var sequence = DOTween.Sequence(gameObject)
                .Append(transform.DOMoveX(xPos - startDisplacement, 0f))
                .Append(GetComponent<CanvasGroup>().DOFade(1, 0.25f))
                .Join(transform.DOMoveX(xPos, 0.25f));

            if (duration > 0.25f)
                sequence.Insert(duration, GetComponent<CanvasGroup>().DOFade(0, 0.25f))
                    .Join(transform.DOMoveX(xPos - startDisplacement, 0.25f))
                    .AppendCallback(() =>
                    {
                        API.Dialogue.ChatKilled -= Kill;
                        Destroy(gameObject); 
                    });

            sequence.Play();

            if (playSound)
                API.Sound.PlayOneShotEffect("balloon");
        }

        public void Kill()
        {
            API.Dialogue.ChatKilled -= Kill;

            DOTween.Kill(gameObject);
            Destroy(gameObject);
        }
    }
}
