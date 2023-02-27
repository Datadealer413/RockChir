using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class LeaderMenu : MonoBehaviour, IViewable
    {
        [SerializeField] private Button scanBtn, sendBtn;

        public bool viewActive
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public void SetView(bool viewActive)
        {
            gameObject.SetActive(viewActive);

            if (viewActive)
            {
                TopMenuManager.managerInstance.visible = true;
                TopMenuManager.managerInstance.menuTitle = "Leader Menu";

                if (SaveData.saveData.choirGroupName != null && SaveData.saveData.choirGroupName != string.Empty)
                {
                    scanBtn.interactable = true;

                    if (SaveData.saveData.scans.Count > 0)
                    {
                        sendBtn.interactable = true;
                    }
                }
            }
        }
    }
}
