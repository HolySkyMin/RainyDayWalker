using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RainyDay
{
    public class ArcadeStageManager : MonoBehaviour
    {
        public float TravelDistance => _travelDistance;

        [SerializeField] Character character;
        [SerializeField] GeneratableField field;
        [SerializeField] Transform fieldParent;
        [SerializeField] UnityEvent onGameStart;
        [SerializeField] UnityEvent onGameOver;

        int _totalFieldCount;
        float _travelDistance, _generateThreshold, _currentSeedX, _currentSeedZ;

        private void Awake()
        {
            _currentSeedX = Random.Range(0f, 10000f);
            _currentSeedZ = Random.Range(0f, 1000f);
            _generateThreshold = -30;
        }

        void Start()
        {
            for(int i = 0; i < 3; i++)
                GenerateField();

            API.Sound.PlayMusic("raining");
            onGameStart.Invoke();
        }

        void LateUpdate()
        {
            _travelDistance = Mathf.Max(_travelDistance, character.transform.position.x);

            if (_travelDistance > _generateThreshold)
                GenerateField();
        }

        public void GenerateField()
        {
            var newField = Instantiate(field, fieldParent);
            newField.transform.localPosition = new Vector3(_totalFieldCount * 30, 0, 0);
            newField.GenerateField(30, 30, 50, 25, _currentSeedX, _currentSeedZ);
            _currentSeedX += 25;
            _totalFieldCount++;
            _generateThreshold += 30;
        }

        public void OnFloodRateChange(float floodRate)
        {
            if(floodRate >= 100)
            {
                character.SetOutOfControl();
                API.Player.ArcadeHighscore = Mathf.Max(API.Player.ArcadeHighscore, _travelDistance);
                API.Manager.SaveGame();
                onGameOver.Invoke();
            }
        }
    }
}
