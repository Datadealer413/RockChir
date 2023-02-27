using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class TopMenuManager : MonoBehaviour
    {
        public bool visible { get { return menuContent.activeSelf; } set { menuContent.SetActive(value); } }
        public string menuTitle { set { menuTxt.text = value.ToUpper(); } }
        public bool loadingAnimation { set { if(loadingAnim.isInitialized) loadingAnim.SetBool("Loading", value); } }
        public bool internetConnection { get { return loadingImg.sprite == connected ? true : false; } set { loadingImg.sprite = value ? connected : disconnected; } }

        public static TopMenuManager managerInstance
        {
            get
            {
                if(_instance != null)
                {
                    return _instance;
                }
                else
                {
#if DEBUG || DEVELOPMENT_BUILD
                    Debug.Log("No instance of TopMenuManager created!");
#endif
                    return null;
                }
            }
        }

        private static TopMenuManager _instance;

        [SerializeField] private Text menuTxt;
        [SerializeField] private GameObject menuContent;
        [SerializeField] private Animator loadingAnim;
        [SerializeField] private Image loadingImg;
        [SerializeField] private Sprite connected, disconnected;
        
        public void Scanned()
        {
            loadingAnim.SetTrigger("Scanned");
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
#if DEBUG || DEVELOPMENT_BUILD
                Debug.Log("Two attempt at TopMenu instancing have been made. Should only be one!");
#endif
            }
        }
    }
}