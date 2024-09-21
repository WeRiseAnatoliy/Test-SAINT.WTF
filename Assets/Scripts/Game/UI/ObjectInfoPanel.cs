using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Game.UI
{
    public class ObjectInfoPanel : PinnedUIPanel
    {
        public TextMeshProUGUI Text;

        [SerializeField] Image progressBar;

        public void SetProgress (float percent)
        {
            progressBar.fillAmount = percent;
        }
    }
}