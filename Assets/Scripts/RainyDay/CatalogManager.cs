using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace RainyDay
{
    public class CatalogManager
    {
        public IReadOnlyDictionary<string, ChatData> Chats => _chats;

        public IReadOnlyList<CollectibleData> Collectibles => _collectibles;

        Dictionary<string, ChatData> _chats;
        List<CollectibleData> _collectibles;

        public CatalogManager()
        {
            _chats = GetJsonCatalog<Dictionary<string, ChatData>>("chats");
            _collectibles = GetJsonCatalog<List<CollectibleData>>("collectibles");
        }

        T GetJsonCatalog<T>(string resourceKey)
        {
            var asset = Resources.Load<TextAsset>(resourceKey);
            var catalog = JsonConvert.DeserializeObject<T>(asset.text);
            Resources.UnloadAsset(asset);
            return catalog;
        }
    }
}
