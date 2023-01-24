using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RainyDay
{
    public class SceneManager
    {
        public delegate void SceneTransitionEvent(bool dontAnimate = false);

        public string CurrentScene => _currentScene;

        public event SceneTransitionEvent ChangeStarted;

        public event SceneTransitionEvent ChangeEnded;

        int _transitionDelay;
        string _currentScene;
        LinkedList<string> _previousSceneHistory;

        const int MAX_HISTORY_COUNT = 10;

        public SceneManager()
        {
            _previousSceneHistory = new LinkedList<string>();
        }

        public void SetTransitionDelay(int millisecond)
        {
            _transitionDelay = millisecond;
        }

        /// <summary>
        /// 현재 씬에서 원하는 다음 씬으로 이동합니다.
        /// </summary>
        /// <param name="nextScene"></param>
        public void Change(string nextScene, bool invokeChangeStart = true, bool invokeChangeEnd = true)
        {
            ChangeAsync(nextScene, invokeChangeStart, invokeChangeEnd).Forget();
        }

        /// <summary>
        /// 현재 씬에서 원하는 다음 씬으로 이동합니다.
        /// </summary>
        /// <param name="nextScene"></param>
        /// <returns>비동기적으로 기다릴 수 있는 <see cref="UniTask"/> 객체가 반환됩니다.</returns>
        public async UniTask ChangeAsync(string nextScene, bool invokeChangeStart = true, bool invokeChangeEnd = true)
        {
            // if (_currentScene == nextScene)
            //     return;
            
            if(invokeChangeStart)
            {
                ChangeStarted?.Invoke();
                await UniTask.Delay(_transitionDelay);
            }
            await ChangeRaw(nextScene);
            ChangeEnded?.Invoke(!invokeChangeEnd);
        }

        /// <summary>
        /// 현재 씬에서 원하는 다음 씬으로 이동합니다. 씬 이동에 따른 이벤트는 호출되지 않습니다.
        /// </summary>
        /// <param name="nextScene"></param>
        /// <returns>자세한 정보가 있는 비동기 작업 객체가 반환됩니다.</returns>
        public AsyncOperation ChangeRaw(string nextScene)
        {
            _previousSceneHistory.AddLast(_currentScene);
            if (_previousSceneHistory.Count > MAX_HISTORY_COUNT)
                _previousSceneHistory.RemoveFirst();
            _currentScene = nextScene;

            return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene);
        }
    }
}
