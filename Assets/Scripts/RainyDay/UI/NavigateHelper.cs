using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RainyDay.UI
{
    public class NavigateHelper : MonoBehaviour
    {
        static event System.Action LevelChanged;

        static int _level;

        [SerializeField] GameObject firstSelected;

        int _myLevel;

        private void OnEnable()
        {
            GripFocus();
            _level++;
            _myLevel = _level;
            LevelChanged += GripFocus;
        }

        private void OnDisable()
        {
            _level--;
            LevelChanged -= GripFocus;

            LevelChanged?.Invoke();
        }

        private void OnLevelChange()
        {
            if (_level == _myLevel)
                GripFocus();
        }

        public void SetFirstTarget(GameObject obj)
        {
            firstSelected = obj;
        }

        protected virtual void GripFocus()
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}
