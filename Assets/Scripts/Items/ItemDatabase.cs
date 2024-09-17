using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TestTask.Items
{
    [CreateAssetMenu(menuName = "TestTask/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public static string ItemDatabseResourcePath = "Item Database";

        private static ItemDatabase lastLoaded;
        public static ItemDatabase GetDefault
        {
            get
            {
                if (lastLoaded)
                    return lastLoaded;
                lastLoaded = Resources.Load(ItemDatabseResourcePath) as ItemDatabase;
                return lastLoaded;
            }
        }

        public static string[] GetAllItemsIds(bool withSorting = true)
        {
            var result = new List<string>();
            var data = GetDefault;
            for (var x = 0; x < data.Items.Length; x++)
                result.Add(data.Items[x].ID);

            if (withSorting)
                result = result.OrderBy((item) => item).ToList();

            return result.ToArray();
        }

        public static ValueDropdownList<string> GetItemsListWithEmpty ()
        {
            return GetItemsList(true);
        }

        public static ValueDropdownList<string> GetItemsList (bool withEmpty, bool withSorting = true)
        {
            var result = new ValueDropdownList<string>();
            if (withEmpty) result.Add("-Empty-", string.Empty);

            var items = GetAllItemsIds(withSorting);
            for(var x = 0; x < items.Length; x++)
                result.Add(items[x]);

            return result;
        }

        public ItemProperties[] Items;

        public ItemProperties this[string id]
        {
            get
            {
                foreach (var v in Items.Where(v => v.ID == id))
                    return v;

                return null;
            }
        }

#if UNITY_EDITOR
        [SerializeField] bool autoFindOnValidate = true;

        private void OnValidate()
        {
            if (autoFindOnValidate == false)
                return;

            var path = AssetDatabase.GetAssetPath(GetDefault);
            var directory = System.IO.Path.GetDirectoryName(path);

            var guids = AssetDatabase.FindAssets($"t:{typeof(ItemProperties)}", new[] { directory });
            var foundItems = (from guid in guids
                              let itemPath = AssetDatabase.GUIDToAssetPath(guid)
                              let item = AssetDatabase.LoadAssetAtPath<ItemProperties>(itemPath)
                              where item != null
                              select item).ToList();

            Items = foundItems.ToArray();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}