using UnityEngine;

namespace TestTask.Items
{
    [CreateAssetMenu(menuName = "TestTask/Item Properties")]
    public class ItemProperties : ScriptableObject
    {
        public string ID;
        public string Title;
        public GameObject Prefab;

        [Range(0f, 5f)]
        public float UploadTime = 1;
    }
}