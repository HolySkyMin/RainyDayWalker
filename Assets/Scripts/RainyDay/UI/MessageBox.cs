using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace RainyDay.UI
{
    /// <summary>
    /// 메시지 박스 컴포넌트입니다.
    /// </summary>
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] GameObject masterObject;
        [SerializeField] PopupPanel popupPanel;
        [SerializeField] TextMeshProUGUI headerText;
        [SerializeField] TextMeshProUGUI bodyText;
        [SerializeField] RectTransform buttonParent;
        [SerializeField] MessageBoxButton buttonTemplate;

        List<GameObject> _buttonObjects;

        private void Awake()
        {
            _buttonObjects = new List<GameObject>();

            API.Message.Raised += OnMessageRaise;
            DontDestroyOnLoad(gameObject);
        }

        void OnMessageRaise(string header, string body, params MessageResponse[] responses)
        {
            foreach (var button in _buttonObjects)
                Destroy(button);
            _buttonObjects.Clear();

            SetHeader(header);
            SetBody(body);
            if (responses.Length < 1)
                buttonParent.gameObject.SetActive(false);
            else
            {
                buttonParent.gameObject.SetActive(true);
                AddResponse(responses);
            }

            masterObject.GetComponent<NavigateHelper>().SetFirstTarget(_buttonObjects.Count > 0 ? _buttonObjects[0] : null);
            masterObject.SetActive(true);
            popupPanel.Show().Forget();
        }

        void SetHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
                headerText.gameObject.SetActive(false);
            else
            {
                headerText.gameObject.SetActive(true);
                headerText.text = header;
            }
        }

        void SetBody(string body)
        {
            bodyText.text = body;
        }

        void AddResponse(IEnumerable<MessageResponse> responses)
        {
            foreach (var response in responses)
            {
                var newButton = Instantiate(buttonTemplate, buttonParent);
                newButton.Set(response);
                newButton.Clicked += OnButtonClick;
                newButton.transform.localScale = Vector3.one;
                newButton.gameObject.SetActive(true);
                _buttonObjects.Add(newButton.gameObject);
            }
        }

        async void OnButtonClick(MessageResponse response)
        {
            await popupPanel.Hide();
            masterObject.SetActive(false);
            API.Message.SetResponse(response);
        }

        /// <summary>
        /// 현재 표시 중인 메시지 박스의 표시를 종료합니다.
        /// </summary>
        public void Hide()
        {
            HideRoutine().Forget();
        }

        async UniTaskVoid HideRoutine()
        {
            await popupPanel.Hide();
            masterObject.SetActive(false);
        }
    }
}
