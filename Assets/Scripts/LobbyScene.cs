using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RainyDay;
using DG.Tweening;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] GameObject[] screens;  // 0: Title, 1: StoryMode, 2: InfiniteWalk
    [SerializeField] GameObject infiniteWalkButton;

    int _currentScreen;

    private void Awake()
    {
        _currentScreen = -1;
    }

    private void Start()
    {
        var dbs = API.GetDataBetweenScene<DBS<int>>();
        ShowScreen(dbs.Value).Play();
    }

    Sequence ShowScreen(int index)
    {
        var sequence = DOTween.Sequence();

        if (_currentScreen >= 0)
        {
            sequence
                .Append(screens[_currentScreen].GetComponent<CanvasGroup>().DOFade(0, 0.5f))
                .AppendCallback(() => { screens[_currentScreen].SetActive(false); });
        }
        sequence
            .AppendCallback(() => 
            { 
                screens[index].SetActive(true); 
                _currentScreen = index;
            })
            .Append(screens[index].GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        return sequence;
    }

    public void OnStoryModeButtonClick()
    {
        if (!API.Player.Stages["prologue"].Visited)
            API.Scene.Change("Stage_prologue", invokeChangeEnd: false);
        else
            ShowScreen(1).Play();
    }

    public void OnCollectibleButtonClick()
    {
        ShowScreen(2).Play();
    }

    public void OnInfiniteWalkButtonClick()
    {

    }

    public void OnResetButtonClick()
    {
        TryResetGame().Forget();
    }

    async UniTaskVoid TryResetGame()
    {
        var response = await API.Message.Show("주의", "정말 플레이 기록을 삭제하시겠습니까?", MessageResponseType.Yes, MessageResponseType.No);
        if (response.Type == MessageResponseType.Yes)
        {
            API.Manager.ResetGame();
            API.Message.Show("알림", "플레이 기록을 삭제했습니다.", MessageResponseType.OK).Forget();
        }
    }

    public void OnBackButtonClick()
    {
        ShowScreen(0).Play();
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
