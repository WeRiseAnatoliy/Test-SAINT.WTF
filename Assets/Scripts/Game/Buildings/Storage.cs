using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TestTask.Items;
using UnityEngine;

using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace TestTask.Buildings
{
    public class Storage : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Items")] Transform parentOfItems;
        [SerializeField, FoldoutGroup("Items")] Vector3 itemsOffset = new Vector3(0, 0.2f, 0);
        [SerializeField, FoldoutGroup("Items")] Vector3Int itemsGridSize = new Vector3Int(5, 0, 5);
        [SerializeField, FoldoutGroup("Items")] bool center = true;

        public int MaxItemsCount = 5;
        public bool IsOutput;

        public TransferData CurrentTransfer;
        public bool IsFull =>
            container.Count >= MaxItemsCount;

        private ItemDatabase itemDatabase;
        [ShowInInspector, ReadOnly, FoldoutGroup("Debug")]
        private List<string> container = new List<string>();

        public string[] Items => container.ToArray();

        public System.Action<GameObject> OnPlayerEnter;
        public System.Action<GameObject> OnPlayerExit;

        private void Start()
        {
            itemDatabase = ItemDatabase.GetDefault; //can be replaced with di
        }

        private void Update()
        {
            HandleTransferData();
        }

        public void AddItem(string itemId)
        {
            var prefab = itemDatabase[itemId].Prefab;
            var instance = Instantiate(prefab, parentOfItems);
            AddItemWithInstance(itemId, instance);
        }

        public void AddItemWithInstance (string itemId, GameObject item)
        {
            container.Add(itemId);
            item.transform.parent = parentOfItems;
            RecalculateItemsPosition();
        }

        public bool ContainsItem(string itemId, params int[] exclude) =>
            ContainsItem(itemId, out _, exclude);

        public bool ContainsItem (string itemId, out int arrayIdx, params int[] exclude)
        {
            arrayIdx = -1;
            for(var x = 0; x < container.Count; x++)
            {
                if (exclude.Contains(x))
                    continue;

                if (container[x] == itemId)
                {
                    arrayIdx = x;
                    return true;
                }
            }
            return false;
        }

        public void RemoveItem (int idx)
        {
            container.RemoveAt(idx);
        }

        public void RecalculateItemsPosition ()
        {
            for(var x = 0; x < parentOfItems.childCount; x++)
            {
                parentOfItems.GetChild(x).transform.localPosition = CalculateChildPosition(x);
                parentOfItems.GetChild(x).transform.localRotation = Quaternion.identity;
            }
        }

        public Vector3 CalculateChildPosition(int index)
        {
            var x = index % itemsGridSize.x;
            var z = index / itemsGridSize.z;

            var position = new Vector3(x * itemsOffset.x, itemsOffset.y, z * itemsOffset.z);

            if (center)
            {
                var totalSize = new Vector3((itemsGridSize.x - 1) * itemsOffset.x, 0, (itemsGridSize.z - 1) * itemsOffset.z);
                position -= totalSize * 0.5f;
            }

            return position;
        }

        #region Transfer
        public void RequestTransfer (Storage otherStorage, GameObject item, float delay)
        {
            CurrentTransfer = new TransferData(this, otherStorage, delay, item);
            if(item)
                item.transform.parent = null;
        }

        public void CancelTransfer ()
        {
            if (CurrentTransfer.Object)
                CurrentTransfer.Object.transform.parent = CurrentTransfer.From.transform;

            CurrentTransfer = null;
        }

        private void HandleTransferData ()
        {
            if(CurrentTransfer != null)
            {
                if(CurrentTransfer.IsDone)
                {
                    ApplyTransfer();
                    CurrentTransfer = null;
                }
                else
                {
                    CurrentTransfer.UpdateObjectPositionByPercent();
                }
            }
        }

        private void ApplyTransfer ()
        {
            CurrentTransfer.Target.AddItem(container[0]);
            container.RemoveAt(0);
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                if(IsOutput && container.Count > 0)
                {
                    RequestTransfer(other.GetComponent<Storage>(),
                        parentOfItems.GetChild(0).gameObject, 
                        itemDatabase[container[0]].UploadTime);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        public class TransferData
        {
            public Storage From;
            public Storage Target;
            public GameObject Object;
            public float Delay;

            public TransferData(Storage from,
                Storage target,
                float delay,
                GameObject gameObject)
            {
                TransferTick = Time.time;
                From = from;
                Target = target;
                Delay = delay;
                Object = gameObject;
            }

            public float TransferTick { get; private set; }

            public float TransferTime =>
                Time.time - TransferTick;

            public float Percent =>
                Mathf.Clamp01(TransferTime / Delay);

            public bool IsDone =>
                TransferTime >= Delay;

            public void UpdateObjectPositionByPercent ()
            {
                if(Object)
                    Object.transform.position = Vector3.Lerp(From.parentOfItems.position, Target.parentOfItems.position, Percent);
            }
        }
    }
}