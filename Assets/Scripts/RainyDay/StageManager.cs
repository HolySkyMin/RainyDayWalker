using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace RainyDay
{
    public class StageManager : MonoBehaviour
    {
        [System.Serializable]
        public struct ProgressStoryData
        {
            public float TargetProgress => targetProgress;

            public string StoryKey => storyKey;

            [SerializeField, Range(0, 1)] float targetProgress;
            [SerializeField] string storyKey;
        }

        public float Progress => _progress;

        [SerializeField] string stageKey;
        [SerializeField] Character character;
        [SerializeField] Transform startLine, finishLine;
        [SerializeField] string startStory, endStory;
        [SerializeField] List<ProgressStoryData> progressStories;
        [SerializeField] UnityEvent onGameStart;
        [SerializeField] UnityEvent onGameOver;
        [SerializeField] UnityEvent onGameClear;
        [SerializeField] StageClearBehaviour afterClear;

        int _progressStoryIndex;
        float _totalDistance, _progress;

        bool _visited, _cleared;
        List<int> _collectibles;

        void Start()
        {
            _totalDistance = finishLine.position.x - startLine.position.x;
            _cleared = false;
            _collectibles = new List<int>();

            character.SetOutOfControl();
            OnStart().Forget();
        }

        async UniTaskVoid OnStart()
        {
            if (!API.Player.Stages[stageKey].Visited)
            {
                await API.Dialogue.ShowDialogueAsync(startStory);
                await UniTask.Delay(500);
                API.Player.Stages[stageKey].Visited = true;
            }

            API.Sound.PlayMusic("raining");
            character.SetIdle();
            onGameStart.Invoke();
        }

        private void LateUpdate()
        {
            _progress = Mathf.Max(_progress, (character.transform.position.x - startLine.position.x) / _totalDistance);

            if(_progressStoryIndex < progressStories.Count && _progress >= progressStories[_progressStoryIndex].TargetProgress)
            {
                API.Dialogue.ShowChat(progressStories[_progressStoryIndex].StoryKey);
                _progressStoryIndex++;
            }

            if(!_cleared && _progress >= 1)
            {
                _cleared = true;
                character.SetOutOfControl();

                OnGameClear().Forget();
            }
        }

        async UniTaskVoid OnGameClear()
        {
            onGameClear.Invoke();
            await UniTask.Delay(1000);

            if (!API.Player.Stages[stageKey].Cleared)
            {
                await API.Dialogue.ShowDialogueAsync(endStory);
                API.Player.Stages[stageKey].Cleared = true;
            }

            foreach (var collectible in _collectibles)
                API.Player.Collectibles.Add(collectible);
            API.Manager.SaveGame();
            afterClear.Invoke();
        }

        public void OnFloodRateChange(float floodRate)
        {
            if(floodRate >= 100)
            {
                character.SetOutOfControl();
                onGameOver.Invoke();
            }
        }

        public void OnCollectibleGet(int index)
        {
            _collectibles.Add(index);
        }

        public async UniTask FreezeCharacter()
        {
            await UniTask.WaitWhile(() => character.CurrentState == "Jumping");
            character.SetOutOfControl();
        }

        public void UnfreezeCharacter()
        {
            character.SetIdle();
        }

        public void OnRetryButtonClick()
        {
            API.Scene.Change($"Stage_{stageKey}", invokeChangeEnd: false);
        }

        public void OnExitButtonClick()
        {
            API.SetDataBetweenScene(new DBS<int>() { Value = 1 });
            API.Scene.Change("Lobby");
        }
    }
}
