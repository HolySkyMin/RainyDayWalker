using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RainyDay.UI
{
    public class LobbyStageButton : MonoBehaviour
    {
        [SerializeField] string stageKey;
        [SerializeField] GameObject stagePanel, lockedPanel;

        private void Awake()
        {
            var unlocked = API.Player.Stages[stageKey].Unlocked;
            stagePanel.SetActive(unlocked);
            lockedPanel.SetActive(!unlocked);
            GetComponent<Button>().interactable = unlocked;
        }

        public void EnterStage()
        {
            API.Scene.Change($"Stage_{stageKey}", invokeChangeEnd: false);
        }
    }
}
