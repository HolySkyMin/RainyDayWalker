using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace RainyDay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance;

        static GameManager _instance;

        public PlayerData Player => _player;

        string FilePath => Path.Combine(Application.persistentDataPath, "playerData.rdw");

        PlayerData _player;

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                if(File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    _player = JsonConvert.DeserializeObject<PlayerData>(json);
                }
                else
                    ResetGame();
            }
            else
                Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SaveGame()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(_player));
        }

        public void ResetGame()
        {
            var asset = Resources.Load<TextAsset>("initialSave");
            _player = JsonConvert.DeserializeObject<PlayerData>(asset.text);
            Resources.UnloadAsset(asset);
            SaveGame();
        }
    }
}
