using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace RainyDay
{
    public class CollectibleObject : MonoBehaviour
    {
        [System.Serializable]
        public class CollectibleGotEvent : UnityEvent<int> { }

        [SerializeField] StageManager manager;
        [SerializeField] Transform meshObject;
        [SerializeField] int index;
        [SerializeField] CollectibleGotEvent onCollectibleGet;

        private void Awake()
        {
            if (API.Player.Collectibles.Contains(index))
                Destroy(gameObject);
        }

        private void Update()
        {
            meshObject.RotateAround(transform.position, Vector3.up, 180 * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Collided with Object tag {other.gameObject.tag}");
            if (other.gameObject.CompareTag("Player"))
                Invoke().Forget();
        }

        async UniTaskVoid Invoke()
        {
            await manager.FreezeCharacter();
            // API.Player.Collectibles.Add(index);
            onCollectibleGet.Invoke(index);
            Destroy(gameObject);
        }
    }
}
