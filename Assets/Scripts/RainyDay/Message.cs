using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RainyDay
{
    public class Message
    {
        public delegate void MessageEvent(string header, string body, params MessageResponse[] responses);

        public event MessageEvent Raised;

        bool _responseSet;
        MessageResponse _response;

        public Message() { }

        /// <summary>
        /// 주어진 제목과 내용으로 알림 메시지를 표시합니다.
        /// </summary>
        /// <param name="header">표시할 제목입니다.</param>
        /// <param name="body">표시할 내용입니다.</param>
        public void Notify(string header, string body) => Show(header, body, new MessageResponse(MessageResponseType.OK)).Forget();

        /// <summary>
        /// 주어진 제목과 내용, 그리고 대답들을 사용하여 메시지를 표시합니다.
        /// </summary>
        /// <param name="header">표시할 제목입니다.</param>
        /// <param name="body">표시할 내용입니다.</param>
        /// <param name="responses">플레이어가 선택할 수 있는 응답들입니다.</param>
        /// <returns>플레이어가 선택한 응답.</returns>
        public UniTask<MessageResponse> Show(string header, string body, params MessageResponseType[] responses)
        {
            var solid = new List<MessageResponse>();
            foreach(var response in responses)
                solid.Add(new MessageResponse(response));
            return Show(header, body, solid.ToArray());
        }

        /// <summary>
        /// 주어진 제목과 내용, 그리고 대답들을 사용하여 메시지를 표시합니다.
        /// </summary>
        /// <param name="header">표시할 제목입니다.</param>
        /// <param name="body">표시할 내용입니다.</param>
        /// <param name="responses">플레이어가 선택할 수 있는 응답들입니다.</param>
        /// <returns>플레이어가 선택한 응답.</returns>
        public async UniTask<MessageResponse> Show(string header, string body, params MessageResponse[] responses)
        {
            _responseSet = false;

            Raised?.Invoke(header, body, responses);
            await UniTask.WaitUntil(() => _responseSet);
            return _response;
        }

        public void SetResponse(MessageResponse response)
        {
            _response = response;
            _responseSet = true;
        }
    }
}
