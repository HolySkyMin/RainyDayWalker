using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RainyDay.UI
{
    [RequireComponent(typeof(Selectable))]
    public class NavigatableScrollRectItem : MonoBehaviour, ISelectHandler
    {
        [SerializeField] NavigatableScrollRect scrollRect;

        public void OnSelect(BaseEventData eventData)
        {
            scrollRect.OnContentItemSelected(GetComponent<RectTransform>());
        }
    }
}
