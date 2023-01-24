using System.Collections.Generic;
using Newtonsoft.Json;

namespace RainyDay
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChatData
    {
        public float Interval => interval;

        public IEnumerable<ChatContentData> Contents => contents;

        [JsonProperty] float interval;
        [JsonProperty] List<ChatContentData> contents;
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChatContentData
    {
        public string Talker => talker;

        public string Content => content;

        [JsonProperty] string talker;
        [JsonProperty] string content;
    }
}
