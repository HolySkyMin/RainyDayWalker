using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RainyDay.UI
{
    public class DialogueDisplayer : MonoBehaviour
    {
        [SerializeField] GameObject backPanel;
        [SerializeField] GameObject[] typeObjects;
        [SerializeField] TextMeshProUGUI[] typeTexts;
        [SerializeField] GameObject[] typeArrows;
        [SerializeField] Image type1Image;
        [SerializeField] DialogueProceeder proceeder;

        int _currentType;

        private void Start()
        {
            API.Dialogue.DialogueStarted += OnDialogueStart;
            API.Dialogue.DialogueContentRaised += OnDialogueContentRaise;
            API.Dialogue.DialogueEnded += OnDialogueEnd;
        }

        void OnDialogueStart()
        {
            backPanel.SetActive(true);
            proceeder.gameObject.SetActive(true);
            _currentType = -1;
        }

        void OnDialogueContentRaise(int type, string content, string imageKey)
        {
            OnDialogueContentRaiseInternal(type, content, imageKey).Forget();
        }

        async UniTaskVoid OnDialogueContentRaiseInternal(int type, string content, string imageKey)
        {
            if (type == 1 && !string.IsNullOrEmpty(imageKey))
            {
                var sprite = Resources.Load<Sprite>($"Dialogues/Images/{imageKey}");
                type1Image.sprite = sprite;
            }

            if (_currentType != type)
            {
                if (_currentType >= 0)
                    typeObjects[_currentType].SetActive(false);
                await UniTask.Delay(500);
                typeObjects[type].SetActive(true);
            }
            _currentType = type;

            typeTexts[_currentType].SetText(content);
            typeTexts[_currentType].maxVisibleCharacters = 0;
            await UniTask.Yield();

            API.Sound.PlayEffect("typing");
            proceeder.AllowInput = true;
            int index = 0;
            for(float clock = 0; index < typeTexts[_currentType].textInfo.characterCount; clock += Time.deltaTime)
            {
                var next = Mathf.FloorToInt(clock * 20);
                if(next > index)
                {
                    index = next;
                    typeTexts[_currentType].maxVisibleCharacters = index;
                }

                if(proceeder.AllowInput && proceeder.HasInput)
                {
                    proceeder.AllowInput = false;
                    break;
                }

                await UniTask.Yield();
            }

            API.Sound.StopEffect();
            typeTexts[_currentType].maxVisibleCharacters = typeTexts[_currentType].textInfo.characterCount;
            typeArrows[_currentType].SetActive(true);
            proceeder.AllowInput = true;
            await UniTask.WaitUntil(() => proceeder.HasInput);
            proceeder.AllowInput = false;
            typeArrows[_currentType].SetActive(false);

            API.Dialogue.ProceedToNext();
        }

        void OnDialogueEnd()
        {
            if (_currentType >= 0)
                typeObjects[_currentType].SetActive(false);
            proceeder.gameObject.SetActive(false);
            backPanel.SetActive(false);
        }
    }
}
