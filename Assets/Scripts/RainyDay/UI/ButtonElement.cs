using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RainyDay.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonElement<T> : MonoBehaviour
    {
        public T Parameter => _parameter;

        public event System.Action<T> Clicked;

        [SerializeField] TextMeshProUGUI text;

        T _parameter;

        public virtual void Set(T parameter)
        {
            _parameter = parameter;
            text.text = GetText();
        }

        public virtual void Set(T parameter, string txt)
        {
            _parameter = parameter;
            text.text = txt;
        }

        protected abstract string GetText();

        public void OnClick()
        {
            Clicked?.Invoke(_parameter);
        }
    }
}
