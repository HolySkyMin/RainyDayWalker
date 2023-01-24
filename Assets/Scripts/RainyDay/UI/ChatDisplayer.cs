using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay.UI
{
    public class ChatDisplayer : MonoBehaviour
    {
        [SerializeField] ChatBalloon balloon;

        private void Start()
        {
            API.Dialogue.ChatRaised += OnChatRaise;
        }

        void OnChatRaise(string talker, string content)
        {
            var newBalloon = Instantiate(balloon, transform);
            newBalloon.Set(talker, content);
            API.Dialogue.ChatKilled += newBalloon.Kill;
            newBalloon.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            API.Dialogue.ClearChats();
            API.Dialogue.ChatRaised -= OnChatRaise;
        }
    }
}
