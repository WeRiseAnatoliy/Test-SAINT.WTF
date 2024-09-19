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

            if (InputStorage && InputStorage.InteractTrigger)
                InputStorage.InteractTrigger.OnLost += OnInteractTriggerExit;
            if (OutputStorage && OutputStorage.InteractTrigger)
                OutputStorage.InteractTrigger.OnLost += OnInteractTriggerExit;
        }

        protected virtual void Update ()
        {
            HandlePlayerTrigger();
        }

        private void OnInteractTriggerExit (GameObject player)
        {
            InputStorage?.CancelTransfer();
            OutputStorage?.CancelTransfer();
        }

        private void HandlePlayerTrigger()
        {
            if (InputStorage &&
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
                playerStorage.CurrentTransfer != null)
                return;

            if (InputItems.Length > 0 && playerStorage.ContainsAnyItem(InputItems))
            {
                for (var x = lastGettedItemId; x < InputItems.Length; x++)
                {
                    if (playerStorage.ContainsItem(InputItems[x], out var arrayIdx))
                    {
                        playerStorage.RequestTransfer(InputStorage,
                           arrayIdx,
                           items[playerStorage.Items[arrayIdx].ItemId].UploadTime);

                        lastGettedItemId++;
                        if (lastGettedItemId >= InputItems.Length)
                            lastGettedItemId = 0;

                        break;
                    }
                }
            }
            else if(InputItems.Length == 0)
            {
                var arrayIdx = 0;
                playerStorage.RequestTransfer(InputStorage,
                           arrayIdx,
                           items[playerStorage.Items[arrayIdx].ItemId].UploadTime);
            }
        }

        protected virtual void HandleOutputStorage(Storage playerStorage)
        {
            var itemIdx = OutputStorage.Items.Length - 1;

            if (playerStorage.IsFull ||
                OutputStorage.Items.Length == 0)
                return;

            OutputStorage.RequestTransfer(playerStorage,
                itemIdx,
                items[OutputStorage.Items[itemIdx].ItemId].UploadTime);
        }
    }
}