using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

namespace RainyDay.UI
{
    public class CollectibleStageDisplayer : MonoBehaviour, ISubmitHandler
    {
        [SerializeField] StageManager manager;
        [SerializeField] TextMeshProUGUI indexText;
        [SerializeField] CanvasGroup contentPanel;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] TextMeshProUGUI contentText;
        [SerializeField] GameObject confirmObject;

        bool _allowInput = false;

        public void OnSubmit(BaseEventData eventData)
        {
            if (_allowInput)
            {
                _allowInput = false;

                GetComponent<CanvasGroup>().alpha = 0;
                contentPanel.alpha = 0;
                contentPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
                confirmObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);

                gameObject.SetActive(false);

                manager.UnfreezeCharacter();
            }
        }

        public void Show(int index)
        {
            var collectible = API.Catalog.Collectibles[index];

            indexText.text = $"수집품 #{index + 1}";
            descriptionText.text = collectible.Description;
            contentText.text = collectible.Content;

            gameObject.SetActive(true);
            
            DOTween.Sequence()
                .Append(GetComponent<CanvasGroup>().DOFade(1, 0.5f))
                .Append(contentPanel.DOFade(1, 0.5f))
                .Join(contentPanel.GetComponent<RectTransform>().DOAnchorPosY(-20, 0.5f))
                .InsertCallback(2, () => {
                    confirmObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(gameObject);
                    _allowInput = true;
                }).Play();
        }
    }
}
