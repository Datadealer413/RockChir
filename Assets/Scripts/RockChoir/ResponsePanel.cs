using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class ResponsePanel : MonoBehaviour
    {
        public bool visible { get { return panelObject.activeSelf; } set { panelObject.SetActive(value); } }
        public string response { set { responseText.text = value.ToUpper(); } }

        [SerializeField] private GameObject panelObject;
        [SerializeField] private Text responseText;
    }
}