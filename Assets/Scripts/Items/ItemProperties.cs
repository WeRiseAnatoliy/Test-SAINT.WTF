using UnityEngine;

namespace TestTask.Items
{
    [CreateAssetMenu(menuName = "TestTask/Item Properties")]
    public class ItemProperties : ScriptableObject
    {
        public string ID;
        public string Title;
        public GameObject Prefab;
    }
}