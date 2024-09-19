using Sirenix.OdinInspector;
using TestTask.Items;
using UnityEngine;

namespace TestTask.Buildings
{
    public class Building : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Storages")] public Storage InputStorage;
        [SerializeField, FoldoutGroup("Storages")] public Storage OutputStorage;

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
            if(InputStorage &&
                InputStorage.CurrentTransfer == null &&
                InputStorage.InteractTrigger.Memory.Count > 0)
            {
                var playerInv = InputStorage.InteractTrigger.Memory[0].GetComponent<Storage>();
                HandleInputStorage(playerInv);
            }

            if (OutputStorage &&
                OutputStorage.CurrentTransfer == null &&
                OutputStorage.InteractTrigger.Memory.Count > 0)
            {
                var playerInv = OutputStorage.InteractTrigger.Memory[0].GetComponent<Storage>();
                HandleOutputStorage(playerInv);
            }
        }

        private int lastGettedItemId;
        protected virtual void HandleInputStorage (Storage playerStorage)
        {
            if (InputStorage.IsFull ||
                playerStorage.Items.Length == 0 ||
                playerStorage.ContainsAnyItem(InputItems) == false ||
                playerStorage.CurrentTransfer != null)
                return;

            for (var x = lastGettedItemId; x < InputItems.Length; x++)
            {
                if (playerStorage.ContainsItem(InputItems[x], out var arrayIdx))
                {
                    playerStorage.RequestTransfer(InputStorage,
                        playerStorage.ItemsParent.GetChild(arrayIdx).gameObject,
                        items[playerStorage.Items[arrayIdx]].UploadTime);

                    lastGettedItemId++;
                    if (lastGettedItemId >= InputItems.Length)
                        lastGettedItemId = 0;

                    break;
                }
            }
        }

        protected virtual void HandleOutputStorage(Storage playerStorage)
        {
            var itemIdx = OutputStorage.Items.Length - 1;

            if (playerStorage.IsFull ||
                OutputStorage.Items.Length == 0)
                return;

            OutputStorage.RequestTransfer(playerStorage,
                OutputStorage.ItemsParent.GetChild(itemIdx).gameObject,
                items[OutputStorage.Items[itemIdx]].UploadTime);
        }
    }
}