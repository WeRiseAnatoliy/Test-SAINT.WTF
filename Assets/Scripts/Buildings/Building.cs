using Sirenix.OdinInspector;
using UnityEngine;

namespace TestTask.Buildings
{
    public class Building : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Storages")] public Storage InputStrorage;
        [SerializeField, FoldoutGroup("Storages")] public Storage OutputStrorage;
    }
}