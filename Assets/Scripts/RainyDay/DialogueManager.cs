using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace RainyDay
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance => _instance;

        static DialogueManager _instance;

        public delegate void ChatRaisedEvent(string talker, string text);
        public delegate void DialogueContentEvent(int type, string content, string imageKey);

        public event ChatRaisedEvent ChatRaised;

        public event System.Action ChatKilled;

        public event System.Action DialogueStarted;

        public event DialogueContentEvent DialogueContentRaised;

        public event System.Action DialogueEnded;

        bool _proceedTrigger;

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowChat(string chatKey)
        {
            ShowChatAsync(chatKey).Forget();
        }

        public async UniTask ShowChatAsync(string chatKey)
        {
            // 지정된 간격으로 채팅 데이터를 재생합니다.
            foreach(var content in API.Catalog.Chats[chatKey].Contents)
            {
                ChatRaised?.Invoke(content.Talker, content.Content);
                await UniTask.Delay(Mathf.RoundToInt(API.Catalog.Chats[chatKey].Interval * 1000));
            }
        }

        public void ClearChats()
        {
            ChatKilled?.Invoke();
        }

        public async UniTask ShowDialogueAsync(string dialogueKey)
        {
            DialogueStarted?.Invoke();

            var textAsset = Resources.Load<TextAsset>($"Dialogues/Scripts/{dialogueKey}");
            var dialogue = JsonConvert.DeserializeObject<List<DialogueData>>(textAsset.text);
            Resources.UnloadAsset(textAsset);

            foreach(var data in dialogue)
            {
                // 0: 이미지 없는 대화화면
                // 1: 이미지 있는 대화화면
                // 2: 날짜 표시 화면
                // 3: BGM 재생
                // 10: 채팅 재생

                switch(data.Type)
                {
                    case 3:
                        if (data.Content == "stop")
                            API.Sound.StopMusic();
                        else
                            API.Sound.PlayMusic(data.Content);
                        break;
                    case 10:
                        DialogueContentRaised?.Invoke(-1, null, null);
                        await UniTask.Delay(500);
                        await ShowChatAsync(data.Content);
                        ChatKilled?.Invoke();
                        break;
                    default:
                        DialogueContentRaised?.Invoke(data.Type, data.Content, data.ImageKey);
                        await UniTask.WaitUntilValueChanged(this, x => x._proceedTrigger);
                        break;
                }
            }

            DialogueEnded?.Invoke();
        }

        public void ProceedToNext()
        {
            _proceedTrigger = !_proceedTrigger;
        }
    }
}
