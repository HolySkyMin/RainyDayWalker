using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace RainyDay
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DialogueData
    {
        public int Type => type;

        public string Content => content;

        public string ImageKey => imageKey;

        [JsonProperty] int type;
        [JsonProperty] string content;
        [JsonProperty] string imageKey;
    }
}
