using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RainyDay.UI
{
    public class LobbyCollectibleScreen : MonoBehaviour
    {
        [System.Serializable]
        public class ChatButtonPair
        {
            public string Key => key;

            public IEnumerable<Button> Buttons => buttons;

            [SerializeField] string key;
            [SerializeField] Button[] buttons;
        }

        [SerializeField] Toggle collectibleToggle;
        [Header("Collectibles")]
        [SerializeField] Button[] collectibleButtons;
        [SerializeField] TextMeshProUGUI collectibleTitle, collectibleDesc, collectibleContent;
        [Header("Chats")]
        [SerializeField] ChatButtonPair[] chatButtons;
        [SerializeField] TextMeshProUGUI chatTitle;
        [SerializeField] Transform chatParent;
        [SerializeField] ChatBalloon chatBalloon;

        bool _loaded;
        List<ChatBalloon> _ballonObjs;

        private void OnEnable()
        {
            if (!_loaded)
                Initialize();

            ClearCollectibleContent();
            ClearChatContent();

            collectibleToggle.isOn = true;
        }

        void Initialize()
        {
            for (int i = 0; i < collectibleButtons.Length; i++)
                collectibleButtons[i].interactable = API.Player.Collectibles.Contains(i);
            
            for (int i = 0; i < chatButtons.Length; i++)
                foreach (var button in chatButtons[i].Buttons)
                    button.interactable = API.Player.Stages[chatButtons[i].Key].Cleared;

            _ballonObjs = new List<ChatBalloon>();
            _loaded = true;
        }

        public void ClearCollectibleContent()
        {
            collectibleTitle.text = "";
            collectibleDesc.text = "";
            collectibleContent.text = "";
            chatTitle.text = "";
        }

        public void ClearChatContent()
        {
            foreach (var obj in _ballonObjs)
                obj.Kill();
            _ballonObjs.Clear();
        }

        string StageKeyToText(string key) => key switch
        {
            "prologue" => "프롤로그",
            "stage1" => "첫 번째 기억",
            "stage2" => "두 번째 기억",
            "stage3" => "세 번째 기억",
            "stage4" => "네 번째 기억",
            "epilogue" => "에필로그",
            _ => "알 수 없는 장소"
        };

        public void OnCollectibleButtonClick(int index)
        {
            var data = API.Catalog.Collectibles[index];

            collectibleTitle.text = $"<b>수집품 #{index + 1}</b><size=60%> | {StageKeyToText(data.Stage)}에서 획득";
            collectibleDesc.text = data.Description;
            collectibleContent.text = data.Content;
        }

        public void OnChatButtonClick(string chatKey)
        {
            ClearChatContent();

            var split = chatKey.Split('_');
            chatTitle.text = $"<b>채팅 기록</b><size=60%> | {StageKeyToText(split[0])} #{split[1]}";

            var data = API.Catalog.Chats[chatKey];
            foreach(var content in data.Contents)
            {
                var balloon = Instantiate(chatBalloon, chatParent);
                balloon.Set(content.Talker, content.Content);
                balloon.gameObject.SetActive(true);
                _ballonObjs.Add(balloon);
            }
        }
    }
}
