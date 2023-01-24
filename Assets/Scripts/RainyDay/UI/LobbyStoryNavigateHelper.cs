using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RainyDay.UI
{
    public class LobbyStoryNavigateHelper : NavigateHelper
    {
        [SerializeField] Button[] buttons;

        protected override void GripFocus()
        {
            foreach(var button in buttons)
                if (button.IsInteractable())
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                    break;
                }
        }
    }
}
