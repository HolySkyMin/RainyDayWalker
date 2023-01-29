using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RainyDay.UI
{
    public class NavigatableScrollRect : ScrollRect
    {
        float adjustOffset = 55f;

        public void OnContentItemSelected(RectTransform item)
        {
            var leftPos = -item.anchoredPosition.x + adjustOffset;
            var rightPos = viewport.rect.width - item.anchoredPosition.x - adjustOffset;
            var upPos = -item.anchoredPosition.y - adjustOffset;
            var downPos = -viewport.rect.height - item.anchoredPosition.y + adjustOffset;

            var inView = RectTransformUtility.RectangleContainsScreenPoint(viewport, item.position);
            if(!inView)
            {
                Debug.Log($"{leftPos}, {rightPos}, {upPos}, {downPos}");
                var xPos = leftPos > content.anchoredPosition.x ? leftPos : rightPos;
                var yPos = upPos < content.anchoredPosition.y ? upPos : downPos;
                content.anchoredPosition = new Vector2(horizontal ? xPos : content.anchoredPosition.x, vertical ? yPos : content.anchoredPosition.y);
            }
        }
    }
}
