using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RainyDay
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CollectibleData
    {
        public string Stage => stage;

        public string Description => description;

        public string Content => content;

        [JsonProperty] string stage;
        [JsonProperty] string description;
        [JsonProperty] string content;
    }
}
