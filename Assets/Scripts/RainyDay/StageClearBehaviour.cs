using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RainyDay
{
    public class StageClearBehaviour : MonoBehaviour
    {
        [SerializeField] string nextStage;

        public virtual void Invoke()
        {
            if(!API.Player.Stages[nextStage].Unlocked)
            {
                API.Player.Stages[nextStage].Unlocked = true;
                API.Manager.SaveGame();
                API.Scene.Change($"Stage_{nextStage}", false, false);
            }
            else
            {
                API.SetDataBetweenScene(new DBS<int>() { Value = 1 });
                API.Scene.Change("Lobby", invokeChangeStart: false);
            }
            // MidBuildTemp().Forget();
        }

        async UniTaskVoid MidBuildTemp()
        {
            await API.Message.Show("알림", "현재 보여드릴 수 있는 플레이는 여기까지입니다.\n\n앞으로도 Rainy Day Walker에 많은 관심 부탁드힙니다!", MessageResponseType.OK);
            API.SetDataBetweenScene(new DBS<int>() { Value = 0 });
            API.Scene.Change("Lobby", invokeChangeStart: false);
        }
    }
}
