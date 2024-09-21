using TestTask.Buildings;
using TestTask.UI;
using UnityEngine;

namespace TestTask.Game.UI
{
    [ResourcePath("SimpleGameScreen")]
    public class SimpleGameScreen : UIElement
    {
        [SerializeField] ObjectInfoPanel infoPanelPrefab;

        public ObjectInfoPanel CreatePanel (FactoryBuilding target)
        {
            var instance = Instantiate(infoPanelPrefab, transform);
            instance.Target = target.transform;
            instance.Text.text = string.Empty;
            instance.SetProgress(0);
            return instance;
        }
    }
}