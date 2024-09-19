using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TestTask.Items;
using TestTask.Triggers;
using UnityEngine;

using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace TestTask.Buildings
{
    public class Storage : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Items")] Transform parentOfItems;
        [SerializeField, FoldoutGroup("Items")] Vector3 itemsOffset = new Vector3(0, 0.2f, 0);
        [SerializeField, FoldoutGroup("Items")] bool center = true;

        public ObjectTrigger InteractTrigger;
        public int MaxItemsCount = 5;

        public TransferData CurrentTransfer;
        public bool IsFull =>
            container.Count >= MaxItemsCount;

        private ItemDatabase itemDatabase;
        [ShowInInspector, ReadOnly, FoldoutGroup("Debug")]
        private List<ItemMemoryData> container = new List<ItemMemoryData>();

        public ItemMemoryData[] Items => container.ToArray();
        public Transform ItemsParent => parentOfItems;

        private void Start()
        {
            itemDatabase = ItemDatabase.GetDefault; //can be replaced with di
        }

        private void Update()
        {
            HandleTransferData();
        }

        #region Add, remove, contains
        public void AddItem(string itemId)
        {
            var prefab = itemDatabase[itemId].Prefab;
            var instance = Instantiate(prefab, parentOfItems);
            AddItemWithInstance(itemId, instance);
        }

        public void AddItemWithInstance (string itemId, GameObject item)
        {
            container.Add(new ItemMemoryData(itemId, item));
            item.transform.parent = parentOfItems;
            RecalculateItemsPosition();
        }

        public bool ContainsAnyItem(params string[] itemIds) =>
            ContainsItems(true, itemIds);

        public bool ContainsAllItems(params string[] itemsIds) =>
            ContainsItems(false, itemsIds);

        public bool ContainsItems (bool anyFromList, params string[] itemIds)
        {
            var used = new List<int>();
            for(var x = 0; x < itemIds.Length; x++)
            {
                if (ContainsItem(itemIds[x], out var idx, used.ToArray()))
                {
                    used.Add(idx);
                    if (anyFromList)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            return used.Count == itemIds.Length;
        }

        public bool ContainsItem(string itemId, params int[] exclude) =>
            ContainsItem(itemId, out _, exclude);

        public bool ContainsItem (string itemId, out int arrayIdx, params int[] exclude)
        {
            arrayIdx = -1;
            for (var x = container.Count - 1; x >= 0; x--)
            {
                if (exclude.Contains(x))
                    continue;

                if (container[x].ItemId == itemId)
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
        #endregion

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
            var position = index * itemsOffset;

            if (center)
            {
                var totalSize = parentOfItems.childCount * itemsOffset;
                position -= totalSize * 0.5f;
            }

            return position;
        }

        #region Transfer
        public void RequestTransfer (Storage otherStorage, int item, float delay)
        {
            CurrentTransfer = new TransferData(this, otherStorage, delay, item);
        }

        public void CancelTransfer ()
        {
            if (CurrentTransfer != null)
                CurrentTransfer.From.Items[CurrentTransfer.ItemIndex].ItemObject.transform.parent = CurrentTransfer.From.ItemsParent;

            CurrentTransfer = null;
        }

        private void HandleTransferData ()
        {
            if(CurrentTransfer != null)
            {
                if(CurrentTransfer.IsDone)
                {
                    CurrentTransfer.UpdateObjectPositionByPercent(CurrentTransfer.From.Items[CurrentTransfer.ItemIndex].ItemObject);
                    ApplyTransfer();
                    CurrentTransfer = null;
                }
                else
                {
                    //CurrentTransfer.UpdateObjectPositionByPercent();
                }
            }
        }

        private void ApplyTransfer ()
        {
            var itemData = CurrentTransfer.From.container[CurrentTransfer.ItemIndex];
            CurrentTransfer.Target.AddItemWithInstance(itemData.ItemId, itemData.ItemObject);
            CurrentTransfer.From.container.RemoveAt(CurrentTransfer.ItemIndex);
            RecalculateItemsPosition();
        }
        #endregion

        public class TransferData
        {
            public Storage From;
            public Storage Target;
            public int ItemIndex;
            public float Delay;

            public TransferData(Storage from,
                Storage target,
                float delay,
                int gameObjectIndex)
            {
                TransferTick = Time.time;
                From = from;
                Target = target;
                Delay = delay;
                ItemIndex = gameObjectIndex;
            }

            public float TransferTick { get; private set; }

            public float TransferTime =>
                Time.time - TransferTick;

            public float Percent =>
                Mathf.Clamp01(TransferTime / Delay);

            public bool IsDone =>
                TransferTime >= Delay;

            public void UpdateObjectPositionByPercent (GameObject Object)
            {
                if(Object)
                    Object.transform.position = Vector3.Lerp(From.parentOfItems.position, Target.ItemsParent.position + Target.CalculateChildPosition(Target.Items.Length), Percent);
            }
        }

        public struct ItemMemoryData
        {
            public string ItemId;
            public GameObject ItemObject;

            public ItemMemoryData(string itemId, GameObject itemObject)
            {
                ItemId = itemId;
                ItemObject = itemObject;
            }
        }
    }
}