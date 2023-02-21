using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay.UI
{
    public class StagePausePanel : MonoBehaviour
    {
        //[SerializeField] Character character;

        public void PauseGame()
        {
            Time.timeScale = 0;
            gameObject.SetActive(true);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }

        public async void GoTiTitle()
        {
            gameObject.SetActive(false);
            var result = await API.Message.Show("주의", "현재 진행 상황이 저장되지 않습니다.\n\n정말 메인 화면으로 돌아가시겠습니까?", MessageResponseType.Yes, MessageResponseType.No);
            if (result.Type == MessageResponseType.Yes)
            {
                API.SetDataBetweenScene<DBS<int>>(new DBS<int>() { Value = 0 });
                API.Scene.Change("Lobby");
            }
            else
                gameObject.SetActive(true);
        }

        public async void Quit()
        {
            gameObject.SetActive(false);
            var result = await API.Message.Show("주의", "현재 진행 상황이 저장되지 않습니다.\n\n정말 종료하고 바탕 화면으로 나가시겠습니까?", MessageResponseType.Yes, MessageResponseType.No);
            if (result.Type == MessageResponseType.Yes)
            {
                Application.Quit();
            }
            else
                gameObject.SetActive(true);
        }
    }
}
