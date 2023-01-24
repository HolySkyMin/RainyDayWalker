using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay
{
    public enum MessageResponseType { None, OK, Cancel, Yes, No, Custom }

    public struct MessageResponse
    {
        public MessageResponseType Type => _type;

        public int Sequence => _sequence;

        public string Text => _text;

        MessageResponseType _type;
        int _sequence;
        string _text;

        public MessageResponse(MessageResponseType type)
        {
            _type = type;
            _sequence = 0;
            _text = type switch
            {
                MessageResponseType.OK => "확인",
                MessageResponseType.Cancel => "취소",
                MessageResponseType.Yes => "예",
                MessageResponseType.No => "아니오",
                _ => "불명"
            };
        }

        public MessageResponse(int sequence, string text)
        {
            _type = MessageResponseType.Custom;
            _sequence = sequence;
            _text = text;
        }
    }
}
