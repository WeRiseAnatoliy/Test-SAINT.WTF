using TestTask.Common;
using UnityEngine;

namespace TestTask.UI
{
    public class UIService : Singletone<UIService>
    {
        #region Static
        public static T CreateStatic<T>() where T : UIElement => Instance.CreateElement<T>();
        public static void RemoveState<T>() where T : UIElement => Instance.RemoveElement<T>();
        #endregion

        [SerializeField] Transform canvas;

        public Element CreateElement<Element> () where Element : UIElement
        {
            var attr = typeof(Element).GetCustomAttributes(typeof(ResourcePathAttribute), false)[0];
            if(attr != null)
            {
                var path = (attr as ResourcePathAttribute).Path;
                var prefab = Resources.Load<GameObject>(path);
                return Instantiate(prefab, canvas).GetComponent<Element>();
            }
            return null;
        }

        public void RemoveElement<Element>() where Element : UIElement
        {
            foreach(Transform child in transform)
            {
                if (child.GetComponent<Element>())
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }
}