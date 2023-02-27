using System;
using UnityEngine;

#pragma warning disable 0649

namespace RockChoir
{
    public class BasicViewable : MonoBehaviour, IViewable
    {
        [SerializeField] private bool showsTopMenu;
        [SerializeField] private string titleName;
        

        public bool viewActive
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public void SetView(bool viewActive)
        {
            if (viewActive)
            {
                TopMenuManager.managerInstance.menuTitle = titleName;
                TopMenuManager.managerInstance.visible = showsTopMenu;
            }
            gameObject.SetActive(viewActive);
        }
    }
}
