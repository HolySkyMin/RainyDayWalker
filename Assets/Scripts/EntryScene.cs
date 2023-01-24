using UnityEngine;
using RainyDay;

public class EntryScene : MonoBehaviour
{
    [SerializeField] string nextScene;

    private void Start()
    {
        API.Scene.Change(nextScene);
    }
}
