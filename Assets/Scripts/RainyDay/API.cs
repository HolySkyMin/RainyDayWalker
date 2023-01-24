using System.Collections.Generic;
using Newtonsoft.Json;

namespace RainyDay
{
    public static class API
    {
        public static DialogueManager Dialogue => DialogueManager.Instance;

        public static GameManager Manager => GameManager.Instance;

        public static PlayerData Player => GameManager.Instance.Player;

        public static SoundManager Sound => SoundManager.Instance;

        public static CatalogManager Catalog => _catalog;

        public static Message Message => _message;

        public static SceneManager Scene => _scene;

        static CatalogManager _catalog;
        static Message _message;
        static SceneManager _scene;

        static Dictionary<string, string> _dbs;

        static API()
        {
            _catalog = new CatalogManager();
            _message = new Message();
            _scene = new SceneManager();

            _dbs = new Dictionary<string, string>();
            SetDataBetweenScene(new DBS<int>() { Value = 0 });
        }

        public static T GetDataBetweenScene<T>(string channel = "main") => JsonConvert.DeserializeObject<T>(_dbs[channel]) ?? default;

        public static void SetDataBetweenScene<T>(T data, string channel = "main")
        {
            var json = JsonConvert.SerializeObject(data);
            if (_dbs.ContainsKey(channel))
                _dbs[channel] = json;
            else
                _dbs.Add(channel, json);
        }
    }
}
