using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RainyDay.UI
{
    public class ArcadeStageScoreDisplayer : MonoBehaviour
    {
        [SerializeField] ArcadeStageManager manager;
        [SerializeField] TextMeshProUGUI travelDistance, highscore;

        private void Start()
        {
            highscore.text = $"최고 기록: {API.Player.ArcadeHighscore.ToString("N2")}";
        }

        void LateUpdate()
        {
            travelDistance.text = $"이동 거리: {manager.TravelDistance.ToString("N2")}";
        }
    }
}
