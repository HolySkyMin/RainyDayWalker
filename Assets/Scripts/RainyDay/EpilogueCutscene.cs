using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace RainyDay
{
    public class EpilogueCutscene : StageClearBehaviour
    {
        [SerializeField] PlayableDirector director;
        [SerializeField] GameObject dimmer;

        public override void Invoke()
        {
            dimmer.SetActive(false);
            director.Play();
        }

        public void OnPlayChat()
        {
            API.Dialogue.ShowChat("epilogue_3");
        }

        public void OnPlayFinish()
        {
            API.Dialogue.ClearChats();
            dimmer.SetActive(true);

            PlayCreditAndTrueEnding().Forget();
        }

        async UniTask PlayCreditAndTrueEnding()
        {
            await API.Dialogue.ShowDialogueAsync("credit");

            if (API.Player.Collectibles.Count == 12)
                await API.Dialogue.ShowDialogueAsync("true_end");

            API.SetDataBetweenScene(new DBS<int>() { Value = 0 });
            API.Scene.Change("Lobby", invokeChangeStart: false);
        }
    }
}
