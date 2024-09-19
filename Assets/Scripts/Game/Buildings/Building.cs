using Sirenix.OdinInspector;
using TestTask.Items;
using UnityEngine;

namespace TestTask.Buildings
{
    public class Building : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Storages")] public Storage InputStrorage;
        [SerializeField, FoldoutGroup("Storages")] public Storage OutputStrorage;

        [FoldoutGroup("Items"), ValueDropdown(nameof(items))] public string[] InputItems;
        [FoldoutGroup("Items"), ValueDropdown(nameof(items))] public string OutputItem;

        private ItemDatabase items;

        protected virtual void Start ()
        {
            items = ItemDatabase.GetDefault;
        }

        protected virtual void Update ()
        {
            HandlePlayerTrigger();
        }

        private void HandlePlayerTrigger()
        {
            if(InputStrorage &&
                InputStrorage.CurrentTransfer == null &&
                InputStrorage.InteractTrigger.Memory.Count > 0)
            {
                var playerInv = InputStrorage.InteractTrigger.Memory[0].GetComponent<Storage>();
                HandleInputStorage(playerInv);
            }

            if (OutputStrorage &&
                OutputStrorage.CurrentTransfer == null &&
                OutputStrorage.InteractTrigger.Memory.Count > 0)
            {
                var playerInv = OutputStrorage.InteractTrigger.Memory[0].GetComponent<Storage>();
                HandleOutputStorage(playerInv);
            }
        }

        private int lastGettedItemId;
        protected virtual void HandleInputStorage (Storage playerStorage)
        {
            if (InputStrorage.IsFull ||
                playerStorage.Items.Length == 0 ||
                playerStorage.ContainsAnyItem(InputItems) == false)
                return;

            for (var x = lastGettedItemId; x < InputItems.Length; x++)
            {
                if (playerStorage.ContainsItem(InputItems[x], out var arrayIdx))
                {
                    playerStorage.RequestTransfer(InputStrorage,
                        playerStorage.ItemsParent.GetChild(arrayIdx).gameObject,
                        items[playerStorage.Items[arrayIdx]].UploadTime);

                    lastGettedItemId++;
                    if (lastGettedItemId >= InputItems.Length)
                        lastGettedItemId = 0;
                }
            }
        }

        protected virtual void HandleOutputStorage(Storage playerStorage)
        {
            var itemIdx = OutputStrorage.Items.Length - 1;

            if (playerStorage.IsFull ||
                OutputStrorage.Items.Length == 0)
                return;

            OutputStrorage.RequestTransfer(playerStorage,
                OutputStrorage.ItemsParent.GetChild(itemIdx).gameObject,
                items[OutputStrorage.Items[itemIdx]].UploadTime);
        }
    }
}