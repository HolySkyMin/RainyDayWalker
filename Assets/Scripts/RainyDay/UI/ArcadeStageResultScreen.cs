using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RainyDay.UI
{
    public class ArcadeStageResultScreen : MonoBehaviour
    {
        [SerializeField] ArcadeStageManager manager;
        [SerializeField] TextMeshProUGUI travelDistance;
        [SerializeField] GameObject highscoreAchieved;

        public void OnGameOver()
        {
            travelDistance.text = manager.TravelDistance.ToString("N2");
            if (manager.TravelDistance == API.Player.ArcadeHighscore)
                highscoreAchieved.SetActive(true);
            gameObject.SetActive(true);
        }

        public void OnRetryButtonClick()
        {
            API.Scene.Change("ArcadeStage", invokeChangeEnd: false);
        }

        public void OnExitButtonClick()
        {
            API.SetDataBetweenScene(new DBS<int>() { Value = 0 });
            API.Scene.Change("Lobby");
        }
    }
}
