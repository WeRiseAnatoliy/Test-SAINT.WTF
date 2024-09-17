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

        public TransferData CurrentTransfer;
        public bool IsFull =>
            container.Count >= MaxItemsCount;

        private ItemDatabase itemDatabase;
        [ShowInInspector, ReadOnly, FoldoutGroup("Debug")]
        private List<string> container = new List<string>();

        public string[] Items => container.ToArray();

        private void Start()
        {
            itemDatabase = ItemDatabase.GetDefault; //can be replaced with di
        }

        private void Update()
        {
            HandleTransferData();
        }

        public void AddItem (string itemId)
        {
            container.Add(itemId);
            var prefab = itemDatabase[itemId].Prefab;
            Instantiate(prefab, parentOfItems);
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
            // Обчислюємо координати у сітці
            int x = index % itemsGridSize.x;
            int z = index / itemsGridSize.z;  // Використовуємо лише 2D сітку (x, z), оскільки y = 0

            // Обчислюємо позицію для кожної дитини на основі офсету
            Vector3 position = new Vector3(x * itemsOffset.x, itemsOffset.y, z * itemsOffset.z);

            // Центруємо сітку, якщо необхідно
            if (center)
            {
                Vector3 totalSize = new Vector3((itemsGridSize.x - 1) * itemsOffset.x, 0, (itemsGridSize.z - 1) * itemsOffset.z);
                position -= totalSize * 0.5f; // Віднімаємо половину розміру, щоб центр був в нульовій точці
            }

            return position;
        }

        #region Transfer
        public void RequestTransfer (Storage otherStorage, GameObject item, float delay)
        {
            CurrentTransfer = new TransferData(otherStorage, delay);
        }

        public void CancelTransfer ()
        {
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
            }
        }

        private void ApplyTransfer ()
        {
            CurrentTransfer.Target.AddItem(container[0]);
            container.RemoveAt(0);
        }
        #endregion

        public class TransferData
        {
            public Storage Target;
            public float Delay;

            public TransferData(Storage target, float delay)
            {
                TransferTick = Time.time;
                Target = target;
                Delay = delay;
            }

            public float TransferTick { get; private set; }

            public float TransferTime =>
                Time.time - TransferTick;

            public float Percent =>
                Mathf.Clamp01(TransferTime / Delay);

            public bool IsDone =>
                TransferTime >= Delay;
        }
    }
}