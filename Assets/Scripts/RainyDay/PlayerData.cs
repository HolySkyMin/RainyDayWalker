using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace RainyDay
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PlayerData
    {
        public class StageData
        {
            public bool Unlocked { get; set; }

            public bool Visited { get; set; }

            public bool Cleared { get; set; }
        }

        public IReadOnlyDictionary<string, StageData> Stages => _stages;

        public ISet<int> Collectibles => _collectibles;

        public float ArcadeHighscore
        {
            get => _arcadeHighscore;
            set => _arcadeHighscore = value;
        }

        [JsonProperty] Dictionary<string, StageData> _stages;
        [JsonProperty] SortedSet<int> _collectibles;
        [JsonProperty] float _arcadeHighscore;
    }
}
