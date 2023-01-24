using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RainyDay.UI
{
    public class DialogueProceeder : MonoBehaviour, IPointerDownHandler, ISubmitHandler
    {
        public bool AllowInput
        {
            get => _allowInput;
            set
            {
                _allowInput = value;
                _hasInput = false;

                if (_allowInput)
                    EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        public bool HasInput => _hasInput;

        bool _allowInput, _hasInput;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_allowInput)
                _hasInput = true;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (_allowInput)
                _hasInput = true;
        }
    }
}
