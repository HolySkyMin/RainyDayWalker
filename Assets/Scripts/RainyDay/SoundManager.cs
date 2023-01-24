using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance => _instance;

        static SoundManager _instance;

        [System.Serializable]
        public class PreloadData
        {
            public string Key => key;

            public AudioClip Clip => clip;

            [SerializeField] string key;
            [SerializeField] AudioClip clip;
        }

        [SerializeField] AudioSource musicSource;
        [SerializeField] AudioSource effectSource;
        [SerializeField] List<PreloadData> preloads;

        Dictionary<string, AudioClip> _clips;

        private void Awake()
        {
            _instance = this;
            _clips = new Dictionary<string, AudioClip>();
        }

        void LoadMusicClip(string key)
        {
            if (!_clips.ContainsKey(key))
            {
                if (preloads.Exists(x => x.Key == key))
                {
                    var data = preloads.Find(x => x.Key == key);
                    _clips.Add(data.Key, data.Clip);
                }
                else
                {
                    var clip = Resources.Load<AudioClip>($"Musics/{key}");
                    _clips.Add(key, clip);
                }
            }
        }

        void LoadSFXClip(string key)
        {
            if (!_clips.ContainsKey(key))
            {
                if (preloads.Exists(x => x.Key == key))
                {
                    var data = preloads.Find(x => x.Key == key);
                    _clips.Add(data.Key, data.Clip);
                }
                else
                {
                    var clip = Resources.Load<AudioClip>($"SFX/{key}");
                    _clips.Add(key, clip);
                }
            }
        }

        public void PlayMusic(string key)
        {
            LoadMusicClip(key);

            musicSource.clip = _clips[key];
            musicSource.Play();
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.UnPause();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlayOneShotEffect(string key)
        {
            LoadSFXClip(key);

            effectSource.PlayOneShot(_clips[key]);
        }

        public void PlayEffect(string key)
        {
            LoadSFXClip(key);

            effectSource.clip = _clips[key];
            effectSource.Play();
        }

        public void StopEffect()
        {
            effectSource.Stop();
        }
    }
}
